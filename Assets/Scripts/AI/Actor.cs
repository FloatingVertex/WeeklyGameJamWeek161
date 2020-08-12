using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public static Vector2 lastKnownPlayerLocation;
    public static float timeLastSpoted;

    public Affiliation affiliation = Affiliation.Enemy;

    public List<Actor> spotting = new List<Actor>();
}

public enum Affiliation
{
    Player,
    Enemy
}
