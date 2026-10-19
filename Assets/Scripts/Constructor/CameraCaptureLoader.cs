using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // ������, ���� �� ����������� UI

public class CameraCaptureLoader : MonoBehaviour
{
    [Header("������ ��� ����������� ��������")]
    public GameObject picturePrefab;

    [Header("����� ������������ DataPath (����� DataPath/SavedObjects/Images)")]
    public string imagesSubfolder = "SavedObjects/Images";

    [Header("��������� ������� � �������� ����� ���������")]
    public Vector3 startPosition = Vector3.zero;
    public Vector3 offset = new Vector3(2f, 0f, 0f);

    [Tooltip("��� �����, ��� ����� SaveLoadManager")]
    public string constructionSceneName = "New 3";

    void Start()
    {
        LoadAndPlaceImages();
    }
    private string GetScreenshotFilePath(int sceneIndex)
    {
        string directory = Path.Combine(
            Application.persistentDataPath,
            "SavedConstructions",
            "Images"
        );

        Directory.CreateDirectory(directory);

        return Path.Combine(directory, $"cameracapture_{sceneIndex}.png");
    }

    void LoadAndPlaceImages()
    {
        // ���� � ����� � ����������
        string folderPath = Path.GetDirectoryName(GetScreenshotFilePath(0));
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"[CameraCaptureLoader] ����� �� �������: {folderPath}");
            return;
        }

        // ���� ��� ����� cameracapture_*.png
        string[] files = Directory.GetFiles(folderPath, "cameracapture_*.png");
        if (files.Length == 0)
        {
            Debug.LogWarning($"[CameraCaptureLoader] ��� ������ cameracapture_*.png � {folderPath}");
            return;
        }

        // ��������� �� ������ �� �����
        // Номер слота берём из имени файла (cameracapture_<слот>.png) — он совпадает с индексом
        // construction_<слот>.json. Раньше использовалась позиция в списке, и после удаления
        // любого слота загружалась не та конструкция.
        var sorted = files
            .Select(path => new { path, slot = ParseSlotIndex(path) })
            .Where(x => x.slot >= 0)
            .OrderBy(x => x.slot)
            .ToArray();

        // ������������ � ��������� ��������
        for (int i = 0; i < sorted.Length; i++)
        {
            string file = sorted[i].path;
            int slotIndex = sorted[i].slot;
            byte[] data = File.ReadAllBytes(file);
            
            Texture2D tex = new Texture2D(2, 2);
            if (!tex.LoadImage(data))
            {
                Debug.LogWarning($"[CameraCaptureLoader] �� ������� ��������� ����������� {file}");
                continue;
            }

            // ������ ������
            GameObject go = Instantiate(picturePrefab, transform);
            go.transform.localPosition = startPosition + offset * i;

            var btn = go.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    ConstructionSelector.SelectedIndex = slotIndex;

                    // Раньше было fileName.Contains("cameracapture_1"), что срабатывало и на слоты 10–19.
                    string sceneToLoad = slotIndex == 1 ? "CHP" : constructionSceneName;

                    SceneManager.LoadScene(sceneToLoad);
                });
                
            }

            // �������� ����� RawImage (UI)
            var raw = go.GetComponent<RawImage>();
            if (raw != null)
            {
                raw.texture = tex;
                continue;
            }

            //��� SpriteRenderer(���)
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Rect r = new Rect(0, 0, tex.width, tex.height);
                sr.sprite = Sprite.Create(tex, r, new Vector2(0.5f, 0.5f));
                continue;
            }
            Debug.LogWarning($"[CameraCaptureLoader] �� ������� ��� �� RawImage, �� SpriteRenderer.");
        }
    }

    private static int ParseSlotIndex(string path)
    {
        var match = Regex.Match(Path.GetFileName(path), @"^cameracapture_(\d+)\.png$", RegexOptions.IgnoreCase);
        return match.Success && int.TryParse(match.Groups[1].Value, out int slot) ? slot : -1;
    }
}
