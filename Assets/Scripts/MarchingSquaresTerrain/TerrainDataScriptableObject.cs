using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "TerrainAsset/TerrainData", order = 1)]
public class TerrainDataScriptableObject : ScriptableObject
{
    public ChunkData[,] chunks;
}
