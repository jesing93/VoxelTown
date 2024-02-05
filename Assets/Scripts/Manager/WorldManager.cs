using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Texture2D textureAtlas;
    public Block[] blockTypes;
    private Dictionary<BlockType, Block> blockDictionary = new();
    public WorldSettings worldSettings;

    private NavMeshSurface navSurface;

    //All generated chunks
    private Dictionary<Vector3, Container> chunks = new();

    public Dictionary<Vector3, Container> Chunks { get => chunks; }

    private void Awake()
    {
        navSurface = GetComponentInChildren<NavMeshSurface>();
    }

    private void Start()
    {
        foreach (Block block in blockTypes) { 
            blockDictionary.Add(block.blockType, block);
        }
        WorldSettings = worldSettings;

        CreateChunk(Vector3.zero);
        CreateChunk(Vector3.forward);
        CreateChunk(Vector3.back);
        CreateChunk(Vector3.right);
        CreateChunk(Vector3.forward + Vector3.right);
        CreateChunk(Vector3.back + Vector3.right);
        CreateChunk(Vector3.left);
        CreateChunk(Vector3.forward + Vector3.left);
        CreateChunk(Vector3.back + Vector3.left);

        RegenerateNavMesh();
    }

    public void RegenerateNavMesh()
    {
        navSurface.RemoveData();
        navSurface.BuildNavMesh();
    }

    /// <summary>
    /// Create a new chunk and generate it if requested
    /// </summary>
    /// <param name="index"></param>
    /// <param name="generate"></param>
    private void CreateChunk(Vector3 index, bool generate = true)
    {
        GameObject cubeObj = new GameObject("CubeContainer");
        cubeObj.transform.parent = transform;
        cubeObj.transform.position = index * 100;
        cubeObj.layer = LayerMask.NameToLayer("Ground");
        Container container = cubeObj.AddComponent<Container>();
        container.Initialize(Vector3.zero, textureAtlas, blockDictionary);

        chunks.Add(index, container);

        if (generate)
        {
            //Island generation

            //Island length
            int randomX = Random.Range(10, 21);
            int lastMinZ = Random.Range(-1, 1);
            int lastMaxZ = Random.Range(0, 2);
            int offsetX = Random.Range(-20, 20);
            int offsetZ = Random.Range(-20, 20);

            //For each column while x < length or z not closed AND x is less than 25 to avoid overlapping of islands
            for (int x = 0; (x < randomX || lastMaxZ > lastMinZ) && x < 25; x++)
            {
                //If not first loop
                if (x != 0)
                {
                    //If first half: better growing probability
                    if (x < randomX / 2)
                    {
                        lastMinZ = lastMinZ + Random.Range(-3, 2);
                        lastMaxZ = lastMaxZ + Random.Range(-1, 4);
                    }
                    //If second half: better shrinking probability
                    else
                    {
                        lastMinZ = lastMinZ + Random.Range(-1, 4);
                        lastMaxZ = lastMaxZ + Random.Range(-3, 2);
                    }
                }

                //For each space between min and max z
                for (int z = lastMinZ; z < lastMaxZ; z++)
                {
                    //Random depth of each space
                    int randomYHeight = Random.Range(-3, -8);
                    for (int y = 0; y > randomYHeight; y--)
                    {
                        //Make first 2 blocks out of dirt
                        BlockType newBlocktype;
                        if (y >= -1)
                        {
                            newBlocktype = BlockType.dirt;
                        }
                        else
                        {
                            newBlocktype = BlockType.stone;
                        }
                        //Create voxel
                        container[new Vector3(x + offsetX, y, z + offsetZ)] = new Voxel { blockType = newBlocktype };
                    }
                }
            }
            //Once generated, render chunk
            container.RenderMesh();
        }
    }

    public void LoadChunks(Dictionary<Vector3, Dictionary<Vector3, Voxel>> loadedChunks)
    {
        //TODO: Fix chunk Save/Load
        chunks.Clear();
        foreach (KeyValuePair<Vector3, Dictionary<Vector3, Voxel>> kvp in loadedChunks)
        {
            Debug.Log("Load chunk: " + kvp.Key);
            CreateChunk(kvp.Key, false);
            chunks[kvp.Key].data = kvp.Value;
            chunks[kvp.Key].RenderMesh();
        }
    }

    public static WorldSettings WorldSettings;
    private static WorldManager _instance;

    public static WorldManager Instance
    {
        get
        {
            if(_instance == null )
            {
                _instance = FindObjectOfType<WorldManager>();
            }
            return _instance;
        }
    }
}

[System.Serializable]
public class WorldSettings
{
    public int containerSize = 16;
    public int maxHeight = 120;
}
