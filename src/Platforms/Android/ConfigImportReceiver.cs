using Android.App;
using Android.Content;
using Android.OS;
using CommunityToolkit.Mvvm.Messaging;
using Expandroid.Models;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

[BroadcastReceiver(Enabled = true, Exported = true, Name = "com.dingleinc.texttoolspro.ConfigImportReceiver")]
[IntentFilter(new[] { "com.dingleinc.texttoolspro.IMPORT_CONFIG" })]
public class ConfigImportReceiver : BroadcastReceiver
{
    //// Tasker and MacroDroid package names
    //private static readonly string[] AllowedPackages = new[]
    //{
    //    "net.dinglisch.android.taskerm",    // Tasker
    //    "com.arlosoft.macrodroid",           // MacroDroid
    //    "com.dingleinc.texttoolspro"
    //};
    private void SendMessage(string cmd, Match value)
    {
        _ = WeakReferenceMessenger.Default.Send(new AcServiceMessage((cmd, value)));
    }
    public override void OnReceive(Context context, Intent intent)
    {
        // Only check on Android 4.4+ (API 19+)
        if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
        {
            //int callingUid = Binder.CallingUid;
            //var pm = context.PackageManager;
            //var packages = pm.GetPackagesForUid(callingUid);
            //if (packages == null || !packages.Any(pkg => AllowedPackages.Contains(pkg)))
            //    return;
            string configStr = intent.GetStringExtra("config_string");
            if (!string.IsNullOrEmpty(configStr))
            {
                try
                {
                    IDeserializer deserializer = new DeserializerBuilder()
                                .WithNamingConvention(UnderscoredNamingConvention.Instance).IgnoreUnmatchedProperties()
                                .Build();
                    DictWrapper localDict = deserializer.Deserialize<DictWrapper>(configStr);
                    foreach (Match item in localDict.Matches)
                    {
                        if (item.Vars is not null)
                        {
                            bool notSupported = item.Replace is null;
                            foreach (Var x in item.Vars)
                            {
                                if (x.Type is not null)
                                {
                                    if (!AppSettings.SupportedList.Contains(x.Type))
                                    {
                                        notSupported = true;
                                        break;
                                    }
                                    else if (x.Type == "date")
                                    {
                                        try
                                        {
                                            x.Params.Format = Utils.GetTheRealFormat(x.Params.Format);
                                        }
                                        catch (Exception)
                                        {
                                            throw new Exception("Please make sure date extension parameter formats are present!");
                                        }
                                    }
                                }
                            }
                            if (notSupported)
                            {
                                continue;
                            }
                        }
                    }
                    if (localDict.Global_vars is not null)
                    {
                        string str = JsonSerializer.Serialize(localDict.Global_vars);
                        File.WriteAllText(AppSettings.GlobalVarsPath, str);
                        _ = WeakReferenceMessenger.Default.Send(new AcGlobalsMessage(localDict.Global_vars));
                    }
                    Dictionary<string, Match> dict = [];
                    foreach (Match match in localDict.Matches)
                    {
                        dict.Add(match.Trigger, match);
                    }
                    string jsonStr = JsonSerializer.Serialize(dict);
                    File.WriteAllText(AppSettings.DictPath, jsonStr);
                    SendMessage("Reset", new Match());
                    Intent resultIntent = new("com.dingleinc.texttoolspro.CONFIG_RESULT");
                    _ = resultIntent.PutExtra("status", 0); // or 1 for failure
                    context.SendBroadcast(resultIntent);

                }
                catch (Exception e)
                {
                    Intent resultIntent = new("com.dingleinc.texttoolspro.CONFIG_RESULT");
                    _ = resultIntent.PutExtra("status", e.Message); // or 1 for failure
                    context.SendBroadcast(resultIntent);
                }
            }

        }
    }
}