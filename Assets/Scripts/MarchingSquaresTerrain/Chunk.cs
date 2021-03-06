﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class Chunk : MonoBehaviour 
{
    [System.NonSerialized]
    public ChunkData data;
    public float edgeLength = 0.1f;
    public float uvScale = 4f;

    private bool needsRebuild = false;

    public bool needToRecalcNavGrid = false;

    public void SetData(ChunkData newData)
    {
        data = newData;
        GenerateNewMesh();
    }

    public void AddCircle(Vector2 center, float radius, bool removeInstead = false)
    {
        ModifyRegion(center, radius, (Vector2 position,float previousValue) =>
        {
            if (removeInstead)
            {
                var newDensity = (position - center).magnitude / radius - 1;
                return Mathf.Min(newDensity, previousValue);
            }
            else
            {
                var newDensity = radius / (position - center).magnitude - 1;
                return Mathf.Max(newDensity, previousValue);
            }
        });
    }

    private void Update()
    {
        if (needsRebuild)
        {
            this.GenerateNewMeshInternal();
            needsRebuild = false;
        }
    }

    public void GenerateNewMesh()
    {
        if (Application.isPlaying)
        {
            needsRebuild = true;
        }
        else
        {
            GenerateNewMeshInternal();
        }
    }

    private IEnumerator DeleteAfterFrame(Transform[] objects)
    {
        yield return new WaitForEndOfFrame();
        foreach(var obj in objects)
        {
            if(obj != transform)
            {
                if (obj != null)// TODO: findout why this happens? when shadows get updated two frames in a row?
                {
                    Utility.Destroy(obj.gameObject);
                }
            }
        }
    }

    private void GenerateNewMeshInternal()
    {
        SetNewMeshInternal(data.GenerateMesh(edgeLength, uvScale));
    }

    private void SetNewMeshInternal(MeshData newData)
    {
        var mesh = new Mesh();
        mesh.vertices = newData.verts;
        mesh.uv = newData.uvs;
        mesh.triangles = newData.triangles;
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        var timer = System.Diagnostics.Stopwatch.StartNew();
        PolygonCollider2D polygonCollider2D = GetComponent<PolygonCollider2D>();
        polygonCollider2D.pathCount = newData.paths.Count;
        StartCoroutine(DeleteAfterFrame(gameObject.GetComponentsInChildren<Transform>()));
        for (int i = 0; i < newData.paths.Count; i++)
        {
            polygonCollider2D.SetPath(i, newData.paths[i]);
            ShadowCaster.CreateShadowCaster( i, transform );
        }

        // update AStar map
        if (GridAStar.singleton)// && needToRecalcNavGrid)
        {
            GridAStar.singleton.RegenerateSection(transform.position, (Vector2)transform.position + new Vector2(data.densities.GetLength(0) * edgeLength, data.densities.GetLength(1) * edgeLength));
        }
    }

    public void ModifyRegion(Vector2 point, float radius, Func<Vector2, float, float> newValueFunction)
    {
        var minPoint = point - new Vector2(radius, radius);
        var maxPoint = point + new Vector2(radius, radius);
        Vector2 minCorner = transform.position;
        Vector2 maxCorner = new Vector2(transform.position.x + ((data.densities.GetLength(0) - 1) * edgeLength), transform.position.y + ((data.densities.GetLength(1) - 1) * edgeLength));
        if(minPoint.x > maxCorner.x || minPoint.y > maxCorner.y || maxPoint.x < minCorner.x || maxPoint.y < minCorner.y)
        {
            return; // nothing needs to be changed
        }
        minPoint.x = Mathf.Max(transform.position.x, minPoint.x);
        minPoint.y = Mathf.Max(transform.position.y, minPoint.y);
        maxPoint.x = Mathf.Min(transform.position.x + ((data.densities.GetLength(0) - 1) * edgeLength), maxPoint.x);
        maxPoint.y = Mathf.Min(transform.position.y + ((data.densities.GetLength(1) - 1) * edgeLength), maxPoint.y);
        data.Map((int xInt, int yInt, float previousValue) => {
            Vector2 worldPosition = new Vector2(xInt * edgeLength,yInt * edgeLength) + (Vector2)transform.position;
            return newValueFunction(worldPosition, previousValue);
        },
            Mathf.FloorToInt((minPoint.x - transform.position.x) / edgeLength),
            Mathf.FloorToInt((minPoint.y - transform.position.y) / edgeLength),
            Mathf.CeilToInt((maxPoint.x - transform.position.x) / edgeLength),
            Mathf.CeilToInt((maxPoint.y - transform.position.y) / edgeLength));
        GenerateNewMesh();
    }
}

[System.Serializable]
public class ChunkData : ISerializationCallbackReceiver
{
    [SerializeField] private float[] _densities;
    [SerializeField] private int _sizeX;

    [System.NonSerialized] public float[,] densities; // positives = terrain, negative = air

    public ChunkData(int sizeX,int sizeY)
    {
        densities = new float[sizeX,sizeY];
        Map((x, y, previous) => { return 10f; });
    }

    public ChunkData(float[,] densities)
    {
        this.densities = new float[densities.GetLength(0), densities.GetLength(1)];
        Map((x, y, previous) => { return densities[x,y]; });
    }

    public void Map(System.Func<int,int,float,float> lamda, int minX = 0, int minY = 0, int maxX = -1, int maxY = -1)
    {
        if(maxX == -1)
        {
            maxX = densities.GetLength(0) - 1;
        }
        if (maxY == -1)
        {
            maxY = densities.GetLength(1) - 1;
        }
        for (int x = minX; x <= maxX; x++)
        {
            for(int y = minY; y <= maxY; y++)
            {
                densities[x, y] = lamda(x, y, densities[x, y]);
            }
            
        }
    }

    private static readonly int[,] corners = new int[,]
    {
        {0,0 },
        {1,0 },
        {0,1 },
        {1,1 }
    };

    private static readonly int[,] edgeIndexToCornerIndexs = new int[,]// in order (0.5,0),(0,0.5),(0.5,1),(1,0.5)
    {
        {0,1},
        {0,2 },
        {2,3 },
        {1,3 }
    };

    private static readonly bool[,] edgeIntersections = new bool[,]// in order (0.5,0),(0,0.5),(0.5,1),(1,0.5)
                    {
                        {false,false,false,false },//0b0000
                        {false,false,true,true },  //0b0001
                        {false,true,true,false },  //0b0010
                        {false,true,false,true },  //0b0011
                        {true,false,false,true },  //0b0100
                        {true,false,true,false },  //0b0101
                        {true,true,true,true },    //0b0110
                        {true,true,false,false },  //0b0111
                        {true,true,false,false },  //0b1000
                        {true,true,true,true },    //0b1001
                        {true,false,true,false },  //0b1010
                        {true,false,false,true },  //0b1011
                        {false,true,false,true },  //0b1100
                        {false,true,true,false },  //0b1101
                        {false,false,true,true },  //0b1110
                        {false,false,false,false },//0b1111
                    };

    private static readonly int[,] triangleConnections = new int[,]// in order (0.5,0),(0,0.5),(0.5,1),(1,0.5)
                    {
                        {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},//0b0000
                        { 0, 2, 1,-1,-1,-1,-1,-1,-1,-1,-1,-1},//0b0001
                        { 0, 2, 1,-1,-1,-1,-1,-1,-1,-1,-1,-1},//0b0010
                        { 0, 1, 2, 2, 1, 3,-1,-1,-1,-1,-1,-1},//0b0011
                        { 0, 1, 2,-1,-1,-1,-1,-1,-1,-1,-1,-1},//0b0100
                        { 0, 2, 1, 1, 2, 3,-1,-1,-1,-1,-1,-1},//0b0101
                        { 0, 2, 5, 1, 4, 3,-1,-1,-1,-1,-1,-1},//0b0110 ambigious case
                        { 0, 1, 2, 0, 3, 1, 1, 3, 4,-1,-1,-1},//0b0111
                        { 0, 2, 1,-1,-1,-1,-1,-1,-1,-1,-1,-1},//0b1000
                        { 0, 3, 2, 1, 5, 4,-1,-1,-1,-1,-1,-1},//0b1001 ambigious case
                        { 0, 1, 2, 1, 3, 2,-1,-1,-1,-1,-1,-1},//0b1010
                        { 0, 1, 2, 0, 2, 3, 3, 2, 4,-1,-1,-1},//0b1011
                        { 0, 2, 1, 2, 3, 1,-1,-1,-1,-1,-1,-1},//0b1100
                        { 0, 2, 1, 0, 3, 2, 3, 4, 2,-1,-1,-1},//0b1101
                        { 0, 2, 1, 2, 3, 1, 3, 4, 1,-1,-1,-1},//0b1110
                        {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},//0b1111
                    };



    public MeshData GenerateMesh(float edgeLength = 0.1f, float uvScale = 1)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        MeshData returnVal = new MeshData();

        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var meshEdgeEdgesDict = new Dictionary<Vector2, Vector2>();

        Action<int,int,int> addRect = (int x,int yStart,int yEnd) => {
            int startingIndex = vertices.Count;
            vertices.Add(new Vector3(x, yStart));
            vertices.Add(new Vector3((x + 1), yStart));
            vertices.Add(new Vector3(x, yEnd));
            vertices.Add(new Vector3((x + 1), yEnd));
            
            triangles.Add(startingIndex);
            triangles.Add(startingIndex + 2);
            triangles.Add(startingIndex + 1);
            
            triangles.Add(startingIndex + 2);
            triangles.Add(startingIndex + 3);
            triangles.Add(startingIndex + 1);
            
            if(yStart == 0)
            {
                meshEdgeEdgesDict[new Vector2((x + 1), yStart)] = new Vector2(x, yStart);
            }
            if(yEnd == densities.GetLength(1) - 1)
            {
                meshEdgeEdgesDict[new Vector2(x, yEnd)] = new Vector2((x + 1), yEnd);
            }
            if(x == 0)
            {
                meshEdgeEdgesDict[new Vector2(x, yStart)] = new Vector2(x, yEnd);
            }
            if (x == densities.GetLength(0) - 2)
            {
                meshEdgeEdgesDict[new Vector2((x+1), yEnd)] = new Vector2((x+1), yStart);
            }
        };
        int dimention0 = densities.GetLength(0);
        int dimention1 = densities.GetLength(1);
        for (int x = 0; x < dimention0 - 1; x++)
        {
            int firstSolid = -1;
            for (int y = 0; y < dimention1 - 1; y++)
            {
                int mask = 0;
                for(int i = 0; i < 4; i++)
                {
                    if (densities[x+corners[i, 0], y+corners[i, 1]] > 0) mask |= 1 << (3-i);
                }

                bool isSolid = mask == 0b1111;
                bool isEmpty = mask == 0;
                if (isSolid && firstSolid == -1)
                {
                    firstSolid = y;
                }
                if (!isSolid && firstSolid != -1) {
                    addRect(x, firstSolid, y);
                    firstSolid = -1;
                }
                if (!isSolid && !isEmpty)
                {
                    int startingIndex = vertices.Count;
                    // ground edge
                    for(int i = 0; i < 4; i++)
                    {
                        if (densities[x+corners[i, 0],y+ corners[i, 1]] > 0)
                        {
                            vertices.Add(new Vector3((x+corners[i, 0]), (y+corners[i, 1])));
                        }
                    }
                    int startingIntersectionVertIndex = vertices.Count;

                    // in order (0.5,0),(0,0.5),(0.5,1),(1,0.5)
                    for(int i = 0; i < 4; i++)
                    {
                        if (edgeIntersections[mask, i])
                        {
                            int corner1 = edgeIndexToCornerIndexs[i, 0];
                            int corner2 = edgeIndexToCornerIndexs[i, 1];
                            var lerpWeight = LerpWeight(densities[(x + corners[corner1, 0]), (y + corners[corner1, 1])], densities[(x + corners[corner2, 0]), (y + corners[corner2, 1])]);
                            var vertexPosition = Vector3.Lerp(
                                new Vector3((x + corners[corner1, 0]), (y + corners[corner1, 1])),
                                new Vector3((x + corners[corner2, 0]), (y + corners[corner2, 1])),
                                lerpWeight);
                            vertices.Add(vertexPosition);
                        }
                    }
                    
                    for(int i = 0; i < 4; i++)
                    {
                        int idst = (i * 3);
                        if (triangleConnections[mask, idst] == -1)
                        {
                            break;
                        }
                        if (Mathf.Max(triangleConnections[mask, idst], triangleConnections[mask, idst + 1], triangleConnections[mask, idst + 2]) >= vertices.Count - startingIndex)
                        {
                            throw new System.Exception("Out of bounds");
                        }
                        triangles.Add(startingIndex + triangleConnections[mask, idst]);
                        triangles.Add(startingIndex + triangleConnections[mask, idst + 1]);
                        triangles.Add(startingIndex + triangleConnections[mask, idst + 2]);
                        for(int trixEdgeId = 0; trixEdgeId < 3; trixEdgeId++)
                        {
                            if(triangleConnections[mask, idst + trixEdgeId] >= (startingIntersectionVertIndex-startingIndex) 
                                && triangleConnections[mask, idst + ((trixEdgeId + 1) %3)] >= (startingIntersectionVertIndex - startingIndex))
                            {
                                var key = (Vector2)vertices[startingIndex + triangleConnections[mask, idst + trixEdgeId]];
                                var value = (Vector2)vertices[startingIndex + triangleConnections[mask, idst + ((trixEdgeId + 1) % 3)]];
                                if (key.GetHashCode() != value.GetHashCode() || key != value)
                                {
                                    meshEdgeEdgesDict[key] = value;
                                }
                            }
                        }
                        if (x == 0 || y == 0 || x == (densities.GetLength(0) - 2) || y == (densities.GetLength(1) - 2))
                        {
                            for (int trixEdgeId = 0; trixEdgeId < 3; trixEdgeId++)
                            {
                                var key = (Vector2)vertices[startingIndex + triangleConnections[mask, idst + trixEdgeId]];
                                var value = (Vector2)vertices[startingIndex + triangleConnections[mask, idst + ((trixEdgeId + 1) % 3)]];
                                if(key.x == value.x && (key.x == 0 || key.x == (densities.GetLength(0) - 1)))
                                {
                                    if (key.GetHashCode() != value.GetHashCode() || key != value)
                                    {
                                        meshEdgeEdgesDict[key] = value;
                                    }
                                }
                                if(key.y == value.y && (key.y == 0 || key.y == (densities.GetLength(0) - 1)))
                                {
                                    if (key.GetHashCode() != value.GetHashCode() || key != value)
                                    {
                                        meshEdgeEdgesDict[key] = value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (firstSolid != -1)
            {
                addRect(x, firstSolid, densities.GetLength(1) - 1);
            }
        }

        returnVal.uvs = new Vector2[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = vertices[i]*edgeLength;
            returnVal.uvs[i] = vertices[i] / uvScale;
        }

        returnVal.verts = vertices.ToArray();
        returnVal.triangles = triangles.ToArray();

        returnVal.paths = new List<List<Vector2>>();
        while(meshEdgeEdgesDict.Count > 0)
        {
            var path = new List<Vector2>();
            var enumerator = meshEdgeEdgesDict.Keys.GetEnumerator();
            enumerator.MoveNext();
            var current = enumerator.Current;
            Vector2 next = Vector2.zero;
            while (meshEdgeEdgesDict.TryGetValue(current, out next))
            {
                path.Add(current * edgeLength);
                meshEdgeEdgesDict.Remove(current);
                current = next;
            }
            //path.Add(current * edgeLength);
            if (path.Count > 1)
            {
                returnVal.paths.Add(path);
            }
        }

        //Debug.Log("Generation took: "+ timer.ElapsedMilliseconds +"ms");

        return returnVal;
    }

    public Vector3 posToVector3(int x, int y, float edgeLength)
    {
        return new Vector3(x * edgeLength, y * edgeLength);
    }

    float LerpWeight(float weight1, float weight2)
    {
        return Mathf.Abs(weight1) / (Mathf.Abs(weight1) + Mathf.Abs(weight2));
    }

    Vector3 ToVertexPosition(int x, int y, float edgeLength)
    {
        return new Vector3(x * edgeLength, y * edgeLength);
    }

    public void OnAfterDeserialize()
    {
        if (_densities == null)
        {
            _densities = new float[0];
        }
        densities = new float[_sizeX, _sizeX == 0 ? 0 : _densities.Length / _sizeX];
        for (int i = 0; i < _densities.Length; i++)
        {
            densities[i / _sizeX, i % _sizeX] = _densities[i];
        }
    }

    public void OnBeforeSerialize()
    {
        _sizeX = densities.GetLength(0);
        int totalCount = (densities.GetLength(0) * densities.GetLength(1));
        _densities = new float[totalCount];
        for (int i = 0; i < (densities.GetLength(0) * densities.GetLength(1)); i++)
        {
            _densities[i] = densities[i / _sizeX, i % _sizeX];
        }
    }
}

public struct MeshData
{
    public Vector3[] verts;
    public Vector2[] uvs;
    public int[] triangles;
    public List<List<Vector2>> paths;
}