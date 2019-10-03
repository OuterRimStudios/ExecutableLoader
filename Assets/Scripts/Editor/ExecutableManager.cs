using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Debug = UnityEngine.Debug;

public class ExecutableManager : EditorWindow
{
    ReorderableList executables;
    [SerializeField] string buildDirectory;
    [SerializeField]List<SceneInfo> executablePaths = new List<SceneInfo>();
    bool openDirectoryInfo;
    [MenuItem("Tools/Executable Manager")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ExecutableManager));
    }

    private void OnEnable()
    {
        executables = new ReorderableList(executablePaths, typeof(string), true, true, true, true);

        executables.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Executables");
        };

        executables.onAddCallback = (ReorderableList reorderableList) =>
        {
            SceneInfo newScene = new SceneInfo();
            //use OpenFolderPanel to select the folder(build) to copy
            string targetBuild = EditorUtility.OpenFilePanel("Choose executable", "", "");
            if (targetBuild == "")
                return;

            string[] splitFolders = targetBuild.Split('/');
            newScene.exeName = splitFolders[splitFolders.Length - 1];
            newScene.folderName = splitFolders[splitFolders.Length - 2];
            //Copy selected folder to centralized folder
            try
            {
                CopyDirectory(targetBuild.Substring(0, targetBuild.Length - (newScene.exeName.Length + 1)), buildDirectory + "\\" + newScene.folderName, true);
            }catch(Exception e){
                Debug.LogError(e);
            }
            //once the folder has copied to a centralized directory grab the path to the exe
            //add that string to the list
            reorderableList.list.Add(newScene);
        };

        executables.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SceneInfo sceneInfo = (SceneInfo)executables.list[index];
            if(!sceneInfo.hubScene)
                sceneInfo.sceneIndex = index + 1;
            EditorGUI.LabelField(rect, "Index: " + sceneInfo.sceneIndex + " | Name: " + sceneInfo.folderName);
        };

        var data = EditorPrefs.GetString("Executables", JsonUtility.ToJson(this, false));
        JsonUtility.FromJsonOverwrite(data, this);
    }

    private void OnDisable()
    {
        var data = JsonUtility.ToJson(this, false);
        EditorPrefs.SetString("Executables", data);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(buildDirectory);
        //The location that the builds will be copied to
        if (GUILayout.Button("Select Directory"))
        {
            string directory = EditorUtility.SaveFolderPanel("Select Directory", buildDirectory, "");
            if (directory != "") buildDirectory = directory;
        }
        if (GUILayout.Button("?", GUILayout.Width(25)))
            openDirectoryInfo = !openDirectoryInfo;
        EditorGUILayout.EndHorizontal();
        if (openDirectoryInfo)
            EditorGUILayout.HelpBox("This directory is the location that all executables will be copied to.", MessageType.Info);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        if(buildDirectory != "")
            executables.DoLayoutList();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Build"))
            Build(false);
        if (GUILayout.Button("Build & Run"))
            Build(true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    void Build(bool buildAndRun)
    {
        //Get filename
        string path = EditorUtility.SaveFolderPanel("Choose Location of Build", "", "");
        string exePath;

        if (!buildAndRun)
            exePath = BuildManager.BuildProject(path);
        else
            exePath = BuildManager.BuildRunProject(path);

        List<SceneInfo> sceneInfos = new List<SceneInfo>();
        string[] splitPath = exePath.Split('/');
        string exeName = splitPath[splitPath.Length - 1];
        string folderName = splitPath[splitPath.Length - 2];
        sceneInfos.Add(new SceneInfo(0, exeName, folderName, true));

        foreach (SceneInfo sceneInfo in executablePaths)
            sceneInfos.Add(sceneInfo);

        SceneManifest.CreateSceneManifest(sceneInfos, path);
    }

    private void CopyDirectory(string sourceDirectory, string destDirectory, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirectory);

        if (!dir.Exists)
            throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirectory);

        DirectoryInfo[] dirs = dir.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirectory))
            Directory.CreateDirectory(destDirectory);

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirectory, file.Name);
            file.CopyTo(temppath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirectory, subdir.Name);
                CopyDirectory(subdir.FullName, temppath, copySubDirs);
            }
        }
    }
}