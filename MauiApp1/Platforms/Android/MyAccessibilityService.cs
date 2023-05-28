using Android;
using Android.AccessibilityServices;
using Android.App;
using Android.OS;
using Android.Views.Accessibility;
using Android.Widget;

[Service(Exported = false, Label = "TextToolsPro", Permission = Manifest.Permission.BindAccessibilityService)]
[IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
[MetaData("android.accessibilityservice", Resource = "@xml/accessibility_service")]
public class MyAccessibilityService : AccessibilityService
{
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
                    if (text == "Booga")
                    {
                        Bundle args = new();
                        args.PutCharSequence(AccessibilityNodeInfo.ActionArgumentSetTextCharsequence, "Baba Yaga");
                        e.Source.PerformAction(Android.Views.Accessibility.Action.SetText, args);
                    }
                }
            }
            
        }
        catch (Exception)
        {

        }
    }
    public void performViewClick(AccessibilityNodeInfo nodeInfo)
    {
        if (nodeInfo == null)
        {
            return;
        }
        while (nodeInfo != null)
        {
            if (nodeInfo.Clickable)
            {
                nodeInfo.PerformAction(Android.Views.Accessibility.Action.Click);
                break;
            }
            nodeInfo = nodeInfo.Parent;
        }
    }

    public override void OnInterrupt()
    {
        //throw new NotImplementedException();
    }
    protected override void OnServiceConnected()
    {
        base.OnServiceConnected();

    }
}
