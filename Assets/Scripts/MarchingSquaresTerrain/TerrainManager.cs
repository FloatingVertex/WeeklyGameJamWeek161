using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public TerrainDataScriptableObject data;
    public int chunkCount = 10;
    public int chunkSize = 31;
    public float range = 1.0f;
    public bool additive = true;
    public float meshScale = 0.1f;
    public GameObject chunkPrefab;

    [SerializeField]
    private Chunk[,] loadedChunks;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    Vector3 chunkToPosition(int x, int y)
    {
        float scaleing = meshScale * (data.chunks[0, 0].densities.GetLength(0) - 1);
        return new Vector2(scaleing * x, scaleing * y);
    }

    (int,int) getChunkIdx(Vector3 position)
    {
        float scaleing = meshScale * (data.chunks[0, 0].densities.GetLength(0) - 1);
        return (Mathf.FloorToInt(position.x/ scaleing), Mathf.FloorToInt(position.y / scaleing));
    }

    public void ResetData()
    {
        data.chunks = new ChunkData[chunkCount, chunkCount];
        for (int chunkX = 0; chunkX < data.chunks.GetLength(0); chunkX++)
        {
            for (int chunkY = 0; chunkY < data.chunks.GetLength(1); chunkY++)
            {
                data.chunks[chunkX, chunkY] = new ChunkData(chunkSize, chunkSize);
            }
        }
    }

    public void ReloadChunks()
    {
        loadedChunks = new Chunk[data.chunks.GetLength(0), data.chunks.GetLength(1)];
        for (int chunkX = 0; chunkX < data.chunks.GetLength(0); chunkX++)
        {
            for(int chunkY = 0; chunkY < data.chunks.GetLength(1); chunkY++)
            {
                if(loadedChunks[chunkX,chunkY] == null)
                {
                    var newChunk = Instantiate(chunkPrefab, chunkToPosition(chunkX, chunkY), Quaternion.identity, transform).GetComponent<Chunk>();
                    newChunk.gameObject.name = "Chunk" + chunkX + "_" + chunkY;
                    newChunk.edgeLength = meshScale;
                    newChunk.SetData(data.chunks[chunkX,chunkY]);
                    loadedChunks[chunkX, chunkY] = newChunk;
                }
                else
                {
                    var chunk = loadedChunks[chunkX, chunkY];
                    chunk.edgeLength = meshScale;
                    chunk.transform.position = chunkToPosition(chunkX, chunkY);
                    chunk.SetData(data.chunks[chunkX, chunkY]);
                }
            }
        }
    }

    public void AddCircle(Vector2 position, float radius)
    {
        if(loadedChunks == null || loadedChunks.GetLength(0) != data.chunks.GetLength(0) || loadedChunks.GetLength(0) != data.chunks.GetLength(0))
        {
            Debug.LogWarning("Trying to edit terrain that hasn't been loaded");
            return;
        }
        (int centerX, int centerY) = getChunkIdx(position);
        int minX = Mathf.Max(centerX - 1, 0);
        int minY = Mathf.Max(centerY - 1, 0);
        int maxX = Mathf.Min(centerX + 2, data.chunks.GetLength(0));
        int maxY = Mathf.Min(centerY + 2, data.chunks.GetLength(1));
        for(int x = minX; x < maxX; x++)
        {
            for(int y = minY; y < maxY; y++)
            {
                if (loadedChunks[x, y] != null)
                {
                    loadedChunks[x, y].AddCircle(position, radius);
                    loadedChunks[x, y].GenerateNewMesh();
                }
                else
                {
                    Debug.LogWarning("Trying to edit terrain that hasn't been loaded");
                }
            }
        }
    }
}
