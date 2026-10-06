using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Scripting;

namespace WordDeduction.UI
{
    // Android's committed Back gesture enters the same presenter action as Escape.
    // A canceled gesture never calls onBackInvoked and never changes app state.
    internal sealed class MobileBack : IDisposable
    {
        int pending;
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject activity, dispatcher;
        BackCallback callback;
#endif
        public MobileBack()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
                if (version.GetStatic<int>("SDK_INT") < 33) return;
            using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            dispatcher = activity.Call<AndroidJavaObject>("getOnBackInvokedDispatcher");
            callback = new BackCallback(() => Interlocked.Exchange(ref pending, 1));
            var target = dispatcher; var handler = callback;
            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                target.Call("registerOnBackInvokedCallback", 0, handler)));
#endif
        }
        public bool Consume() => Interlocked.Exchange(ref pending, 0) != 0;
        public void Dispose()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (dispatcher == null) return;
            var target = dispatcher; var handler = callback;
            activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                target.Call("unregisterOnBackInvokedCallback", handler);
                target.Dispose();
            }));
            activity.Dispose(); activity = null; dispatcher = null; callback = null;
#endif
            Interlocked.Exchange(ref pending, 0);
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        [Preserve]
        sealed class BackCallback : AndroidJavaProxy
        {
            readonly Action invoke;
            public BackCallback(Action invoke) : base("android.window.OnBackInvokedCallback") { this.invoke = invoke; }
            [Preserve] public void onBackInvoked() { invoke(); }
        }
#endif
    }
}
