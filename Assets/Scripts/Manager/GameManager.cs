using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEditor;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //public Transform player;

    private Data data;

    private PlayerInput playerInput;

    private void Awake()
    {
        instance = this;
        data = new ();

        playerInput = new PlayerInput();

        //Assign this function to the input event
        playerInput.Camera.Save.performed += e => OnQuickSave();
        playerInput.Camera.Load.performed += e => OnQuickLoad();

        //Enable player input
        playerInput.Enable();
    }

    private void OnQuickSave()
    {
        SaveData();
    }

    private void OnQuickLoad()
    {
        LoadData();
    }

    private void SaveData()
    {
        Debug.Log("Save");
        foreach (KeyValuePair<Vector3, Container> kvp in WorldManager.Instance.Chunks) {
            data.Chunks.Add(kvp.Key, kvp.Value.data);
        }
        data.PlayerPosition = CameraController.Instance.gameObject.transform.position;
        data.PlayerRotation = CameraController.Instance.gameObject.transform.rotation.eulerAngles;

        data.Save();
    }

    private void LoadData()
    {
        Debug.Log("Load");
        data.Load();
        WorldManager.Instance.LoadChunks(data.Chunks);

        CameraController.Instance.gameObject.transform.position = data.PlayerPosition;
        CameraController.Instance.gameObject.transform.rotation = Quaternion.Euler(data.PlayerRotation);
    }

    public void ResetData()
    {
        
    }
}
