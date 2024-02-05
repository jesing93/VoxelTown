using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Container : MonoBehaviour
{
    private Dictionary<BlockType, Block> blockData;
    private Texture2D textureAtlas;
    private Vector3 blockPos;
    public Vector3 containerPosition;

    // Voxel data
    public Dictionary<Vector3, Voxel> data;
    private Mesh meshData;

    //Components
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    //Render data lists
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Vector2> uvs;

    private int lastVertex = 0;
    private int textureWidth = 0;
    private int numTextures = 16;

    private void Awake()
    {
        blockPos = new(0, 0, 0);
        meshData = new();
        vertices = new();
        triangles = new();
        uvs = new();
    }

    /// <summary>
    /// Initialize the voxels container
    /// </summary>
    /// <param name="atlas"></param>
    /// <param name="newBlockData"></param>
    public void Initialize(Vector3 position, Texture2D atlas, Dictionary<BlockType, Block> newBlockData)
    {
        ConfigureComponents();
        textureAtlas = atlas;
        blockData = newBlockData;
        data = new Dictionary<Vector3, Voxel>();
        containerPosition = position;

        vertices.Clear();
        triangles.Clear();
        uvs.Clear();

        //Get atlas width
        textureWidth = textureAtlas.width;
    }

    private void ConfigureComponents()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    /// <summary>
    /// Clear the buffer data
    /// </summary>
    public void ClearData()
    {
        data.Clear();
    }

    /// <summary>
    /// Clear, generate and upload the mesh
    /// </summary>
    public void RenderMesh()
    {
        meshData.Clear();
        GenerateMesh();
        UploadMesh();
    }

    /// <summary>
    /// Generate the voxels
    /// </summary>
    public void GenerateMesh()
    {

        //int counter = 0;
        Vector3[] faceVertices = new Vector3[4];
        Vector2[] faceUVs = new Vector2[4];

        foreach (KeyValuePair<Vector3, Voxel> kvp in data)
        {
            if (kvp.Value.blockType == BlockType.air)
                continue;

            DrawCube(kvp.Key, kvp.Value.blockType);

            //Set the mesh data
            meshData.vertices = vertices.ToArray();
            meshData.triangles = triangles.ToArray();
            meshData.SetUVs(0, uvs.ToArray());
            //Recalculate lightning
            meshData.RecalculateNormals();
        }
    }

    /// <summary>
    /// Apply the new mesh
    /// </summary>
    public void UploadMesh()
    {
        //meshData.UploadMesh();

        //Set the mesh
        meshFilter.mesh = meshData;
        meshCollider.sharedMesh = meshData;
        meshRenderer.material.mainTexture = textureAtlas;
    }

    public Voxel this[Vector3 index]
    {
        get
        {
            if (data.ContainsKey(index))
                return data[index];
            else
                return emptyVoxel;
        }

        set
        {
            if(!data.ContainsKey(index))
                data[index] = value;
            else
                data.Add(index, value);
        }
    }

    public static Voxel emptyVoxel = new ();

    #region CubeGeneration

    private void DrawCube( Vector3 blockPos, BlockType blockType )
    {
        Front_GenerateFace(blockPos, blockType);
        Back_GenerateFace(blockPos, blockType);
        Right_GenerateFace(blockPos, blockType);
        Left_GenerateFace(blockPos, blockType);
        Top_GenerateFace(blockPos, blockType);
        Bottom_GenerateFace(blockPos, blockType);
    }

    private void Front_GenerateFace(Vector3 blockPos, BlockType blockType)
    {//Only check on solid blocks
        if (this[blockPos + voxelFaceChecks[1]].isSolid)
            return;
        lastVertex = vertices.Count;

        //Declare vertices
        vertices.Add(blockPos + new Vector3(-0.5f, 0, 0.5f)); //0
        vertices.Add(blockPos + new Vector3(-0.5f, 1, 0.5f)); //1
        vertices.Add(blockPos + new Vector3(0.5f, 1, 0.5f)); //2
        vertices.Add(blockPos + new Vector3(0.5f, 0, 0.5f)); //3

        AddTriangles();
        AddUVs(blockData[blockType].atlasCoordinate[0]);
    }

    private void Back_GenerateFace(Vector3 blockPos, BlockType blockType)
    {
        if (this[blockPos + voxelFaceChecks[0]].isSolid)
            return;
        lastVertex = vertices.Count;

        //Declare vertices
        vertices.Add(blockPos + new Vector3(0.5f, 0, -0.5f)); //0
        vertices.Add(blockPos + new Vector3(0.5f, 1, -0.5f)); //1
        vertices.Add(blockPos + new Vector3(-0.5f, 1, -0.5f)); //2
        vertices.Add(blockPos + new Vector3(-0.5f, 0, -0.5f)); //3

        AddTriangles();
        AddUVs(blockData[blockType].atlasCoordinate[0]);
    }

    private void Left_GenerateFace(Vector3 blockPos, BlockType blockType)
    {
        if (this[blockPos + voxelFaceChecks[2]].isSolid)
            return;
        lastVertex = vertices.Count;

        //Declare vertices
        vertices.Add(blockPos + new Vector3(-0.5f, 0, -0.5f)); //0
        vertices.Add(blockPos + new Vector3(-0.5f, 1, -0.5f)); //1
        vertices.Add(blockPos + new Vector3(-0.5f, 1, 0.5f)); //2
        vertices.Add(blockPos + new Vector3(-0.5f, 0, 0.5f)); //3

        AddTriangles();
        AddUVs(blockData[blockType].atlasCoordinate[0]);
    }

    private void Right_GenerateFace(Vector3 blockPos, BlockType blockType)
    {
        if (this[blockPos + voxelFaceChecks[3]].isSolid)
            return;
        lastVertex = vertices.Count;

        //Declare vertices
        vertices.Add(blockPos + new Vector3(0.5f, 0, 0.5f)); //0
        vertices.Add(blockPos + new Vector3(0.5f, 1, 0.5f)); //1
        vertices.Add(blockPos + new Vector3(0.5f, 1, -0.5f)); //2
        vertices.Add(blockPos + new Vector3(0.5f, 0, -0.5f)); //3

        AddTriangles();
        AddUVs(blockData[blockType].atlasCoordinate[0]);
    }

    private void Top_GenerateFace(Vector3 blockPos, BlockType blockType)
    {
        if (this[blockPos + voxelFaceChecks[5]].isSolid)
            return;
        lastVertex = vertices.Count;

        //Declare vertices
        vertices.Add(blockPos + new Vector3(0.5f, 1, -0.5f)); //0
        vertices.Add(blockPos + new Vector3(0.5f, 1, 0.5f)); //1
        vertices.Add(blockPos + new Vector3(-0.5f, 1, 0.5f)); //2
        vertices.Add(blockPos + new Vector3(-0.5f, 1, -0.5f)); //3

        AddTriangles();
        AddUVs(blockData[blockType].atlasCoordinate[1]);
    }

    private void Bottom_GenerateFace(Vector3 blockPos, BlockType blockType)
    {
        if (this[blockPos + voxelFaceChecks[4]].isSolid)
            return;
        lastVertex = vertices.Count;

        //Declare vertices
        vertices.Add(blockPos + new Vector3(-0.5f, 0, -0.5f)); //0
        vertices.Add(blockPos + new Vector3(-0.5f, 0, 0.5f)); //1
        vertices.Add(blockPos + new Vector3(0.5f, 0, 0.5f)); //2
        vertices.Add(blockPos + new Vector3(0.5f, 0, -0.5f)); //3

        AddTriangles();
        AddUVs(blockData[blockType].atlasCoordinate[2]);
    }

    private void AddTriangles()
    {
        //First triangle
        triangles.Add(lastVertex + 2);
        triangles.Add(lastVertex + 1);
        triangles.Add(lastVertex);

        //Second triangle
        triangles.Add(lastVertex);
        triangles.Add(lastVertex + 3);
        triangles.Add(lastVertex + 2);
    }

    private void AddUVs(Vector2 coord)
    {
        uvs.Add((coord * textureWidth) / (textureWidth * numTextures));
        uvs.Add(((coord + Vector2.up) * textureWidth) / (textureWidth * numTextures));
        uvs.Add(((coord + Vector2.one) * textureWidth) / (textureWidth * numTextures));
        uvs.Add(((coord + Vector2.right) * textureWidth) / (textureWidth * numTextures));
    }

    #endregion

    #region VoxelStatics
    static readonly Vector3[] voxelFaceChecks = new Vector3[6]
    {
        new Vector3(0, 0, -1),//back
        new Vector3(0, 0, 1),//front
        new Vector3(-1, 0, 0),//left
        new Vector3(1, 0, 0),//right
        new Vector3(0, -1, 0),//bottom
        new Vector3(0, 1, 0),//top
    };
    #endregion
}
