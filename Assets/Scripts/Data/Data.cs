using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : FileManager
{
    [SerializeField] private float time;
    [SerializeField] private Vector3 playerPosition;
    [SerializeField] private Vector3 playerRotation;
    [SerializeField] private Dictionary<Vector3, Dictionary<Vector3, Voxel>> chunks;

    public Data()
    {
        path = Application.persistentDataPath + "/Save/Data/data.cfg";
        Debug.Log(path);

        chunks = new();
        if (!Load())
        {
            ResetValues();
        }
    }

    public float Time { get => time; set => time = value; }
    public Vector3 PlayerPosition { get => playerPosition; set => playerPosition = value; }
    public Vector3 PlayerRotation { get => playerRotation; set => playerRotation = value; }
    public Dictionary<Vector3, Dictionary<Vector3, Voxel>> Chunks { get => chunks; set => chunks = value; }

    private void ResetValues()
    {
        time = 0;
        playerPosition = Vector2.zero;
        playerRotation = new Vector3 (-50, 45, 0);
        chunks.Clear();

        DeleteFolderTree();
        CreateFolderTree();

        Save();
    }


}
