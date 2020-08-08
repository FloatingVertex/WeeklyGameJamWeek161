using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "TerrainAsset/TerrainData", order = 1)]
public class TerrainDataScriptableObject : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private ChunkData[] _ChunkDatas;
    [SerializeField] private int _sizeX;
    [System.NonSerialized] public ChunkData[,] chunks;

    public void OnAfterDeserialize()
    {
        if(_ChunkDatas == null)
        {
            _ChunkDatas = new ChunkData[0];
        }
        chunks = new ChunkData[_sizeX, _sizeX == 0 ? 0 : _ChunkDatas.Length / _sizeX];
        for (int i = 0; i < _ChunkDatas.Length; i++)
        {
            chunks[i / _sizeX, i % _sizeX] = _ChunkDatas[i];
        }
    }

    public void OnBeforeSerialize()
    {
        if (chunks == null)
        {
            chunks = new ChunkData[0, 0];
        }
        _sizeX = chunks.GetLength(0);
        int totalCount = (chunks.GetLength(0) * chunks.GetLength(1));
        _ChunkDatas = new ChunkData[totalCount];
        for (int i = 0; i < (chunks.GetLength(0) * chunks.GetLength(1)); i++)
        {
            _ChunkDatas[i] = chunks[i / _sizeX, i % _sizeX];
        }
    }
}
