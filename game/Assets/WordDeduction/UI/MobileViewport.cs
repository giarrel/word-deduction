using System;
using UnityEngine;
using UnityEngine.UIElements;
namespace WordDeduction.UI
{
    internal static class MobileViewport
    {
        public static void Configure(PanelSettings panel)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            ConfigureFrameRate();
            using (var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var resources = activity.Call<AndroidJavaObject>("getResources"))
            using (var metrics = resources.Call<AndroidJavaObject>("getDisplayMetrics"))
            {
                // Android density, unlike hardware DPI, defines actual 48dp targets.
                panel.scaleMode = PanelScaleMode.ConstantPixelSize;
                panel.scale = metrics.Get<float>("density");
            }
#endif
        }
        public static void ConfigureFrameRate()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // Android otherwise defaults to 30 fps, regardless of vSyncCount.
            // Follow the current display so 90/120 Hz devices remain compatible.
            int refreshRate = (int)Math.Round(Screen.currentResolution.refreshRateRatio.value);
            Application.targetFrameRate = refreshRate > 0 ? refreshRate : 60;
#endif
        }
        public static float KeyboardHeight()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var window = activity.Call<AndroidJavaObject>("getWindow"))
                using (var decor = window.Call<AndroidJavaObject>("getDecorView"))
                using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
                {
                    if (version.GetStatic<int>("SDK_INT") >= 30)
                    {
                        using (var insets = decor.Call<AndroidJavaObject>("getRootWindowInsets"))
                        using (var types = new AndroidJavaClass("android.view.WindowInsets$Type"))
                        {
                            if (insets != null)
                            {
                                int ime = types.CallStatic<int>("ime");
                                if (!insets.Call<bool>("isVisible",ime)) return 0;
                                using (var bounds = insets.Call<AndroidJavaObject>("getInsets",ime)) return bounds.Get<int>("bottom");
                            }
                        }
                    }
                    using (var visible = new AndroidJavaObject("android.graphics.Rect"))
                    {
                        decor.Call("getWindowVisibleDisplayFrame",visible);
                        int covered = decor.Call<int>("getHeight") - visible.Get<int>("bottom");
                        return covered > Screen.height * 0.15f ? covered : 0;
                    }
                }
            }
            catch (AndroidJavaException) { return TouchScreenKeyboard.visible ? TouchScreenKeyboard.area.height : 0; }
#else
            return TouchScreenKeyboard.visible ? TouchScreenKeyboard.area.height : 0;
#endif
        }
    }
}
