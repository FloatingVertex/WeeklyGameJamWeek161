using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager singleton;
    public GlobalConfigurations config;
    public string[] availableWeapons;
    public string levelObjectiveString = "Bunkers destroyed";
    private IObjective[] objectives;

    void Awake()
    {
        if(singleton != null && singleton != this)
        {
            Debug.LogError("Multiple Level Managers");
        }
        singleton = this;
    }

    private void Start()
    {
        objectives = GetComponentsInChildren<IObjective>();
    }

    // done/total
    public (int,int) GetObjectivesInfo()
    {
        int done = 0;
        foreach(var objective in objectives)
        {
            if (objective.IsComplete()) done++;
        }
        return (done, objectives.Length);
    }
}
