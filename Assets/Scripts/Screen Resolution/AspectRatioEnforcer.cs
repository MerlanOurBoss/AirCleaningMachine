using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Вписывает прямоугольники всех камер в область 16:9 внутри окна (полосы сверху/снизу или по бокам).
///
/// Исправлено:
/// - убран [ExecuteAlways]: в редакторе после каждой перекомпиляции уже ужатый rect считался
///   «исходным», камеры сжимались накопительно и это сохранялось в сцену;
/// - если rect камеры поменял другой скрипт (CameraSequenceController — сетка и анимация раскрытия),
///   новое значение принимается как исходное, а не затирается каждый кадр;
/// - применение в LateUpdate — после Update и корутин других скриптов этого кадра.
/// </summary>
public class AspectRatioEnforcer : MonoBehaviour
{
    private float targetAspect = 16f / 9f;

    // Исходный rect каждой камеры (в долях полного окна) и rect, который мы ей последний раз выставили.
    private readonly Dictionary<Camera, Rect> originalRects = new Dictionary<Camera, Rect>();
    private readonly Dictionary<Camera, Rect> appliedRects = new Dictionary<Camera, Rect>();

    void LateUpdate() => ApplyToAll();

    void ApplyToAll()
    {
        if (Screen.height <= 0) return;

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
            // Новая камера или rect поменял кто-то другой — это новое исходное значение.
            if (!originalRects.ContainsKey(cam) ||
                !appliedRects.TryGetValue(cam, out Rect applied) ||
                cam.rect != applied)
            {
                originalRects[cam] = cam.rect;
            }

            Rect orig = originalRects[cam];

            // Масштабируем исходный rect внутри safeArea
            cam.rect = new Rect(
                safeArea.x + orig.x * safeArea.width,
                safeArea.y + orig.y * safeArea.height,
                orig.width  * safeArea.width,
                orig.height * safeArea.height
            );

            // Запоминаем то, что реально получилось (Unity может обрезать rect до 0..1).
            appliedRects[cam] = cam.rect;
        }
    }
}
