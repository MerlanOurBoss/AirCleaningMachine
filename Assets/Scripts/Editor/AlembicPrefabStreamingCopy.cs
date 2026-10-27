using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

/// <summary>
/// Пакет Alembic при сборке копирует в StreamingAssets только .abc плееров, которые лежат в сценах.
/// Префабы, создаваемые во время игры (например Emul в конструкторе), ищут файл по исходному пути
/// ассета: StreamingAssets/Assets/.../файл.abc. Раньше для этого в проекте лежала ручная копия
/// lll.abc на 99 МБ; теперь нужные .abc копируются в билд автоматически.
/// </summary>
public class AlembicPrefabStreamingCopy : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        string streamingAssets = GetStreamingAssetsPath(report.summary);
        if (streamingAssets == null) return;

        int copied = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null || prefab.GetComponentInChildren<AlembicStreamPlayer>(true) == null) continue;

            foreach (string dependency in AssetDatabase.GetDependencies(prefabPath, true))
            {
                if (!dependency.EndsWith(".abc", System.StringComparison.OrdinalIgnoreCase)) continue;

                string destination = Path.Combine(streamingAssets, dependency);
                if (File.Exists(destination)) continue;

                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                File.Copy(Path.GetFullPath(dependency), destination, true);
                copied++;
            }
        }

        if (copied > 0)
            Debug.Log($"[AlembicPrefabStreamingCopy] Скопировано .abc для префабов: {copied}");
    }

    private static string GetStreamingAssetsPath(BuildSummary summary)
    {
        switch (summary.platform)
        {
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneLinux64:
                return Path.ChangeExtension(summary.outputPath, null) + "_Data/StreamingAssets";
            case BuildTarget.StandaloneOSX:
                return Path.Combine(summary.outputPath, "Contents/Resources/Data/StreamingAssets");
            default:
                return null; // Alembic поддерживает только эти платформы
        }
    }
}
