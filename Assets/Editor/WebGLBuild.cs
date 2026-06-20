using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WebGLBuild
{
    private const string DefaultBuildPath = "Build/WebGL";

    public static void Build()
    {
        var buildPath = GetBuildPath();
        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            throw new InvalidOperationException("No enabled scenes found in EditorBuildSettings.");
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);

        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        PlayerSettings.WebGL.decompressionFallback = true;

        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        });

        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException($"WebGL build failed: {report.summary.result}");
        }

        Debug.Log($"WebGL build completed at {buildPath}");
    }

    private static string GetBuildPath()
    {
        var buildPath = Environment.GetEnvironmentVariable("UNITY_BUILD_PATH");
        return string.IsNullOrWhiteSpace(buildPath) ? DefaultBuildPath : buildPath;
    }
}
