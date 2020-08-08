using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class Chunk : MonoBehaviour
{
    public ChunkData data;

    public GameObject dotPrefab;

    void Start()
    {
        data = new ChunkData(101, 101);
        data.Map((x, y, previous) => { return -100.0f; });
        data.Update(0, 0, ChunkData.CircleAdd(new Vector2(40, 40), 40), false);
        //data.Map((x, y, previous) => { return Random.Range(-1.0f, 1.0f); });
        GenerateNewMesh();
    }

    void GenerateNewMesh()
    {
        (var mesh, var paths) = data.GenerateMesh(0.1f);
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<PolygonCollider2D>().pathCount = paths.Count;
        for (int i = 0; i < paths.Count; i++) {
            GetComponent<PolygonCollider2D>().SetPath(i, paths[i]);
        }
    }
}

public class ChunkData
{
    public float[,] densities; // positives = terrain, negative = air

    public ChunkData(int sizeX,int sizeY)
    {
        densities = new float[sizeX,sizeY];
    }

    public ChunkData(float[,] densities)
    {
        this.densities = densities;
    }

    public void Map(System.Func<int,int,float,float> lamda)
    {
        for(int x = 0; x < densities.GetLength(0); x++)
        {
            for(int y = 0; y < densities.GetLength(1); y++)
            {
                densities[x, y] = lamda(x, y, densities[x, y]);
            }
        }
    }

    public void Update(int xStart, int yStart, float[,] newDensities, bool overrideWithMin = true)
    {
        for(int xOffset = 0; xOffset < newDensities.GetLength(0); xOffset++)
        {
            for(int yOffset = 0; yOffset < newDensities.GetLength(1); yOffset++)
            {
                if (overrideWithMin)
                {
                    densities[xStart + xOffset,yStart + yOffset] = Mathf.Min(newDensities[xOffset,yOffset], densities[xStart + xOffset, yStart + yOffset]);
                }
                else
                {
                    densities[xStart + xOffset, yStart + yOffset] = Mathf.Max(newDensities[xOffset, yOffset], densities[xStart + xOffset, yStart + yOffset]);
                }
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

    public (Mesh,List<List<Vector2>>) GenerateMesh(float edgeLength = 0.1f)
    {
        var vertices = new List<Vector3>();
        var uv = new List<Vector2>();
        var triangles = new List<int>();
        var meshEdgeEdgesDict = new Dictionary<Vector2, Vector2>();

        Action<int,int,int> addRect = (int x,int yStart,int yEnd) => {
            int startingIndex = vertices.Count;
            vertices.Add(new Vector3(x, yStart));
            vertices.Add(new Vector3((x + 1), yStart));
            vertices.Add(new Vector3(x, yEnd));
            vertices.Add(new Vector3((x + 1), yEnd));
            uv.Add(new Vector2(0, 0));
            uv.Add(new Vector2(1, 0));
            uv.Add(new Vector2(0, 1));
            uv.Add(new Vector2(1, 1));

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
            if(yEnd == densities.GetLength(1)-1)
            {
                meshEdgeEdgesDict[new Vector2(x, yEnd)] = new Vector2((x + 1), yEnd);
            }
            if(x == 0)
            {
                meshEdgeEdgesDict[new Vector2(x, yStart)] = new Vector2(x, yEnd);
            }
            if (x == densities.GetLength(0) - 1)
            {
                meshEdgeEdgesDict[new Vector2((x+1), yEnd)] = new Vector2((x+1), yStart);
            }
        };

        for (int x = 0; x < densities.GetLength(0) - 1; x++)
        {
            int firstSolid = -1;
            for (int y = 0; y < densities.GetLength(1) - 1; y++)
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
                            uv.Add(new Vector2(corners[i, 0], corners[i, 1]));
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
                            vertices.Add(Vector3.Lerp(
                                new Vector3((x + corners[corner1, 0]), (y + corners[corner1, 1])),
                                new Vector3((x + corners[corner2, 0]), (y + corners[corner2, 1])),
                                LerpWeight(densities[(x + corners[corner1, 0]), (y + corners[corner1, 1])], densities[(x + corners[corner2, 0]), (y + corners[corner2, 1])])));
                            uv.Add(new Vector2(0,0));
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
                                if (key != value)
                                {
                                    meshEdgeEdgesDict[key] = value;
                                }
                            }
                        }
                        if (x == 0 || y == 0 || x == densities.GetLength(0) - 1 || y == densities.GetLength(1) - 1)
                        {
                            for (int trixEdgeId = 0; trixEdgeId < 3; trixEdgeId++)
                            {
                                var key = (Vector2)vertices[startingIndex + triangleConnections[mask, idst + trixEdgeId]];
                                var value = (Vector2)vertices[startingIndex + triangleConnections[mask, idst + ((trixEdgeId + 1) % 3)]];
                                if(key.x == value.x && (key.x == 0 || key.x == (densities.GetLength(0) - 1)))
                                {
                                    meshEdgeEdgesDict[key] = value;
                                }
                                if(key.y == value.y && (key.y == 0 || key.y == (densities.GetLength(0) - 1)))
                                {
                                    meshEdgeEdgesDict[key] = value;
                                }
                            }
                        }
                    }
                }
            }
        }

        for(int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = vertices[i]*edgeLength;
        }

        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        var colliderPaths = new List<List<Vector2>>();
        while(meshEdgeEdgesDict.Count > 0)
        {
            var path = new List<Vector2>();
            var enumerator = meshEdgeEdgesDict.Keys.GetEnumerator();
            enumerator.MoveNext();
            var current = enumerator.Current;
            while (meshEdgeEdgesDict.ContainsKey(current))
            {
                var next = meshEdgeEdgesDict[current];
                path.Add(current * edgeLength);
                meshEdgeEdgesDict.Remove(current);
                current = next;
            }
            path.Add(current * edgeLength);
            colliderPaths.Add(path);
        }

        return (mesh, colliderPaths);
    }

    public Vector3 posToVector3(int x, int y, float edgeLength)
    {
        return new Vector3(x * edgeLength, y * edgeLength);
    }

    float LerpWeight(float weight1, float weight2)
    {
        if(weight1 * weight2 > 0)
        {
            throw new System.Exception("Weights can't both be positive");
        }
        return Mathf.Abs(weight1) / (Mathf.Abs(weight1) + Mathf.Abs(weight2));
    }

    Vector3 ToVertexPosition(int x, int y, float edgeLength)
    {
        return new Vector3(x * edgeLength, y * edgeLength);
    }

    public static float[,] CircleAdd(Vector2 positionOffset, float radius)
    {
        int sizeX = Mathf.RoundToInt(radius * 2 + 1.5f);
        int sizeY = Mathf.RoundToInt(radius * 2 + 1.5f);
        var newDensities = new float[sizeX, sizeY];
        for(int x = 0; x < sizeX; x++)
        {
            for(int y = 0; y < sizeY; y++)
            {
                newDensities[x, y] = radius/(positionOffset - new Vector2(x, y)).magnitude - 1;
            }
        }
        return newDensities;
    }
}
