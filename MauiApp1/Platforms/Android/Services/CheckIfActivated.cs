using Android.AccessibilityServices;
using Android.Content;
using Android.Content.PM;
using Android.Views.Accessibility;
using MauiApp1.Models;
using Android.Provider;
using Microsoft.Maui.ApplicationModel;
using AndroidX.Activity.Result.Contract;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using CommunityToolkit.Maui.Core;
using Android;

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
        public bool RequestPermission()
        {
            var activity = Platform.CurrentActivity ?? throw new NullReferenceException("Current activity is null");

            if (ContextCompat.CheckSelfPermission(activity, Manifest.Permission.WriteExternalStorage) == Permission.Granted)
            {
                return true;
            }

            if (ActivityCompat.ShouldShowRequestPermissionRationale(activity, Manifest.Permission.WriteExternalStorage))
            {
                //await Toast.Make("Please grant access to external storage", ToastDuration.Short, 12).Show();
            }

            ActivityCompat.RequestPermissions(activity, new[] { Manifest.Permission.WriteExternalStorage }, 1);

            return false;
        }
    }
}
