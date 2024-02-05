using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class BuildingPlacement : MonoBehaviour
{
    private bool isPlacing;
    private bool isBulldozering;
    private bool isPlacementValid;

    private BuildingPresets curBuildingPreset;

    private float indicatorUpdateRate = 0.05f;
    private float lastUpdateTime;
    private Vector3 curIndicatorPos;

    public GameObject placementIndicator;
    public GameObject bulldozerIndicator;
    public GameObject validIndicator;
    public GameObject invalidIndicator;

    private PlayerInput playerInput;

    private LayerMask buildingLayer;
    private LayerMask groundLayer;

    private void Awake()
    {
        buildingLayer = LayerMask.GetMask("Building");
        groundLayer = LayerMask.GetMask("Ground");
        playerInput = new PlayerInput();
        //Assign this function to the input event
        playerInput.Camera.Cancel.performed += e => OnCancelBuildingPlacement();
        playerInput.Camera.Confirm.performed += e => OnConfirm();
        playerInput.Camera.RotateBP.performed += e => OnRotate();
        //Enable player input
        playerInput.Enable();
    }

    private void Update()
    {
        if(Time.time - lastUpdateTime > indicatorUpdateRate)
        {
            lastUpdateTime = Time.time;

            curIndicatorPos = Selector.Instance.GetCurTilePosition();
            if(isPlacing)
            {
                CheckValidPlacement();
            }
            else if(isBulldozering)
            {
                bulldozerIndicator.transform.position = curIndicatorPos;
            }
        }
    }

    /// <summary>
    /// Check if building placement is valid
    /// </summary>
    private void CheckValidPlacement()
    {
        placementIndicator.transform.position = curIndicatorPos;
        Collider[] hitColliders = Physics.OverlapBox(placementIndicator.transform.position + new Vector3(0, 0.5f, 0), new Vector3(0.49f, 0.49f, 0.49f), Quaternion.identity, buildingLayer);
        //Debug.Log(hitColliders.Length);
        if (hitColliders.Length == 0 &&
            ((curBuildingPreset.resourceCost == ResourceType.money && City.Instance.money >= curBuildingPreset.cost) ||
            (curBuildingPreset.resourceCost == ResourceType.material && City.Instance.materials >= curBuildingPreset.cost)))
        {
            if(curBuildingPreset.buildingType == BuildingType.Block)
            {
                isPlacementValid = true;
            }
            else
            {
                Ray ray = new(placementIndicator.transform.position + new Vector3 (0, 0.5f, 0), Vector3.down);
                Debug.DrawRay(placementIndicator.transform.position + new Vector3(0, 0.5f, 0), Vector3.down, Color.red, 1);
                //CheckGround
                if (Physics.Raycast(ray, 1, groundLayer))
                {
                    isPlacementValid = true;
                }
                else
                {
                    isPlacementValid = false;
                }
            }
        }
        else
        {
            isPlacementValid = false;
        }
        if (isPlacementValid)
        {
            validIndicator.SetActive(true);
            invalidIndicator.SetActive(false);
        }
        else
        {
            validIndicator.SetActive(false);
            invalidIndicator.SetActive(true);
        }
    }

    /// <summary>
    /// Called when we press a building UI button
    /// </summary>
    /// <param name="preset"></param>
    public void BeginNewBuildingPlacement(BuildingPresets preset)
    {
        //TODO: make sure we have enough money

        if(isBulldozering)
        {
            ToggleBulldozer();
        }
        if(isPlacing && curBuildingPreset == preset)
        {
            CancelBuildingPlacement();
        }
        else
        {
            isPlacing = true;
            curBuildingPreset = preset;
            validIndicator.GetComponent<MeshFilter>().mesh = preset.prefab.GetComponentInChildren<MeshFilter>().sharedMesh;
            invalidIndicator.GetComponent<MeshFilter>().mesh = preset.prefab.GetComponentInChildren<MeshFilter>().sharedMesh;
            validIndicator.GetComponent<Transform>().localScale = preset.prefab.transform.GetChild(0).localScale;
            invalidIndicator.GetComponent<Transform>().localScale = preset.prefab.transform.GetChild(0).localScale;
            placementIndicator.SetActive(true);
        }
    }

    public void CancelBuildingPlacement()
    {
        isPlacing = false;
        curBuildingPreset = null;
        placementIndicator.SetActive(false);
        placementIndicator.transform.rotation = Quaternion.identity;
    }

    private void ToggleBulldozer()
    {
        isBulldozering = !isBulldozering;
        bulldozerIndicator.SetActive(isBulldozering);
    }

    public void OnBulldozer()
    {
        //cancel building placement
        if (isPlacing)
        {
            CancelBuildingPlacement();
        }
        ToggleBulldozer();
    }

    private void OnCancelBuildingPlacement()
    {
        //cancel building placement
        if(isPlacing)
        {
            CancelBuildingPlacement();
        }
        else if(isBulldozering)
        {
            ToggleBulldozer();
        }
    }

    /// <summary>
    /// Confirm action
    /// </summary>
    private void OnConfirm()
    {
        //Build
        if (isPlacing && isPlacementValid)
        {
            GameObject buildingObj = Instantiate(curBuildingPreset.prefab, curIndicatorPos, placementIndicator.transform.rotation);
            City.Instance.OnPlaceBuilding(buildingObj.GetComponent<Building>());
            CancelBuildingPlacement();
        }
        //Destroy
        else if (isBulldozering)
        {
            Building buildingToDestroy = City.Instance.buildings.Find(x => x.transform.position == curIndicatorPos);
            if(buildingToDestroy != null)
            {
                City.Instance.OnRemoveBuilding(buildingToDestroy);
            }
        }
        //Default
        else
        {
            //TODO: select item
        }
    }

    private void OnRotate()
    {
        if (isPlacing)
        {
            placementIndicator.transform.Rotate(0, 90, 0);
        }
    }
}
