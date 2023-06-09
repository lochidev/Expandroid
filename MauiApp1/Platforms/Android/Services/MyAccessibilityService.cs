using Android;
using Android.AccessibilityServices;
using Android.App;
using Android.OS;
using Android.Views.Accessibility;
using CommunityToolkit.Mvvm.Messaging;
using MauiApp1.Models;
using System.Collections.Concurrent;
using System.Security.Cryptography;

[Service(Exported = false, Label = "TextToolsPro", Permission = Manifest.Permission.BindAccessibilityService)]
[IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
[MetaData("android.accessibilityservice", Resource = "@xml/accessibility_service")]
public class MyAccessibilityService : AccessibilityService
{
    private ConcurrentDictionary<string, Match> dict;
    private List<Var> globals;

    public override void OnCreate()
    {
        base.OnCreate();
        dict = new();
        WeakReferenceMessenger.Default.Register<AcServiceMessage>(this, (r, m) =>
        {
            var cmd = m.Value.Item1;
            var item = m.Value.Item2;
            if (cmd == "Add")
            {
                if (!(string.IsNullOrEmpty(item.Trigger) || string.IsNullOrEmpty(item.Replace)))
                { 
                    dict.AddOrUpdate(item.Trigger, item,
                    (key, oldValue) => item);
                }
            }
            else if(cmd == "Quit")
            {
                DisableSelf();            
            }
            else if (cmd is not "_")
            {
                dict.Remove(item.Trigger, out var _);
            }
        });
        WeakReferenceMessenger.Default.Register<AcGlobalsMessage>(this, (r, m) =>
        {
            globals = m.Value;
        });
    }

    public override void OnAccessibilityEvent(AccessibilityEvent e)
    {
        try
        {
            if (e.Source == null)
                return;
            if (e.Source.ClassName.Equals("android.widget.EditText"))
            {
                var Text = e.Text;
                if (Text != null)
                {
                    var text = string.Join(" ", Text);
                    if (dict.TryGetValue(text, out var match))
                    {
                        Bundle args = new();
                        // echo, random, clipboard and date only supported
                        string replace = match.Replace;
                        if (globals is not null)
                        {
                            foreach (var item in globals)
                            {
                                replace = ParseItem(item, replace);
                            }
                        }
                        if (match.Vars is not null && match.Vars.Count > 0)
                        {
                            foreach (var item in match.Vars)
                            {
                                replace = ParseItem(item, replace);
                            }
                        }
                        if (replace is not null)
                        {
                            args.PutCharSequence(AccessibilityNodeInfo.ActionArgumentSetTextCharsequence, replace);
                            e.Source.PerformAction(Android.Views.Accessibility.Action.SetText, args);
                        }
                    }
                }
            }

        }
        catch (Exception)
        {

        }
    }
    private string ParseItem(Var item, string replace)
    {
        try
        {
            if (item.Type is not null)
            {
                switch (item.Type)
                {
                    case "echo":
                        replace = replace.Replace(WrapName(item.Name), item.Params.Echo);
                        break;
                    case "random":
                        var choices = item.Params.Choices;
                        replace = replace.Replace(WrapName(item.Name), choices[RandomNumberGenerator.GetInt32(0, choices.Count)]);
                        break;
                    case "clipboard":
                        if (Clipboard.Default.HasText)
                        {
                            var clip = Clipboard.Default.GetTextAsync().GetAwaiter().GetResult();
                            replace = replace.Replace(WrapName(item.Name), clip);
                        }
                        break;
                    case "date":
                        var param = item.Params;
                        var date = (DateTime.Now + TimeSpan.FromSeconds(param.Offset)).ToString(param.Format);
                        replace = replace.Replace(WrapName(item.Name), date);
                        break;
                    default:
                        break;
                }
            }
            return replace;
        }
        catch (Exception)
        {
            return null;
        }
    }
    private static string WrapName(string name)
    {
        return "{{" + name + "}}";
    }
    public override void OnInterrupt()
    {
        //throw new NotImplementedException();
    }
    protected override void OnServiceConnected()
    {
        base.OnServiceConnected();
        WeakReferenceMessenger.Default.Send(new AcServiceMessage(("_", null)));
    }
}
