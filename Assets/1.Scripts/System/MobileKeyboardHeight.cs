using UnityEngine;

public static class MobileUtilities
{
    // Klavye yüksekliği (piksel)
    public static int GetKeyboardHeightPixels()
    {
    #if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var activity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                var window   = activity.Call<AndroidJavaObject>("getWindow");
                var decor    = window.Call<AndroidJavaObject>("getDecorView");

                using (var rect = new AndroidJavaObject("android.graphics.Rect"))
                {
                    // Görünür alan
                    decor.Call("getWindowVisibleDisplayFrame", rect);
                    int visibleHeight = rect.Call<int>("height");

                    // Gerçek ekran yüksekliği
                    var dm = new AndroidJavaObject("android.util.DisplayMetrics");
                    var display = activity.Call<AndroidJavaObject>("getWindowManager")
                                          .Call<AndroidJavaObject>("getDefaultDisplay");
                    display.Call("getRealMetrics", dm);
                    int realHeight = dm.Get<int>("heightPixels");

                    int keyboard = Mathf.Max(0, realHeight - visibleHeight);
                    return keyboard;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("GetKeyboardHeightPixels failed: " + e.Message);
            return 0;
        }
    #elif UNITY_IOS && !UNITY_EDITOR
        return (int)TouchScreenKeyboard.area.height;
    #else
        return 0;
    #endif
    }

    // UI birimi (RectTransform/Canvas ölçeğine çevrilmiş)
    public static int GetKeyboardHeightUI(RectTransform anyOnCanvas)
    {
        int px = GetKeyboardHeightPixels();

        var canvas = anyOnCanvas ? anyOnCanvas.GetComponentInParent<Canvas>() : null;
        float scale = (canvas != null) ? canvas.scaleFactor : 1f;

        // Canvas birimine çevir
        return Mathf.RoundToInt(px / Mathf.Max(0.0001f, scale));
    }
}
