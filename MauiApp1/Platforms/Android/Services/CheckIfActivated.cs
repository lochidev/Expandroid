using Android.AccessibilityServices;
using Android.Content;
using Android.Content.PM;
using Android.Views.Accessibility;
using MauiApp1.Models;
using Android.Provider;

namespace MauiApp1.Services
{
    internal class CheckIfActivated : ICheckIfActivated
    {
        public bool IsActivated()
        {
            var context = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.BaseContext;
            AccessibilityManager am = (AccessibilityManager)context.GetSystemService(Context.AccessibilityService);
            IList<AccessibilityServiceInfo> enabledServices = am.GetEnabledAccessibilityServiceList(FeedbackFlags.Generic);

            foreach (AccessibilityServiceInfo enabledService in enabledServices)
            {
                ServiceInfo enabledServiceInfo = enabledService.ResolveInfo.ServiceInfo;
                if (enabledServiceInfo.PackageName.Equals(context.PackageName))
                    return true;
            }

            return false;
        }
        public void OpenSettings()
        {
            var context = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.BaseContext;
            Intent intent = new Intent(Settings.ActionAccessibilitySettings);
            intent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
        }
    }
}
