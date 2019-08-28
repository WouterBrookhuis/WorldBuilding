using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GridTerrain : MonoBehaviour
{
    public GridTerrainData TerrainData = new GridTerrainData(63, 97, 1, 1);
    public bool SmoothTerrain;
    public float NodeGizmoSize = 0.1f;

    [Header("Map reset settings")]
    public int NewMapWidth = 23;
    public int NewMapHeight = 13;

    public int Width => TerrainData.Width;
    public int Height => TerrainData.Height;

    private void Start()
    {
        /*for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                if (y < Height * 0.25f || y > Height * 0.75f)
                {
                    TerrainData.Nodes[x, y] = x / 3;
                }
                else
                {
                    TerrainData.Nodes[x, y] = Mathf.Clamp(x / 3, 0, Width / 2 / 3);
                }
            }
        }*/

        GetComponent<MeshFilter>().sharedMesh = GenerateMesh();
    }

    private bool CutNorthWest(Vector3[] vertices, int topLeft, int topRight, int bottomLeft, int bottomRight)
    {
        bool useNorthwest = false;
        if(vertices[topLeft].y == vertices[bottomRight].y)
        {
            if(vertices[bottomLeft].y == vertices[topLeft].y || vertices[topRight].y == vertices[topLeft].y)
            {
                // 3 are on the same level, place edge Northwest
                useNorthwest = true;
            }
            // Not 3 on the same level
            else if(vertices[bottomLeft].y == vertices[topRight].y)
            {
                // Two pairs of same level nodes, check which one is lower
                if(vertices[bottomLeft].y > vertices[topLeft].y)
                {
                    // Northwest edge is lower so use that
                    useNorthwest = true;
                }
            }
            else
            {
                // both Northeast line nodes are on different levels
                if((vertices[topLeft].y + vertices[bottomRight].y) / 2 < (vertices[topRight].y + vertices[bottomLeft].y) / 2)
                {
                    // Northwest edge is lower so use that
                    useNorthwest = true;
                }
            }
        }
        else if(vertices[topRight].y == vertices[bottomLeft].y)
        {
            if(vertices[bottomRight].y == vertices[topRight].y || vertices[topLeft].y == vertices[topRight].y)
            {
                // 3 are on the same level, place edge Northwest
                useNorthwest = false;
            }
            // Not 3 on the same level
            else if(vertices[bottomRight].y == vertices[topLeft].y)
            {
                // Two pairs of same level nodes, check which one is lower
                if(vertices[bottomRight].y > vertices[topRight].y)
                {
                    // Northwest edge is lower so use that
                    useNorthwest = false;
                }
            }
            else
            {
                // both Northeast line nodes are on different levels
                if((vertices[topLeft].y + vertices[bottomRight].y) / 2 < (vertices[topRight].y + vertices[bottomLeft].y) / 2)
                {
                    // Northwest edge is lower so use that
                    useNorthwest = true;
                }
            }
        }
        // both Northeast line nodes are on different levels
        else if((vertices[topLeft].y + vertices[bottomRight].y) / 2 < (vertices[topRight].y + vertices[bottomLeft].y) / 2)
        {
            // Northwest edge is lower so use that
            useNorthwest = true;
        }

        return useNorthwest;
    }

    private Color32 CreateGray(int shade)
    {
        shade *= 10;
        return new Color32((byte)shade, (byte)shade, (byte)shade, 255);
    }

    public Mesh GenerateMesh()
    {
        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        var width = Width;
        var height = Height;
        var vertices = new Vector3[
            SmoothTerrain ?
            width * height :
            (width - 1) * (height - 1) * 6];
        var uvs = new Vector2[vertices.Length];
        var color = new Color32[vertices.Length];
        var triangles = new int[(width - 1) * (height - 1) * 2 * 6];
        int trisIndex = 0;
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if (SmoothTerrain)
                {
                    vertices[x + y * width] = TerrainData.GetNodePositionUnchecked(x, y);
                    uvs[x + y * width] = new Vector2(vertices[x + y * width].x, vertices[x + y * width].z);
                    var nodeLvl = TerrainData.Nodes[x + y * width];
                    color[x + y * width] = new Color32((byte)(nodeLvl * 10), (byte)(nodeLvl * 10), (byte)(nodeLvl * 10), 255);
                }
                
                if (x == 0 || y == 0)
                {
                    // No need to create triangles for the first row and column
                    continue;
                }

                // Now we create triangles for the quad behind us
                // So if we are at [2, 3] we also use nodes [1, 3], [2, 2] and [1, 2]
                // This way we know that those triangles already have their vertices
                // set and we can use the height to determine stuff

                var bottomLeft =    (x - 1) +  (y - 1) * width;
                var bottomRight =   (x) +      (y - 1) * width;
                var topLeft =       (x - 1) +  (y) * width;
                var topRight =      (x) +      (y) * width;

                if (!SmoothTerrain)
                {
                    // For hard edges we need extra vertices, so create one set of vertices per quad
                    var quadIdx = ((x - 1) + (y - 1) * (width - 1)) * 6;
                    bottomLeft = quadIdx + 0;
                    bottomRight = quadIdx + 1;
                    topLeft = quadIdx + 2;
                    topRight = quadIdx + 3;

                    // Guard against overwriting already set vertex data
                    Assert.AreEqual(Vector3.zero, vertices[bottomLeft]);
                    Assert.AreEqual(Vector3.zero, vertices[bottomRight]);
                    Assert.AreEqual(Vector3.zero, vertices[topLeft]);
                    Assert.AreEqual(Vector3.zero, vertices[topRight]);

                    vertices[bottomLeft] = TerrainData.GetNodePositionUnchecked(x - 1, y - 1);
                    vertices[bottomRight] = TerrainData.GetNodePositionUnchecked(x, y - 1);
                    vertices[topLeft] = TerrainData.GetNodePositionUnchecked(x - 1, y);
                    vertices[topRight] = TerrainData.GetNodePositionUnchecked(x, y);

                    uvs[bottomLeft] = new Vector2(0,0);
                    uvs[bottomRight] = new Vector2(1, 0);
                    uvs[topLeft] = new Vector2(0, 1);
                    uvs[topRight] = new Vector2(1, 1);

                    color[bottomLeft] = CreateGray(TerrainData[x - 1, y - 1]);
                    color[bottomRight] = CreateGray(TerrainData[x, y - 1]);
                    color[topLeft] = CreateGray(TerrainData[x - 1, y]);
                    color[topRight] = CreateGray(TerrainData[x, y]);

                    if(CutNorthWest(vertices, topLeft, topRight, bottomLeft, bottomRight))
                    {
                        // Line in the middel goes to top left
                        triangles[trisIndex + 0] = topLeft;
                        triangles[trisIndex + 1] = bottomRight;
                        triangles[trisIndex + 2] = bottomLeft;

                        var topLeft2 = quadIdx + 4;
                        var bottomRight2 = quadIdx + 5;
                        vertices[topLeft2] = vertices[topLeft];
                        vertices[bottomRight2] = vertices[bottomRight];
                        uvs[topLeft2] = uvs[topLeft];
                        uvs[bottomRight2] = uvs[bottomRight];
                        color[topLeft2] = color[topLeft];
                        color[bottomRight2] = color[bottomRight];

                        triangles[trisIndex + 3] = topLeft2;
                        triangles[trisIndex + 4] = topRight;
                        triangles[trisIndex + 5] = bottomRight2;
                    }
                    else
                    {
                        // Line in the middel goes to top right
                        triangles[trisIndex + 0] = topLeft;
                        triangles[trisIndex + 1] = topRight;
                        triangles[trisIndex + 2] = bottomLeft;

                        var topRight2 = quadIdx + 4;
                        var bottomLeft2 = quadIdx + 5;
                        vertices[topRight2] = vertices[topRight];
                        vertices[bottomLeft2] = vertices[bottomLeft];
                        uvs[topRight2] = uvs[topRight];
                        uvs[bottomLeft2] = uvs[bottomLeft];
                        color[topRight2] = color[topRight];
                        color[bottomLeft2] = color[bottomLeft];

                        triangles[trisIndex + 3] = topRight2;
                        triangles[trisIndex + 4] = bottomRight;
                        triangles[trisIndex + 5] = bottomLeft2;
                    }
                }
                else
                {
                    if(CutNorthWest(vertices, topLeft, topRight, bottomLeft, bottomRight))
                    {
                        triangles[trisIndex + 0] = topLeft;
                        triangles[trisIndex + 1] = bottomRight;
                        triangles[trisIndex + 2] = bottomLeft;

                        triangles[trisIndex + 3] = topLeft;
                        triangles[trisIndex + 4] = topRight;
                        triangles[trisIndex + 5] = bottomRight;
                    }
                    else
                    {
                        triangles[trisIndex + 0] = topLeft;
                        triangles[trisIndex + 1] = topRight;
                        triangles[trisIndex + 2] = bottomLeft;

                        triangles[trisIndex + 3] = topRight;
                        triangles[trisIndex + 4] = bottomRight;
                        triangles[trisIndex + 5] = bottomLeft;
                    }
                }

                trisIndex += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors32 = color;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        mesh.name = "Terrain Mesh";
        return mesh;
    }
}
