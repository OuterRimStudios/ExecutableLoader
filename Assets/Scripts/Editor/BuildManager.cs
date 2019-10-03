using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildManager
{
    [MenuItem("Tools/Build Manager/Build")]
    public static void BuildProject()
    {
        //Get filename
        string path = EditorUtility.SaveFolderPanel("Choose Location of Build", "", "");
        List<string> levels = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            levels.Add(scene.path);

        //Build
        List<string> folderNames = new List<string>();
        List<string> exePaths = new List<string>();
        foreach (string level in levels)
        {
            int sceneNameLength = level.Length - 19;    //the 19 comes from 'Assets/Scenes/' + '.unity' that are removed
            string folderName = level.Substring(13, sceneNameLength);
            folderNames.Add(folderName.Substring(1));   //Substring 1 to remove '/' at the beginning of the string
            string exePath = path + folderName + "/Build.exe";
            exePaths.Add(exePath);
            BuildPipeline.BuildPlayer(new string[] { level }, exePath, BuildTarget.StandaloneWindows64, BuildOptions.None);
        }

        //Create Scene Manifest
        SceneManifest.CreateSceneManifest(folderNames, path);
    }

    public static string BuildProject(string path)
    {
        string level = EditorBuildSettings.scenes[0].path;
        int sceneNameLength = level.Length - 19;    //the 19 comes from 'Assets/Scenes/' + '.unity' that are removed
        string folderName = level.Substring(13, sceneNameLength);
        string exePath = path + folderName + "/Build.exe";
        BuildPipeline.BuildPlayer(new string[] { level }, exePath, BuildTarget.StandaloneWindows64, BuildOptions.None);
        return exePath;
    }

    [MenuItem("Tools/Build Manager/Build and Run")]
    public static void BuildRunProject()
    {
        //Get filename
        string path = EditorUtility.SaveFolderPanel("Choose Location of Build", "", "");
        List<string> levels = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            levels.Add(scene.path);

        //Build
        List<string> folderNames = new List<string>();
        List<string> exePaths = new List<string>();
        foreach (string level in levels)
        {
            int sceneNameLength = level.Length - 19;    //the 19 comes from 'Assets/Scenes/' + '.unity' that are removed
            string folderName = level.Substring(13, sceneNameLength);
            folderNames.Add(folderName.Substring(1));   //Substring 1 to remove '/' at the beginning of the string
            string exePath = path + folderName + SceneManifest.BUILD_NAME;
            exePaths.Add(exePath);
            BuildPipeline.BuildPlayer(new string[] { level }, exePath, BuildTarget.StandaloneWindows64, BuildOptions.None);
        }

        //Create Scene Manifest
        SceneManifest.CreateSceneManifest(folderNames, path);

        //Run executable
        Process proc = new Process();
        proc.StartInfo.FileName = exePaths[0];
        proc.Start();
    }

    public static string BuildRunProject(string path)
    {
        string level = EditorBuildSettings.scenes[0].path;
        int sceneNameLength = level.Length - 19;    //the 19 comes from 'Assets/Scenes/' + '.unity' that are removed
        string folderName = level.Substring(13, sceneNameLength);
        string exePath = path + folderName + "/Build.exe";
        BuildPipeline.BuildPlayer(new string[] { level }, exePath, BuildTarget.StandaloneWindows64, BuildOptions.None);

        //Run executable
        Process proc = new Process();
        proc.StartInfo.FileName = exePath;
        proc.Start();

        return exePath;
    }
}