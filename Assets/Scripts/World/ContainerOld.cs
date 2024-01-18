using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class ContainerOld : MonoBehaviour
{
    private Block[] blockData;
    private Texture2D textureAtlas;
    private Vector3 blockPos;
    public Vector3 containerPosition;

    // Noise buffer data
    private Dictionary<Vector3, Voxel> data;
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
    public void Initialize(Vector3 position, Texture2D atlas, Block[] newBlockData)
    {
        ConfigureComponents();
        textureAtlas = atlas;
        blockData = newBlockData;
        data = new Dictionary<Vector3, Voxel>();
        containerPosition = position;

        /*vertices.Clear();
        triangles.Clear();
        uvs.Clear();

        //Get atlas width
        textureWidth = textureAtlas.width;*/
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
        //ComputeManager.Instance.CleanAndRequeueBuffer(data);
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
        Voxel block;

        //int counter = 0;
        Vector3[] faceVertices = new Vector3[4];
        Vector2[] faceUVs = new Vector2[4];

        for (int x = 1; x < WorldManager.WorldSettings.containerSize + 1; x++)
        {
            for (int y = 0; y < WorldManager.WorldSettings.maxHeight; y++)
            {
                for (int z = 1; z < WorldManager.WorldSettings.containerSize + 1; z++)
                {
                    blockPos = new(x, y, z);
                    block = this[blockPos];
                    
                    DrawCube();

                    //Set the mesh data
                    meshData.vertices = vertices.ToArray();
                    meshData.triangles = triangles.ToArray();
                    meshData.SetUVs(0, uvs.ToArray());
                    //Recalculate lightning
                    meshData.RecalculateNormals();
                }
            }
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

    /// <summary>
    /// Check if voxel in point is solid
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool checkVoxelIsSolid(Vector3 point)
    {
        if (point.y < 0 || (point.x > WorldManager.WorldSettings.containerSize + 2) || (point.z > WorldManager.WorldSettings.containerSize + 2))
            return true;
        else
            return this[point].isSolid;
    }

    public Voxel this[Vector3 index]
    {
        get
        {
            return data[index];
        }

        set
        {
            data[index] = value;
        }
    }

    #region CubeGeneration

    private void DrawCube()
    {
        Front_GenerateFace();
        Back_GenerateFace();
        Right_GenerateFace();
        Left_GenerateFace();
        Top_GenerateFace();
        Bottom_GenerateFace();
    }

    private void Front_GenerateFace()
    {//Only check on solid blocks
        if (checkVoxelIsSolid(blockPos + voxelFaceChecks[1]))
            return;
        lastVertex = vertices.Count;

        //Declare vertices
        vertices.Add(blockPos + new Vector3(-0.5f, 0, 0.5f)); //0
        vertices.Add(blockPos + new Vector3(-0.5f, 1, 0.5f)); //1
        vertices.Add(blockPos + new Vector3(0.5f, 1, 0.5f)); //2
        vertices.Add(blockPos + new Vector3(0.5f, 0, 0.5f)); //3

        AddTriangles();
        AddUVs(0);
    }

    private void Back_GenerateFace()
    {
        if (checkVoxelIsSolid(blockPos + voxelFaceChecks[0]))
            return;
        lastVertex = vertices.Count;

        //Declare vertices
        vertices.Add(blockPos + new Vector3(0.5f, 0, -0.5f)); //0
        vertices.Add(blockPos + new Vector3(0.5f, 1, -0.5f)); //1
        vertices.Add(blockPos + new Vector3(-0.5f, 1, -0.5f)); //2
        vertices.Add(blockPos + new Vector3(-0.5f, 0, -0.5f)); //3

        AddTriangles();
        AddUVs(0);
    }

    private void Left_GenerateFace()
    {
        if (checkVoxelIsSolid(blockPos + voxelFaceChecks[2]))
            return;
        lastVertex = vertices.Count;

        //Declare vertices
        vertices.Add(blockPos + new Vector3(-0.5f, 0, -0.5f)); //0
        vertices.Add(blockPos + new Vector3(-0.5f, 1, -0.5f)); //1
        vertices.Add(blockPos + new Vector3(-0.5f, 1, 0.5f)); //2
        vertices.Add(blockPos + new Vector3(-0.5f, 0, 0.5f)); //3

        AddTriangles();
        AddUVs(0);
    }

    private void Right_GenerateFace()
    {
        if (checkVoxelIsSolid(blockPos + voxelFaceChecks[3]))
            return;
        lastVertex = vertices.Count;

        //Declare vertices
        vertices.Add(blockPos + new Vector3(0.5f, 0, 0.5f)); //0
        vertices.Add(blockPos + new Vector3(0.5f, 1, 0.5f)); //1
        vertices.Add(blockPos + new Vector3(0.5f, 1, -0.5f)); //2
        vertices.Add(blockPos + new Vector3(0.5f, 0, -0.5f)); //3

        AddTriangles();
        AddUVs(0);
    }

    private void Top_GenerateFace()
    {
        if (checkVoxelIsSolid(blockPos + voxelFaceChecks[5]))
            return;
        lastVertex = vertices.Count;

        //Declare vertices
        vertices.Add(blockPos + new Vector3(0.5f, 1, -0.5f)); //0
        vertices.Add(blockPos + new Vector3(0.5f, 1, 0.5f)); //1
        vertices.Add(blockPos + new Vector3(-0.5f, 1, 0.5f)); //2
        vertices.Add(blockPos + new Vector3(-0.5f, 1, -0.5f)); //3

        AddTriangles();
        AddUVs(1);
    }

    private void Bottom_GenerateFace()
    {
        if (checkVoxelIsSolid(blockPos + voxelFaceChecks[4]))
            return;
        lastVertex = vertices.Count;

        //Declare vertices
        vertices.Add(blockPos + new Vector3(-0.5f, 0, -0.5f)); //0
        vertices.Add(blockPos + new Vector3(-0.5f, 0, 0.5f)); //1
        vertices.Add(blockPos + new Vector3(0.5f, 0, 0.5f)); //2
        vertices.Add(blockPos + new Vector3(0.5f, 0, -0.5f)); //3

        AddTriangles();
        AddUVs(2);
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

    private void AddUVs(int uvFace)
    {
        uvs.Add((blockData[0].atlasCoordinate[uvFace] * textureWidth) / (textureWidth * numTextures));
        uvs.Add(((blockData[0].atlasCoordinate[uvFace] + Vector2.up) * textureWidth) / (textureWidth * numTextures));
        uvs.Add(((blockData[0].atlasCoordinate[uvFace] + Vector2.one) * textureWidth) / (textureWidth * numTextures));
        uvs.Add(((blockData[0].atlasCoordinate[uvFace] + Vector2.right) * textureWidth) / (textureWidth * numTextures));
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
