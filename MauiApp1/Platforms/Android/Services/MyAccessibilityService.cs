using Android;
using Android.AccessibilityServices;
using Android.App;
using Android.OS;
using Android.Views.Accessibility;
using Android.Widget;
using CommunityToolkit.Mvvm.Messaging;
using MauiApp1.Models;
using System.Collections.Concurrent;

[Service(Exported = false, Label = "TextToolsPro", Permission = Manifest.Permission.BindAccessibilityService)]
[IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
[MetaData("android.accessibilityservice", Resource = "@xml/accessibility_service")]
public class MyAccessibilityService : AccessibilityService
{
    private ConcurrentDictionary<string, string> dict;
    public override void OnCreate()
    {
        base.OnCreate();
        dict = new ConcurrentDictionary<string, string>();
        WeakReferenceMessenger.Default.Register<AcServiceMessage>(this, (r, m) =>
        {
            var cmd = m.Value.Item1;
            var item = m.Value.Item2;
            if (cmd == "Add")
            {
                dict.AddOrUpdate(item.Key, item.Value,
                    (key, oldValue) => item.Value);
            }
            else
            {
                dict.Remove(item.Key, out var _);
            }
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
                    if (dict.TryGetValue(text, out var replace))
                    {
                        Bundle args = new();
                        args.PutCharSequence(AccessibilityNodeInfo.ActionArgumentSetTextCharsequence, replace);
                        e.Source.PerformAction(Android.Views.Accessibility.Action.SetText, args);
                    }
                }
            }
            
        }
        catch (Exception)
        {

        }
    }   

    public override void OnInterrupt()
    {
        //throw new NotImplementedException();
    }
    protected override void OnServiceConnected()
    {
        base.OnServiceConnected();
        WeakReferenceMessenger.Default.Send(new AcServiceMessage(("_", new ("_", "_"))));
    }
}
