using UnityEngine;

/// <summary>
/// Захват скриншота конструкции камерой. Заменяет ScreenShotTake() из
/// оригинального SaveLoadManager.
///
/// Исправленный баг оригинала: там screenshotPath сначала писался как
/// относительный путь, а через несколько строк ЗАТИРАЛСЯ абсолютным
/// (Application.persistentDataPath + relative) — то есть в файл сохранения
/// в итоге всегда попадал абсолютный путь, из-за чего сейв ломался при
/// смене машины/пользователя/переустановке. Здесь сервис только пишет PNG
/// на диск и возвращает byte[] — какой путь (относительный) сохранять
/// в SceneData, решает вызывающий код (SaveLoadManager), см. SaveFileRepository.
/// </summary>
public class ConstructionScreenshotService : MonoBehaviour
{
    [SerializeField] private Camera captureCamera;
    [SerializeField] private GameObject cameraUIToHide;
    [SerializeField] private Rect defaultCameraRect = new Rect(0.22f, 0.051f, 0.753f, 0.883f);
    [SerializeField] private int resolutionMultiplier = 3;

    public byte[] CapturePng()
    {
        if (captureCamera == null)
        {
            Debug.LogWarning("ConstructionScreenshotService: captureCamera не назначена.");
            return null;
        }

        int width = Screen.width * resolutionMultiplier;
        int height = Screen.height * resolutionMultiplier;

        if (cameraUIToHide != null) cameraUIToHide.SetActive(false);
        captureCamera.rect = new Rect(0, 0, 1, 1);

        var renderTexture = new RenderTexture(width, height, 24);
        captureCamera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;
        captureCamera.Render();

        var renderedTexture = new Texture2D(width, height);
        renderedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        renderedTexture.Apply();
        RenderTexture.active = null;

        byte[] pngBytes = renderedTexture.EncodeToPNG();

        captureCamera.rect = defaultCameraRect;
        captureCamera.targetTexture = null;
        if (cameraUIToHide != null) cameraUIToHide.SetActive(true);

        // Освобождаем GPU/CPU-память — оригинал этого не делал (утечка RenderTexture
        // и Texture2D при каждом сохранении, пока не подберёт GC/сборщик мусора Unity).
        renderTexture.Release();
        Destroy(renderTexture);
        Destroy(renderedTexture);

        return pngBytes;
    }
}