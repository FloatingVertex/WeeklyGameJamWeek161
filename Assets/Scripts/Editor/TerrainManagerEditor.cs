using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainManager))]
public class TerrainManagerEditor : Editor
{
    private SerializedProperty range;
    private SerializedProperty additive;
    private SerializedProperty chunkCount;
    private SerializedProperty chunkSize;
    private SerializedProperty meshScale;

    // Start is called before the first frame update
    private void OnEnable()
    {
        range = serializedObject.FindProperty("range");
        additive = serializedObject.FindProperty("additive");
        chunkCount = serializedObject.FindProperty("chunkCount");
        chunkSize = serializedObject.FindProperty("chunkSize");
        meshScale = serializedObject.FindProperty("meshScale");
    }

    private void OnSceneGUI()
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        Event e = Event.current;
        var mousePosition = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        var scenePortPosition = SceneView.currentDrawingSceneView.camera.transform.position;
        var terrainManager = ((TerrainManager)serializedObject.targetObject);

        if (Event.current.button == 0)
        {
            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                case EventType.MouseDrag:
                    GUIUtility.hotControl = controlID;
                    terrainManager.LoadAroundPoint(mousePosition.origin, terrainManager.editModeViewRange, terrainManager.editModeDeleteExtraTerrain);
                    terrainManager.AddCircle(mousePosition.origin, terrainManager.range, !terrainManager.additive,
                        noiseMultiple: terrainManager.noiseMultiple,
                        noiseScale: terrainManager.noiseScale,
                        circleMultiple: terrainManager.circleMultiple);
                    EditorUtility.SetDirty(terrainManager.data);
                    e.Use();
                    break;
                case EventType.MouseUp:
                    GUIUtility.hotControl = 0;
                    e.Use();
                    break;
                case EventType.MouseMove:
                    terrainManager.LoadAroundPoint(mousePosition.origin, terrainManager.editModeViewRange, terrainManager.editModeDeleteExtraTerrain);
                    break;
            }
        }
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        var terrainManager = ((TerrainManager)serializedObject.targetObject);
        
        GUILayout.Label("With this selected Click and Drag in the scene view to paint terrain. (Options below). Terrain is only generated around mouse in editor for performance reasons");

        EditorGUILayout.PropertyField(chunkCount, new GUIContent("chunkCount"));
        EditorGUILayout.PropertyField(chunkSize, new GUIContent("chunkSize"));
        if (GUILayout.Button("Regenerate Data (Potentially Clears Current Data!!!!!)"))
        {
            terrainManager.ChangeDimentions();
            EditorUtility.SetDirty(terrainManager.data);
            terrainManager.ClearChildren();
        }
        EditorGUILayout.PropertyField(meshScale, new GUIContent("meshScale"));
        if (GUILayout.Button("Generate Fully"))
        {
            terrainManager.ReloadChunks();
        }
        if (GUILayout.Button("Clear"))
        {
            terrainManager.ClearChildren();
        }
        EditorGUILayout.Slider(range, 0.01f, 15f, new GUIContent("Range"));

        string[] propertyNames = new string[] { "additive", "chunkPrefab","data","noise","noiseScale", "noiseMultiple", "circleMultiple", "editModeViewRange", "editModeDeleteExtraTerrain", "playModeViewRange","playModeTransformToFollow" };

        foreach (var name in propertyNames)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(name), new GUIContent(name));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
