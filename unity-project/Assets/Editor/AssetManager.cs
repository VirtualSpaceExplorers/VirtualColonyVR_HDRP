using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEditor.IMGUI.Controls;
using System.IO;
using Assets.Src.EditorExtensions;

public class AssetManager : EditorWindow
{
    [SerializeField]
    private TreeViewState _treeViewState;

    private FileTreeView _fileTreeView;

    [MenuItem("Window/AssetManager")]
    public static void ShowWindow()
    {
        var window = GetWindow<AssetManager>();
        window.titleContent = new GUIContent("Asset Manager Nexus Aurora");
        window.Show();
    }

    void OnGUI()
    {
        Rect topRect = GUILayoutUtility.GetRect(0, 60, 0, 20);

        if (GUILayout.Button(new GUIContent("Download All")))
        {
            Debug.Log("Downloading!");
        }

        Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
        _fileTreeView.OnGUI(rect);


    }

    private void OnEnable()
    {
        if (_treeViewState == null)
            _treeViewState = new TreeViewState();

        _fileTreeView = new FileTreeView(_treeViewState);

    }
}
