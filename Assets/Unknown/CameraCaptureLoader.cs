using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // убрать, если не используете UI

public class CameraCaptureLoader : MonoBehaviour
{
    [Header("Префаб для отображения картинки")]
    public GameObject picturePrefab;

    [Header("Папка относительно DataPath (будет DataPath/SavedObjects/Images)")]
    public string imagesSubfolder = "SavedObjects/Images";

    [Header("Начальная позиция и смещение между префабами")]
    public Vector3 startPosition = Vector3.zero;
    public Vector3 offset = new Vector3(2f, 0f, 0f);

    [Tooltip("Имя сцены, где лежит SaveLoadManager")]
    public string constructionSceneName = "New 3";

    void Start()
    {
        LoadAndPlaceImages();
    }

    void LoadAndPlaceImages()
    {
        // Путь к папке с картинками
        string folderPath = Path.Combine(Application.dataPath, imagesSubfolder);
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"[CameraCaptureLoader] Папка не найдена: {folderPath}");
            return;
        }

        // Берём все файлы cameracapture_*.png
        string[] files = Directory.GetFiles(folderPath, "cameracapture_*.png");
        if (files.Length == 0)
        {
            Debug.LogWarning($"[CameraCaptureLoader] Нет файлов cameracapture_*.png в {folderPath}");
            return;
        }

        // Сортируем по номеру из имени
        var sorted = files.OrderBy(path =>
        {
            var name = Path.GetFileName(path);
            var m = Regex.Match(name, @"cameracapture_(\d+)\.png");
            return m.Success ? int.Parse(m.Groups[1].Value) : 0;
        }).ToArray();

        // Инстанцируем и назначаем текстуры
        for (int i = 0; i < sorted.Length; i++)
        {
            string file = sorted[i];

            int pictureIndex = i;

            byte[] data = File.ReadAllBytes(file);
            Texture2D tex = new Texture2D(2, 2);
            if (!tex.LoadImage(data))
            {
                Debug.LogWarning($"[CameraCaptureLoader] Не удалось загрузить изображение {file}");
                continue;
            }

            // Создаём префаб
            GameObject go = Instantiate(picturePrefab, transform);
            go.transform.localPosition = startPosition + offset * i;

            var btn = go.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    ConstructionSelector.SelectedIndex = pictureIndex;
                    SceneManager.LoadScene(constructionSceneName);
                });
                
            }

            // Пытаемся найти RawImage (UI)
            var raw = go.GetComponent<RawImage>();
            if (raw != null)
            {
                raw.texture = tex;
                continue;
            }

            //Или SpriteRenderer(мир)
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Rect r = new Rect(0, 0, tex.width, tex.height);
                sr.sprite = Sprite.Create(tex, r, new Vector2(0.5f, 0.5f));
                
            }
            Debug.LogWarning($"[CameraCaptureLoader] На префабе нет ни RawImage, ни SpriteRenderer.");
        }
    }
}
