using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using UnityEngine;

using Debug = UnityEngine.Debug;

public class SceneLoader : MonoBehaviour
{
    private string rootPath;
    private List<SceneInfo> sceneManifest = new List<SceneInfo>();

    private void Start()
    {
        string dataPath = Application.dataPath;
        int slashCount = 0;
        int finalPathLength = 0;
        for (int i = dataPath.Length - 1; i > 0; i--)
        {
            if (dataPath[i] == '/')
                slashCount++;

            if (slashCount == 1)
                finalPathLength = i;
        }

        rootPath = dataPath.Substring(0, finalPathLength);
        if (File.Exists(rootPath + "/SceneManifest.xml"))
            sceneManifest = XMLOp.Deserialize<List<SceneInfo>>(rootPath + "/SceneManifest.xml");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Home))
        {
            LoadExecutable(0);
        }
        else if (Input.inputString.Length != 0)
        {
            for (int i = 0; i < sceneManifest.Count; i++)
            {
                if (Input.inputString[0].ToString() == sceneManifest[i].sceneIndex.ToString())
                    LoadExecutable(i);
            }
        }
    }

    private void LoadExecutable(int index)
    {
        try
        {
            Process newScene = new Process();
            newScene.StartInfo.FileName = rootPath + sceneManifest[index].folderName + SceneManifest.BUILD_NAME;
            newScene.Start();
            Application.Quit();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}

public class SceneManifest
{
    public const string BUILD_NAME = "/Build.exe";

    public static void CreateSceneManifest(List<string> folderNames, string manifestPath)
    {
        List<SceneInfo> sceneInfoList = new List<SceneInfo>();
        for (int i = 0; i < folderNames.Count; i++)
        {
            //Debug.Log(folderNames[i]);
            sceneInfoList.Add(new SceneInfo(i + 1, folderNames[i]));
        }

        XMLOp.Serialize(sceneInfoList, manifestPath + "/SceneManifest.xml");
    }
}

public class SceneInfo
{
    public int sceneIndex;
    public string folderName;

    public SceneInfo()
    {
        sceneIndex = -1;
        folderName = "";
    }

    public SceneInfo(int _sceneIndex, string _folderName)
    {
        sceneIndex = _sceneIndex;
        folderName = _folderName;
    }
}