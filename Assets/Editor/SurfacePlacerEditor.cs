using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SurfacePlacer))]
public class SurfacePlacerEditor : Editor
{
    private SerializedProperty maxReach;
    private SerializedProperty prefab;
    private SerializedProperty alignTo;
    private SerializedProperty offsetAlongNormal;
    private SerializedProperty parent;
    private SerializedProperty mask;
    private SerializedProperty placeOnEdge;

    // Start is called before the first frame update
    private void OnEnable()
    {
        maxReach = serializedObject.FindProperty("maxReach");
        prefab = serializedObject.FindProperty("prefab");
        alignTo = serializedObject.FindProperty("alignTo");
        offsetAlongNormal = serializedObject.FindProperty("offsetAlongNormal");
        parent = serializedObject.FindProperty("parent");
        mask = serializedObject.FindProperty("mask");
        placeOnEdge = serializedObject.FindProperty("placeOnEdge");
    }

    private void OnSceneGUI()
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        Event e = Event.current;
        var mousePosition = (Vector2)HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
        var placer = ((SurfacePlacer)serializedObject.targetObject);

        switch (e.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                GUIUtility.hotControl = controlID;
                var candidates = Physics2D.OverlapCircleAll(mousePosition, placer.maxReach, placer.mask);
                if (candidates.Length > 0 && placer.placeOnEdge) {
                    Collider2D bestCollider = candidates[0];
                    float bestDistance = placer.maxReach;
                    foreach (var collider in candidates)
                    {
                        if (!collider.isTrigger && Vector2.Distance(mousePosition,collider.ClosestPoint(mousePosition)) < bestDistance)
                        {
                            bestDistance = Vector2.Distance(mousePosition, collider.ClosestPoint(mousePosition));
                            bestCollider = collider;
                        }
                    }

                    GameObject newObj = PrefabUtility.InstantiatePrefab(placer.prefab) as GameObject;
                    newObj.transform.position = bestCollider.ClosestPoint(mousePosition);
                    newObj.transform.parent = placer.parent ? placer.parent : placer.transform;

                    if (placer.alignTo == PlacementAllignTo.PointToMouse)
                    {
                        Utility.RotateTowardsTarget(newObj.transform, mousePosition);
                    }
                }
                else if(!placer.placeOnEdge)
                {
                    GameObject newObj = PrefabUtility.InstantiatePrefab(placer.prefab) as GameObject;
                    newObj.transform.position = mousePosition;
                    newObj.transform.parent = placer.parent ? placer.parent : placer.transform;
                }
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
        GUILayout.Label("With this selected click in the scene view to place prefabs on the learest collider surface, below are the options");
        EditorGUILayout.PropertyField(prefab, new GUIContent("prefab"));
        EditorGUILayout.PropertyField(parent, new GUIContent("parent"));
        EditorGUILayout.PropertyField(maxReach , new GUIContent("maxReach"));
        EditorGUILayout.PropertyField(alignTo, new GUIContent("alignTo"));
        EditorGUILayout.PropertyField(offsetAlongNormal , new GUIContent("offsetAlongNormal"));
        EditorGUILayout.PropertyField(mask, new GUIContent("mask"));
        EditorGUILayout.PropertyField(placeOnEdge, new GUIContent("placeOnEdge"));
        serializedObject.ApplyModifiedProperties();
    }
}
