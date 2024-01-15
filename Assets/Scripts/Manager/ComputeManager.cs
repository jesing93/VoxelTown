using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;

public class ComputeManager : MonoBehaviour
{
    public ComputeShader noiseShader;

    //Object pools
    private List<NoiseBuffer> allNoiseComputeBuffers = new();
    private Queue<NoiseBuffer> availableNoiseComputeBuffers = new();

    private int xThreads;
    private int yThreads;

    public void Initialize(int count = 256)
    {
        //Chunk size
        xThreads = WorldManager.WorldSettings.containerSize / 8 + 1;
        yThreads = WorldManager.WorldSettings.maxHeight / 8;

        //Noise shader options
        noiseShader.SetInt("containerSizeX", WorldManager.WorldSettings.containerSize);
        noiseShader.SetInt("containerSizeY", WorldManager.WorldSettings.maxHeight);

        noiseShader.SetBool("generateCaves", true);
        noiseShader.SetBool("forceFloor", true);

        noiseShader.SetInt("maxHeight", WorldManager.WorldSettings.maxHeight);
        noiseShader.SetInt("oceanHeight", 42);

        noiseShader.SetFloat("noiseScale", 0.004f);
        noiseShader.SetFloat("caveScale", 0.01f);
        noiseShader.SetFloat("caveThreshold", 0.8f);

        noiseShader.SetInt("surfaceVoxelID", 1);
        noiseShader.SetInt("subSurfaceVoxelId", 2);

        for (int i = 0; i < count; i++)
        {
            CreateNewNoiseBuffer();
        }
    }

    #region Noise Buffers

    #region Pooling

    /// <summary>
    /// Get an available noise buffer or create a new one if none available
    /// </summary>
    /// <returns></returns>
    public NoiseBuffer GetNoiseBuffer()
    {
        if (availableNoiseComputeBuffers.Count > 0)
        {
            return availableNoiseComputeBuffers.Dequeue();
        }
        else
        {
            return CreateNewNoiseBuffer(false);
        }
    }

    /// <summary>
    /// Create a new noise buffer
    /// </summary>
    /// <param name="enqueue"></param>
    /// <returns></returns>
    public NoiseBuffer CreateNewNoiseBuffer(bool enqueue = true)
    {
        NoiseBuffer buffer = new();
        buffer.InitializeBuffer();
        allNoiseComputeBuffers.Add(buffer);

        if(enqueue)
        {
            availableNoiseComputeBuffers.Enqueue(buffer);
        }
        return buffer;
    }

    /// <summary>
    /// Clear a chunk and enqueue the noise buffer
    /// </summary>
    /// <param name="buffer"></param>
    public void CleanAndRequeueBuffer(NoiseBuffer buffer)
    {
        ClearVoxelData(buffer);
        availableNoiseComputeBuffers.Enqueue(buffer);
    }

    #endregion

    #region Compute Helpers
    public void GenerateVoxelData (ref Container cont)
    {
        //Add a buffer to the the shader
        noiseShader.SetBuffer(0, "voxelArray", cont.data.noiseBuffer);
        //Cound the number of active buffers
        noiseShader.SetBuffer(0, "count", cont.data.countBuffer);

        noiseShader.SetVector("chunkPosition", cont.containerPosition);
        //A seed for the noise generation
        noiseShader.SetVector("seedOffset", Vector3.zero);

        noiseShader.Dispatch(0, xThreads, yThreads, xThreads);

        //Render chunk asynchronously
        AsyncGPUReadback.Request(cont.data.noiseBuffer, (callback) =>
        {
            callback.GetData<Voxel>(0).CopyTo(WorldManager.Instance.container.data.voxelArray.array);
            WorldManager.Instance.container.RenderMesh();
        });
    }

    /// <summary>
    /// Clear voxel data of the buffer
    /// </summary>
    /// <param name="buffer"></param>
    private void ClearVoxelData(NoiseBuffer buffer)
    {
        buffer.countBuffer.SetData(new int[] { 0 });
        noiseShader.SetBuffer(1, "voxelArray", buffer.noiseBuffer);
        noiseShader.Dispatch(1, xThreads, yThreads, xThreads);
    }

    #endregion

    #endregion

    private void OnApplicationQuit()
    {
        DisposeAllBuffers();
    }

    /// <summary>
    /// Empty buffers
    /// </summary>
    public void DisposeAllBuffers()
    {
        foreach (NoiseBuffer buffer in allNoiseComputeBuffers)
        {
            buffer.Dispose();
        }
    }

    //Singleton
    private static ComputeManager _instance;

    public static ComputeManager Instance
    {
        get
        {
            if(_instance == null )
                _instance = FindObjectOfType<ComputeManager>();
            return _instance;
        }
    }
}

public struct NoiseBuffer
{
    //Buffers
    public ComputeBuffer noiseBuffer;
    public ComputeBuffer countBuffer;

    public bool isInit;
    public bool isCleared;

    //Voxels in the chunk
    public IndexedArray<Voxel> voxelArray;

    /// <summary>
    /// Initialize buffer
    /// </summary>
    public void InitializeBuffer()
    {
        //Init count buffer
        countBuffer = new(1, 4, ComputeBufferType.Counter);
        countBuffer.SetCounterValue(0);
        countBuffer.SetData(new uint[] { 0 });

        //Init noise buffer
        voxelArray = new();
        noiseBuffer = new(voxelArray.Count, 4);
        noiseBuffer.SetData(voxelArray.GetData);

        isInit = true;
    }

    /// <summary>
    /// Clear the buffers
    /// </summary>
    public void Dispose()
    {
        countBuffer?.Dispose();
        noiseBuffer?.Dispose();

        isInit = false;
    }

    /// <summary>
    /// Buffer voxel struct
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Voxel this[Vector3 index]
    {
        get
        {
            return voxelArray[index];
        }
        set
        {
            voxelArray[index] = value;
        }
    }
}
