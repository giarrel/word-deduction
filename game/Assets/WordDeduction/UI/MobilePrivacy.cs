using UnityEngine;

namespace WordDeduction.UI
{
    internal static class MobilePrivacy
    {
        static int requestGeneration;
        static volatile bool ready = true;
        public static bool Ready => ready;
        public static bool ReduceMotion { get; private set; }
        public static void Protect(bool secure)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var animator = new AndroidJavaClass("android.animation.ValueAnimator")) ReduceMotion = !animator.CallStatic<bool>("areAnimatorsEnabled");
            int generation = System.Threading.Interlocked.Increment(ref requestGeneration);
            ready = !secure;
            using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                activity.Call("runOnUiThread",new AndroidJavaRunnable(() => {
                    using (var currentPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    using (var currentActivity = currentPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    using (var window = currentActivity.Call<AndroidJavaObject>("getWindow"))
                    {
                        if (secure) window.Call("addFlags",0x00002000); else window.Call("clearFlags",0x00002000);
                        if (System.Threading.Volatile.Read(ref requestGeneration) == generation) ready = true;
                    }
                }));
            }
#else
            ready = true;
#endif
        }
    }
}
