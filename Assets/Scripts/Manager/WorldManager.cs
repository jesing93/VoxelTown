using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Texture2D textureAtlas;
    public Block[] blockTypes;
    public WorldSettings worldSettings;

    public Container container;

    private void Start()
    {
        WorldSettings = worldSettings;
        ComputeManager.Instance.Initialize(1);
        GameObject cubeObj = new GameObject("CubeContainer");
        cubeObj.transform.parent = transform;
        container = cubeObj.AddComponent<Container>();
        container.Initialize(Vector3.zero);

        /*for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                int randomYHeight = Random.Range(1, 8);
                for (int y = 0; y < randomYHeight; y++)
                {
                    container[new Vector3(x, y, z)] = new Voxel() { ID = 1 };
                }
            }
        }*/
        ComputeManager.Instance.GenerateVoxelData(ref container);
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
