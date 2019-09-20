using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;

namespace MyMap.Droid
{
    [Application]
    [MetaData("com.google.android.maps.v2.API_KEY",           Value = "AIzaSyB0MpILs80kwMA3g1wsrodF6l0M_LV2d3s")]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        // JNI Handle is requried for Location Tracking
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
: base(javaReference, transfer) { }

        public override void OnCreate()
        {
            base.OnCreate(); RegisterActivityLifecycleCallbacks(this);
            CrossCurrentActivity.Current.Init(this);
        }

        public override void OnTerminate()
        {
            base.OnTerminate(); UnregisterActivityLifecycleCallbacks(this);
        }
        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }
        public void OnActivityDestroyed(Activity activity) { }
        public void OnActivityPaused(Activity activity) { }
        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }
        public void OnActivitySaveInstanceState(Activity activity, Bundle outState) { }
        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }
        public void OnActivityStopped(Activity activity) { }
    }

}
