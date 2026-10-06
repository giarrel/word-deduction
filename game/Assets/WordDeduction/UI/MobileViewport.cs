using System;
using UnityEngine;
namespace WordDeduction.UI
{
    internal static class MobileViewport
    {
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
