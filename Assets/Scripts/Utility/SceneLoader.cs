using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using UnityEngine;

using Debug = UnityEngine.Debug;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    private List<SceneInfo> sceneManifest = new List<SceneInfo>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        string manifest = FindFile(Application.dataPath, "SceneManifest.xml", true);
        if (manifest != "")
            sceneManifest = XMLOp.Deserialize<List<SceneInfo>>(manifest);
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

    public void LoadExecutable(int index)
    {
        try
        {
            Process newScene = new Process();
            newScene.StartInfo.FileName = FindFile(Application.dataPath, (sceneManifest[index].exeName != "" ? sceneManifest[index].exeName : SceneManifest.BUILD_NAME), true);
            newScene.Start();
            Application.Quit();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    string FindFile(string searchPath, string fileName, bool searchParentOnFail)
    {
        foreach(string file in Directory.EnumerateFiles(searchPath, fileName, SearchOption.AllDirectories))
            return file;

        if (searchParentOnFail)
            return FindFile(Directory.GetParent(searchPath).FullName, fileName, searchParentOnFail);
        else return "";
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
            sceneInfoList.Add(new SceneInfo(i + 1, folderNames[i], false));
        }

        XMLOp.Serialize(sceneInfoList, manifestPath + "/SceneManifest.xml");
    }

    public static void CreateSceneManifest(List<SceneInfo> sceneInfoList, string manifestPath)
    {
        XMLOp.Serialize(sceneInfoList, manifestPath + "/SceneManifest.xml");
    }
}

[Serializable]
public class SceneInfo
{
    public int sceneIndex;
    public string exeName;
    public string folderName;
    public bool hubScene;

    public SceneInfo()
    {
        sceneIndex = -1;
        exeName = "";
        folderName = "";
        hubScene = false;
    }

    public SceneInfo(int _sceneIndex, string _exePath, string _folderName, bool _hubScene)
    {
        sceneIndex = _sceneIndex;
        exeName = _exePath;
        folderName = _folderName;
        hubScene = _hubScene;
    }

    public SceneInfo(int _sceneIndex, string _folderName, bool _hubScene)
    {
        sceneIndex = _sceneIndex;
        exeName = "";
        folderName = _folderName;
        hubScene = _hubScene;
    }
}