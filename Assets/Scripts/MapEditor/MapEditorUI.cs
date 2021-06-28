#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapEditor))]
public class MapEditorUI : Editor
{
    private MapEditor mapEditor;
    private SerializedObject soMapEditor;

    private SerializedProperty mapName;

    private SerializedProperty rows;
    private SerializedProperty columns;

    private SerializedProperty brushState;
    private SerializedProperty brushStyle;
    private SerializedProperty selectedLayer;

    private SerializedProperty proceduralMap;
    private SerializedProperty perlinNoiseScale;

    private SerializedProperty mapContainerPrefab;
    private SerializedProperty tileToDraw;


    private string[] tabButtons = new string[] {"Automatic tools", "Manual tools", "Procedural tools", "Map prefabs"};

    private void OnEnable()
    {
        mapEditor = (MapEditor)target;
        soMapEditor = new SerializedObject(mapEditor);

        mapName = soMapEditor.FindProperty("mapName");

        rows = soMapEditor.FindProperty("rows");
        columns = soMapEditor.FindProperty("columns");

        brushState = soMapEditor.FindProperty("brushState");
        brushStyle = soMapEditor.FindProperty("brushStyle");
        selectedLayer = soMapEditor.FindProperty("selectedLayer");

        proceduralMap = soMapEditor.FindProperty("proceduralMap");
        perlinNoiseScale = soMapEditor.FindProperty("perlinNoiseScale");

        mapContainerPrefab = soMapEditor.FindProperty("mapContainerPrefab");
        tileToDraw = soMapEditor.FindProperty("tileToDraw");

    }


    public override void OnInspectorGUI()
    {
        soMapEditor.Update();

        EditorGUI.BeginChangeCheck();


        EditorGUILayout.PropertyField(mapName);

        GUILayout.Space(20f);

        if (EditorGUI.EndChangeCheck())
        {
            soMapEditor.ApplyModifiedProperties();
        }
        EditorGUI.BeginChangeCheck();

        

        
        //DrawDefaultInspector();

        mapEditor.toolbarTab = GUILayout.Toolbar(mapEditor.toolbarTab, this.tabButtons, null, GUI.ToolbarButtonSize.FitToContents);


        mapEditor.currentTab = tabButtons[mapEditor.toolbarTab];

        /*
        switch (mapEditor.toolbarTab)
        {
            case 0:
                mapEditor.currentTab = "Automatic tools";
                break;

            case 1:
                mapEditor.currentTab = "Manual";
                break;

            case 2:
                mapEditor.currentTab = "Procedural tools";
                break;

            case 3:
                mapEditor.currentTab = "Map prefabs";
                break;
        }*/

        if (EditorGUI.EndChangeCheck())
        {
            soMapEditor.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }


        EditorGUI.BeginChangeCheck();

        

        switch (mapEditor.currentTab)
        {
            case "Automatic tools":
                EditorGUILayout.PropertyField(rows);
                EditorGUILayout.PropertyField(columns);

                GUILayout.Space(10f);

                if (GUILayout.Button("Generate map"))
                {
                    mapEditor.GenerateMap();
                }
                break;

            case "Manual tools":
                EditorGUILayout.PropertyField(selectedLayer);
                EditorGUILayout.PropertyField(brushState);
                EditorGUILayout.PropertyField(brushStyle);
                break;

            case "Procedural tools":
                EditorGUILayout.PropertyField(proceduralMap);
                EditorGUILayout.PropertyField(perlinNoiseScale);

                GUILayout.Space(10f);

                if (GUILayout.Button("Generate procedural map"))
                {
                    mapEditor.GenerateMap();
                }
                break;

            case "Map prefabs":
                EditorGUILayout.PropertyField(mapContainerPrefab);
                EditorGUILayout.PropertyField(tileToDraw);
                break;
        }

        if (EditorGUI.EndChangeCheck())
        {
            soMapEditor.ApplyModifiedProperties();
        }


        GUILayout.Space(40f);

        if (GUILayout.Button("Generate new map container"))
        {
            mapEditor.GenerateMap();
        }

        GUILayout.Space(20f);


        if (GUILayout.Button("Set game"))
        {
            mapEditor.SetGame();
        }
        if (GUILayout.Button("Optimize game"))
        {
            mapEditor.OptimizeGame();
        }
        if (GUILayout.Button("Deoptimize game"))
        {
            mapEditor.DeoptimizeGame();
        }


        GUILayout.Space(20f);
        if (GUILayout.Button("Create new map prefab"))
        {
            mapEditor.CreateNewMapPrefab();
        }

        if (GUILayout.Button("Save current map changes"))
        {
            mapEditor.SaveMapChanges();
        }

        

        GUILayout.Space(40f);

        if (GUILayout.Button("Delete current map"))
        {
            mapEditor.DeleteCurrentMap();
        }

        

        
    }
}
#endif