using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Создаёт Assets/Resources/ProcessLimits.asset с нормами-заглушками, если его ещё нет: ProcessMonitor
/// загружает нормы оттуда, а править их удобно в инспекторе. Меню — Tools/Process Monitor/Select Limits.
/// </summary>
[InitializeOnLoad]
internal static class ProcessLimitsAssetCreator
{
    private const string AssetPath = "Assets/Resources/" + ProcessLimits.ResourcesPath + ".asset";

    // delayCall — AssetDatabase во время загрузки домена ещё может импортировать ассеты.
    static ProcessLimitsAssetCreator() => EditorApplication.delayCall += () => EnsureAsset();

    [MenuItem("Tools/Process Monitor/Select Limits")]
    private static void SelectLimits()
    {
        var asset = EnsureAsset();
        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);
    }

    private static ProcessLimits EnsureAsset()
    {
        var asset = AssetDatabase.LoadAssetAtPath<ProcessLimits>(AssetPath);
        if (asset != null) return asset;

        Directory.CreateDirectory(Path.GetDirectoryName(AssetPath));
        asset = ScriptableObject.CreateInstance<ProcessLimits>();
        AssetDatabase.CreateAsset(asset, AssetPath);
        AssetDatabase.SaveAssets();
        Debug.Log($"[ProcessMonitor] Создан {AssetPath} с нормами-заглушками — замените их реальными нормативами.");
        return asset;
    }
}
