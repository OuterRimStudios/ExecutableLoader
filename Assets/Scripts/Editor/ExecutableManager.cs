using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class ExecutableManager : EditorWindow
{
    ReorderableList executables;
    [SerializeField]List<string> executablePaths = new List<string>();
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
            //save a string for a directory that all of the builds will be copied to
            //use OpenFolderPanel to select the folder(build) to copy
            //once the folder has copied to a centralized directory grab the path to the exe
            //add that string to the list
            reorderableList.list.Add(EditorUtility.OpenFilePanel("Choose executable", "", "exe"));
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
        executables.DoLayoutList();

        EditorGUILayout.EndVertical();
    }
}