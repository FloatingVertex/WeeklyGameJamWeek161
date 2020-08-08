using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainManager))]
public class TerrainManagerEditor : Editor
{
    private SerializedProperty data;
    private SerializedProperty range;
    private SerializedProperty additive;
    private SerializedProperty chunkCount;
    private SerializedProperty chunkSize;
    private SerializedProperty chunkPrefab;

    // Start is called before the first frame update
    private void OnEnable()
    {
        data = serializedObject.FindProperty("data");
        range = serializedObject.FindProperty("range");
        additive = serializedObject.FindProperty("additive");
        chunkCount = serializedObject.FindProperty("chunkCount");
        chunkSize = serializedObject.FindProperty("chunkSize");
        chunkPrefab = serializedObject.FindProperty("chunkPrefab");
    }

    private void OnSceneGUI()
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        Event e = Event.current;
        var mousePosition = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        switch (e.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
            case EventType.MouseDrag:
                GUIUtility.hotControl = controlID;
                var terrainManager = ((TerrainManager)serializedObject.targetObject);
                terrainManager.AddCircle(mousePosition.origin, terrainManager.range, !terrainManager.additive);
                EditorUtility.SetDirty(terrainManager.data);
                e.Use();
                break;
            case EventType.MouseUp:
                GUIUtility.hotControl = 0;
                e.Use();
                break;
        }
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        var terrainManager = ((TerrainManager)serializedObject.targetObject);

        Debug.Log("terrainManager.data is dirty? " + EditorUtility.IsDirty(terrainManager.data));
        Debug.Log("terrainManager is dirty? " + EditorUtility.IsDirty(terrainManager));
        GUI.enabled = true;
        EditorGUILayout.PropertyField(chunkCount, new GUIContent("chunkCount"));
        EditorGUILayout.PropertyField(chunkSize, new GUIContent("chunkSize"));
        if (GUILayout.Button("Regenerate (Clears Current Data)"))
        {
            terrainManager.ResetData();
            EditorUtility.SetDirty(terrainManager.data);
            terrainManager.ReloadChunks();
        }
        if (GUILayout.Button("Generate"))
        {
            terrainManager.ReloadChunks();
        }
        EditorGUILayout.Slider(range, 0.01f, 10f, new GUIContent("Range"));
        EditorGUILayout.PropertyField(additive, new GUIContent("Additive"));
        EditorGUILayout.PropertyField(chunkPrefab, new GUIContent("chunkPrefab"));
        EditorGUILayout.PropertyField(data, new GUIContent("Data"));

        serializedObject.ApplyModifiedProperties();
    }
}
