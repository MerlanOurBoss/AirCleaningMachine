using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Сборка Windows-билда из меню или из командной строки (CI):
/// Unity.exe -batchmode -quit -projectPath . -executeMethod BuildScript.BuildWindows
/// При ошибке сборки процесс завершается с кодом 1.
/// </summary>
public static class BuildScript
{
    private const string OutputDir = "Builds/Windows";

    [MenuItem("Tools/Build/Windows (x64)")]
    public static void BuildWindowsMenu() => Build(exitOnFinish: false);

    public static void BuildWindows() => Build(exitOnFinish: Application.isBatchMode);

    private static void Build(bool exitOnFinish)
    {
        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        Directory.CreateDirectory(OutputDir);

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = Path.Combine(OutputDir, Application.productName + ".exe"),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
            Debug.Log($"[BuildScript] Сборка готова: {options.locationPathName}, {summary.totalSize / (1024 * 1024)} МБ, {summary.totalTime}");
        else
            Debug.LogError($"[BuildScript] Сборка не удалась: {summary.result}, ошибок: {summary.totalErrors}");

        if (exitOnFinish)
            EditorApplication.Exit(summary.result == BuildResult.Succeeded ? 0 : 1);
    }
}
