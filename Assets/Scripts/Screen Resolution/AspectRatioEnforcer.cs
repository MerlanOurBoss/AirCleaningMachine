using UnityEngine;
using System.Collections.Generic;
[ExecuteAlways]
public class AspectRatioEnforcer : MonoBehaviour
{
    private float targetAspect = 16f / 9f;

    // Сохраняем исходные rect каждой камеры
    private Dictionary<Camera, Rect> originalRects = new Dictionary<Camera, Rect>();

    void Start()
    {
        // Запоминаем исходные rect всех камер
        foreach (Camera cam in Camera.allCameras)
            originalRects[cam] = cam.rect;

        ApplyToAll();
    }

    void Update() => ApplyToAll();

    void ApplyToAll()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        // Вычисляем область 16:9 внутри окна
        Rect safeArea;
        if (scaleHeight < 1.0f)
        {
            // Letterbox: полосы сверху и снизу
            float yOffset = (1f - scaleHeight) / 2f;
            safeArea = new Rect(0f, yOffset, 1f, scaleHeight);
        }
        else
        {
            // Pillarbox: полосы слева и справа
            float scaleWidth = 1f / scaleHeight;
            float xOffset = (1f - scaleWidth) / 2f;
            safeArea = new Rect(xOffset, 0f, scaleWidth, 1f);
        }

        foreach (Camera cam in Camera.allCameras)
        {
            // Если камера новая — запомнить её исходный rect
            if (!originalRects.ContainsKey(cam))
                originalRects[cam] = cam.rect;

            Rect orig = originalRects[cam];

            // Масштабируем исходный rect внутри safeArea
            cam.rect = new Rect(
                safeArea.x + orig.x * safeArea.width,
                safeArea.y + orig.y * safeArea.height,
                orig.width  * safeArea.width,
                orig.height * safeArea.height
            );
        }
    }
}