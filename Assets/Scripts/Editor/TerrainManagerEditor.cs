using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainManager))]
public class TerrainManagerEditor : Editor
{
    private SerializedObject obj;
    private SerializedProperty data;
    private SerializedProperty range;
    private SerializedProperty additive;
    private SerializedProperty chunkCount;
    private SerializedProperty chunkSize;
    private SerializedProperty chunkPrefab;

    // Start is called before the first frame update
    private void OnEnable()
    {
        obj = new SerializedObject(target);
        data = obj.FindProperty("data");
        range = obj.FindProperty("range");
        additive = obj.FindProperty("additive");
        chunkCount = obj.FindProperty("chunkCount");
        chunkSize = obj.FindProperty("chunkSize");
        chunkPrefab = obj.FindProperty("chunkPrefab");

        Debug.Log("OnEnable event called");
        //SceneView.onSceneGUIDelegate += _OnSceneEvents;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("OnDisable event called");
        //SceneView.onSceneGUIDelegate -= _OnSceneEvents;
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
                ((TerrainManager)obj.targetObject).AddCircle(mousePosition.origin, ((TerrainManager)obj.targetObject).range);
                e.Use();
                obj.ApplyModifiedProperties();
                break;
            case EventType.MouseUp:
                GUIUtility.hotControl = 0;
                e.Use();
                break;
        }
    }

    override public void OnInspectorGUI()
    {
        GUI.enabled = true;
        EditorGUILayout.PropertyField(chunkCount, new GUIContent("chunkCount"));
        EditorGUILayout.PropertyField(chunkSize, new GUIContent("chunkSize"));
        if (GUILayout.Button("Regenerate (Clears Current Data)"))
        {
            List<GameObject> toDestroy = new List<GameObject>();
            foreach(Transform child in ((TerrainManager)obj.targetObject).transform)
            {
                toDestroy.Add(child.gameObject);
            }
            foreach(var go in toDestroy)
            {
                DestroyImmediate(go);
            }
            ((TerrainManager)obj.targetObject).ResetData();
            ((TerrainManager)obj.targetObject).ReloadChunks();
        }
        EditorGUILayout.Slider(range, 0.01f, 10f, new GUIContent("Range"));
        EditorGUILayout.PropertyField(additive, new GUIContent("Additive"));
        EditorGUILayout.PropertyField(chunkPrefab, new GUIContent("chunkPrefab"));
        EditorGUILayout.PropertyField(data, new GUIContent("Data"));

        obj.ApplyModifiedProperties();
    }
}
