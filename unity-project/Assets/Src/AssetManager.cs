using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetManager : EditorWindow
{
    [MenuItem("Window/AssetManager")]

    public static void ShowWindow()
    {
        GetWindow(typeof(AssetManager));
    }

    void OnGUI()
    {
        
    }
}
