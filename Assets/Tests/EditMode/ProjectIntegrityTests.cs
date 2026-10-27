using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Проверки целостности проекта: ловят битые сцены и префабы до сборки.
/// Запуск: Window > General > Test Runner > EditMode.
/// </summary>
public class ProjectIntegrityTests
{
    private static IEnumerable<string> BuildScenes()
    {
        foreach (var scene in EditorBuildSettings.scenes)
            if (scene.enabled) yield return scene.path;
    }

    [Test]
    public void BuildSettings_HasEnabledScenes_AndAllExist()
    {
        var scenes = new List<string>(BuildScenes());
        Assert.IsNotEmpty(scenes, "В Build Settings нет включённых сцен.");
        foreach (var path in scenes)
            Assert.IsTrue(File.Exists(path), $"Сцена из Build Settings не найдена: {path}");
    }

    [TestCaseSource(nameof(BuildScenes))]
    public void Scene_HasNoMissingScripts_AndOneAudioListener(string path)
    {
        Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

        var missing = new List<string>();
        int listeners = 0;
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject) > 0)
                    missing.Add(t.name);
            }
            foreach (var listener in root.GetComponentsInChildren<AudioListener>(true))
                if (listener.gameObject.activeInHierarchy && listener.enabled) listeners++;
        }

        Assert.IsEmpty(missing, $"{path}: Missing Script на объектах: {string.Join(", ", missing)}");
        Assert.LessOrEqual(listeners, 1, $"{path}: активных AudioListener больше одного ({listeners}).");
    }

    [Test]
    public void Prefabs_HaveNoMissingScripts()
    {
        var broken = new List<string>();
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            foreach (var t in prefab.GetComponentsInChildren<Transform>(true))
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject) > 0)
                {
                    broken.Add($"{path} ({t.name})");
                    break;
                }
            }
        }
        Assert.IsEmpty(broken, "Префабы с Missing Script:\n" + string.Join("\n", broken));
    }

    [Test]
    public void TranslationsCsv_HasRequiredColumns()
    {
        var csv = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Data/Translations.csv");
        Assert.IsNotNull(csv, "Не найден Assets/Data/Translations.csv");
        string header = csv.text.Split('\n')[0];
        foreach (var column in new[] { "OriginalName", "Kazakh", "English" })
            StringAssert.Contains(column, header, $"В Translations.csv нет колонки {column}");
    }
}
