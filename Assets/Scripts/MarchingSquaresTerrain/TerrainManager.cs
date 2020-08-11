using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainManager : MonoBehaviour, IDamageable
{
    public static TerrainManager singleton;

    public TerrainDataScriptableObject data;
    public int chunkCount = 10;
    public int chunkSize = 31;
    public float range = 1.0f;
    public bool additive = true;
    public float meshScale = 0.1f;
    public GameObject chunkPrefab;
    public Texture2D noise;
    [Tooltip("How scale of detail of the noise is")]
    public float noiseScale = 1.0f;
    [Tooltip("How strong the noise is 0 = no noise")]
    public float noiseMultiple = 1.0f;
    public float circleMultiple = 1.0f;

    [Tooltip("Radius of circle around mouse cursor in which terrain is generated")]
    public float editModeViewRange = 50.0f;
    [Tooltip("Delete terrain chunks that are far from mouse cursor, speeds up playmode enter times as the editmode terrains don't have deleted all at once")]
    public bool editModeDeleteExtraTerrain = true;
    public float playModeViewRange = 30.0f;


    public Transform playModeTransformToFollow;

    [System.NonSerialized] private Dictionary<Vector2, Chunk> loadedChunks = new Dictionary<Vector2, Chunk>();
    [System.NonSerialized] private ChunkData[,] playModeData;


    // Start is called before the first frame update
    void OnEnable()
    {
        singleton = this;
        ClearChildren();
        if(Application.isPlaying)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            playModeData = new ChunkData[data.chunks.GetLength(0),data.chunks.GetLength(1)];
            for(int x = 0; x < data.chunks.GetLength(0); x++)
            {
                for (int y = 0; y < data.chunks.GetLength(1); y++)
                {
                    playModeData[x, y] = new ChunkData(data.chunks[x, y].densities);
                }
            }
            Debug.Log("Copying data took: " + timer.ElapsedMilliseconds + "ms");
        }
    }

    public void ClearChildren()
    {
        loadedChunks = new Dictionary<Vector2, Chunk>();
        var timer = System.Diagnostics.Stopwatch.StartNew();
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
        //Debug.Log("Clearing childern took: " + timer.ElapsedMilliseconds + "ms");
    }

    // Update is called once per frame
    void Update()
    {
        if(playModeTransformToFollow && Application.isPlaying)
        {
            LoadAroundPoint(playModeTransformToFollow.position, playModeViewRange);
        }
    }

    float ChunkEdgeLength()
    {
        return meshScale * (data.chunks[0, 0].densities.GetLength(0) - 1);
    }

    Vector3 chunkToPosition(int x, int y)
    {
        float scaleing = ChunkEdgeLength();
        return new Vector2(scaleing * x, scaleing * y);
    }

    (int,int) getChunkIdx(Vector3 position)
    {
        float scaleing = ChunkEdgeLength();
        return (Mathf.FloorToInt(position.x/ scaleing), Mathf.FloorToInt(position.y / scaleing));
    }

    public void ChangeDimentions()
    {
        // can only perserve data if dimention doesn't change
        var old_data = new ChunkData[0, 0];
        if (data.chunks.Length > 0 && data.chunks[0,0].densities.GetLength(0) == chunkSize && data.chunks[0, 0].densities.GetLength(0) == chunkSize)
        {
            old_data = data.chunks;
        }
        data.chunks = new ChunkData[chunkCount, chunkCount];
        for (int chunkX = 0; chunkX < data.chunks.GetLength(0); chunkX++)
        {
            for (int chunkY = 0; chunkY < data.chunks.GetLength(1); chunkY++)
            {
                if (old_data.GetLength(0) > chunkX && old_data.GetLength(1) > chunkY)
                {
                    data.chunks[chunkX, chunkY] = old_data[chunkX, chunkY];
                }
                else
                {
                    data.chunks[chunkX, chunkY] = new ChunkData(chunkSize, chunkSize);
                }
            }
        }
    }

    public void LoadAroundPoint(Vector2 position, float radius, bool deleteOthers = true)
    {
        if(data.chunks.Length == 0)
        {
            Debug.LogWarning("Terrain data is empty");
            return;
        }
        (int minX, int minY) = getChunkIdx(position-Vector2.one*radius);
        (int maxX, int maxY) = getChunkIdx(position+Vector2.one*radius);
        (int centerX, int centerY) = getChunkIdx(position);
        minX = Mathf.Max(0, minX-1);
        minY = Mathf.Max(0, minY-1);
        maxX = Mathf.Min(data.chunks.GetLength(0), maxX+1);
        maxY = Mathf.Min(data.chunks.GetLength(1), maxY+1);
        if (deleteOthers)
        {
            var toDelete = new List<Vector2>();
            foreach (var kvp in loadedChunks)
            {
                // delete chunks that are loaded but further away
                if (Vector2.Distance((Vector2)chunkToPosition(Mathf.RoundToInt(kvp.Key.x), Mathf.RoundToInt(kvp.Key.y)) + (Vector2.one * ChunkEdgeLength() * 0.5f), position) > radius)
                {
                    toDelete.Add(kvp.Key);
                }
            }
            foreach (var chunkPosition in toDelete)
            {
                if (Application.isPlaying)
                {
                    Destroy(loadedChunks[chunkPosition].gameObject);
                }
                else
                {
                    DestroyImmediate(loadedChunks[chunkPosition].gameObject);
                }
                loadedChunks.Remove(chunkPosition);
            }
        }
        for (int chunkX = minX; chunkX < maxX; chunkX++)
        {
            for (int chunkY = minY; chunkY < maxY; chunkY++)
            {
                if(Vector2.Distance((Vector2)chunkToPosition(chunkX,chunkY) + (Vector2.one * ChunkEdgeLength() * 0.5f), position) > radius)
                {
                    continue;
                }
                if (!loadedChunks.ContainsKey(new Vector2(chunkX, chunkY)))// chunk not loaded load, it
                {
                    var newChunk = Instantiate(chunkPrefab, chunkToPosition(chunkX, chunkY), Quaternion.identity, transform).GetComponent<Chunk>();
                    newChunk.gameObject.name = "Chunk" + chunkX + "_" + chunkY;
                    newChunk.edgeLength = meshScale;
                    if (Application.isPlaying)
                    {
                        newChunk.SetData(playModeData[chunkX, chunkY]);
                        //newChunk.SetData(new ChunkData(data.chunks[chunkX, chunkY].densities));// create copy so original isn't modified
                    }
                    else
                    {
                        newChunk.SetData(data.chunks[chunkX, chunkY]);
                    }
                    newChunk.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
                    loadedChunks[new Vector2(chunkX, chunkY)] = newChunk;
                }
            }
        }
    }

    public void ReloadChunks(int x=0, int y=0, int xCount=-1, int yCount=-1)
    {
        if(xCount == -1)
        {
            xCount = data.chunks.GetLength(0);
        }
        if (yCount == -1)
        {
            yCount = data.chunks.GetLength(y);
        }
        var timer = System.Diagnostics.Stopwatch.StartNew();
        ClearChildren();
        loadedChunks = new Dictionary<Vector2, Chunk>();
        for (int chunkX = 0; chunkX < data.chunks.GetLength(0); chunkX++)
        {
            for(int chunkY = 0; chunkY < data.chunks.GetLength(1); chunkY++)
            {
                var newChunk = Instantiate(chunkPrefab, chunkToPosition(chunkX, chunkY), Quaternion.identity, transform).GetComponent<Chunk>();
                newChunk.gameObject.name = "Chunk" + chunkX + "_" + chunkY;
                newChunk.edgeLength = meshScale;
                if (Application.isPlaying)
                {
                    newChunk.SetData(new ChunkData(data.chunks[chunkX, chunkY].densities));// create copy so original isn't modified
                }
                else
                {
                    newChunk.SetData(data.chunks[chunkX, chunkY]);
                }
                newChunk.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
                loadedChunks[new Vector2(chunkX, chunkY)] = newChunk;
            }
        }
        Debug.Log("ReloadingChunks took: "+ timer.ElapsedMilliseconds +" ms");
    }

    public void AddCircle(Vector2 position, float radius, bool removeInstead=false, float noiseMultiple = 0.0f, float noiseScale = 0.0f, float noiseOffset = 0.0f,float circleMultiple = 1.0f)
    {
        (int centerX, int centerY) = getChunkIdx(position);
        int extraBuffer = Mathf.FloorToInt(radius / ChunkEdgeLength());
        int minX = Mathf.Max(centerX - 1 - extraBuffer, 0);
        int minY = Mathf.Max(centerY - 1 - extraBuffer, 0);
        int maxX = Mathf.Min(centerX + 2 + extraBuffer, data.chunks.GetLength(0));
        int maxY = Mathf.Min(centerY + 2 + extraBuffer, data.chunks.GetLength(1));
        for(int x = minX; x < maxX; x++)
        {
            for(int y = minY; y < maxY; y++)
            {
                if (loadedChunks.ContainsKey(new Vector2(x, y)))
                {
                    if (noiseMultiple == 0)
                    {
                        loadedChunks[new Vector2(x, y)].AddCircle(position, radius, removeInstead);
                    }
                    else
                    {
                        loadedChunks[new Vector2(x, y)].ModifyRegion(position, radius * 1.5f, (Vector2 center, float previousValue) =>
                        {
                            var newDensity = circleMultiple * ((position - center).magnitude / radius - 1);
                            float noiseSum = 0f;
                            var noiseScales = new float[,]
                            {
                                {10.0f, 0.1f },// small details
                                {3.0f, 0.1f },
                                {1.0f, 0.2f },
                                {0.1f, 0.2f },// large details
                            };
                            for (int i = 0; i < noiseScales.GetLength(0); i++) {
                                noiseSum += (noise.GetPixel(
                                    Mathf.RoundToInt(center.x * noiseScales[i,0] * noiseScale + noiseOffset) % noise.width,
                                    Mathf.RoundToInt(center.y * noiseScales[i, 0] * noiseScale) % noise.height).grayscale 
                                    - 0.5f) * noiseMultiple * noiseScales[i,1];
                            }
                            if (removeInstead)
                            {
                                return Mathf.Min(newDensity+noiseSum, previousValue);
                            }
                            else
                            {
                                newDensity = newDensity * -1;
                                return Mathf.Max(newDensity+noiseSum, previousValue);
                            }
                        });
                    }
                }
                else
                {
                    //Debug.LogWarning("Trying to edit terrain that hasn't been loaded");
                }
            }
        }
    }

    public void Damage(float damageTaken, DamageType type, Vector2 point, Vector2 damageDirection, Vector2 surfaceNormal)
    {
        Dictionary<DamageType, float> radiusMultiples = new Dictionary<DamageType, float> {
            { DamageType.Explosive, 1f },
            { DamageType.Impact, 0.3f },
            { DamageType.Penetrating, 0.01f } };
        var dmgRadius = Mathf.Sqrt(damageTaken * radiusMultiples[type] * 0.01f);
        AddCircle(point, dmgRadius, true);
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
        loadedChunks = new Chunk[_sizeX, _sizeX == 0 ? 0 : _loadedChunks.Length / _sizeX];
        for (int i = 0; i < _loadedChunks.Length; i++)
        {
            loadedChunks[i / _sizeX, i % _sizeX] = _loadedChunks[i];
        }
        _loadedChunks = null;
    }

    public void OnBeforeSerialize()
    {
        if (loadedChunks == null)
        {
            loadedChunks = new Chunk[0, 0];
        }
        _sizeX = loadedChunks.GetLength(0);
        int totalCount = (loadedChunks.GetLength(0) * loadedChunks.GetLength(1));
        _loadedChunks = new Chunk[totalCount];
        for (int i = 0; i < totalCount; i++)
        {
            _loadedChunks[i] = loadedChunks[i / _sizeX, i % _sizeX];
        }
    }
}
