using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
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
    private ChunksSquare loadedChunks = new ChunksSquare();

    // Start is called before the first frame update
    void Start()
    {
        ReloadChunks();
    }

    private void Awake()
    {
        //ReloadChunks();
    }

    private void OnEnable()
    {
        ReloadChunks();
    }

    private void OnDisable()
    {
    }

    private void ClearChildren()
    {
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in transform)
        {
            toDestroy.Add(child.gameObject);
        }
        foreach (var target in toDestroy)
        {
            if (Application.isPlaying)
            {
                Destroy(target);
            }
            else
            {
                DestroyImmediate(target);
            }
        }
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
        Debug.Log("ReloadingChunks");
        ClearChildren();
        loadedChunks.loadedChunks = new Chunk[data.chunks.GetLength(0), data.chunks.GetLength(1)];
        for (int chunkX = 0; chunkX < data.chunks.GetLength(0); chunkX++)
        {
            for(int chunkY = 0; chunkY < data.chunks.GetLength(1); chunkY++)
            {
                var newChunk = Instantiate(chunkPrefab, chunkToPosition(chunkX, chunkY), Quaternion.identity, transform).GetComponent<Chunk>();
                newChunk.gameObject.name = "Chunk" + chunkX + "_" + chunkY;
                newChunk.edgeLength = meshScale;
                if (Application.isPlaying)
                {
                    newChunk.SetData(new ChunkData(data.chunks[chunkX, chunkY].densities));
                }
                else
                {
                    newChunk.SetData(data.chunks[chunkX, chunkY]);
                }
                newChunk.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
                loadedChunks.loadedChunks[chunkX, chunkY] = newChunk;
            }
        }
    }

    public void AddCircle(Vector2 position, float radius, bool removeInstead=false)
    {
        if(loadedChunks == null || loadedChunks.loadedChunks.GetLength(0) != data.chunks.GetLength(0) || loadedChunks.loadedChunks.GetLength(0) != data.chunks.GetLength(0))
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
                if (loadedChunks.loadedChunks[x, y] != null)
                {
                    loadedChunks.loadedChunks[x, y].AddCircle(position, radius, removeInstead);
                    loadedChunks.loadedChunks[x, y].GenerateNewMesh();
                }
                else
                {
                    Debug.LogWarning("Trying to edit terrain that hasn't been loaded");
                }
            }
        }
    }
}

[System.Serializable]
public class ChunksSquare : ISerializationCallbackReceiver
{
    [SerializeField] private Chunk[] _loadedChunks;
    [SerializeField] private int _sizeX;
    [System.NonSerialized]public Chunk[,] loadedChunks;
    public void OnAfterDeserialize()
    {
        if (_loadedChunks == null)
        {
            _loadedChunks = new Chunk[0];
        }
        loadedChunks = new Chunk[_sizeX, _loadedChunks.Length / _sizeX];
        for (int i = 0; i < _loadedChunks.Length; i++)
        {
            loadedChunks[i / _sizeX, i % _sizeX] = _loadedChunks[i];
        }
        _loadedChunks = null;
    }

    public void OnBeforeSerialize()
    {
        _sizeX = loadedChunks.GetLength(0);
        int totalCount = (loadedChunks.GetLength(0) * loadedChunks.GetLength(1));
        _loadedChunks = new Chunk[totalCount];
        for (int i = 0; i < totalCount; i++)
        {
            _loadedChunks[i] = loadedChunks[i / _sizeX, i % _sizeX];
        }
    }
}
