using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Texture2D textureAtlas;
    public Block[] blockTypes;
    private Dictionary<BlockType, Block> blockDictionary = new();
    public WorldSettings worldSettings;

    //All generated chunks
    private Dictionary<Vector3, Container> chunks = new();

    private void Start()
    {
        foreach (Block block in blockTypes) { 
            blockDictionary.Add(block.blockType, block);
        }
        WorldSettings = worldSettings;
        GenerateChunk(Vector3.zero);
        GenerateChunk(Vector3.right);
        GenerateChunk(Vector3.forward);
        GenerateChunk(Vector3.forward + Vector3.right);
    }

    private void GenerateChunk(Vector3 index)
    {
        GameObject cubeObj = new GameObject("CubeContainer");
        cubeObj.transform.parent = transform;
        cubeObj.transform.position = index * 100;
        Container container = cubeObj.AddComponent<Container>();
        container.Initialize(Vector3.zero, textureAtlas, blockDictionary);

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

        chunks.Add(index, container);
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
