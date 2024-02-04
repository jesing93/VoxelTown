using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selector : MonoBehaviour
{
    private Camera mainCamera;
    public static Selector Instance;
    private LayerMask groundLayer;

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
        groundLayer = LayerMask.GetMask("Ground");
    }

    /// <summary>
    /// Get the tile that the mouse is hovering over
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCurTilePosition()
    {
        //return if we are hovering over UI
        if (EventSystem.current.IsPointerOverGameObject())
            return new Vector3(0, -99, 0);

        //create ray
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        //shoot the ray at the ground
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, groundLayer))
        {
            //Get the position at which we intersected the plane
            Vector3 newPos = hit.point;
            if (hit.normal == Vector3.up) //If we hit the top face
            {
                //newPos -= new Vector3(0.5f, 0f, 0.5f);
            }
            if (hit.normal == Vector3.down) //If we hit the bottom face
            {
                newPos += Vector3.down - new Vector3(0.5f, 0f, 0.5f);
            }
            else //If we hit a side face
            {
                newPos += hit.normal - new Vector3((hit.normal.x == 0 ? 1 : hit.normal.x) * 0.5f, 1f, (hit.normal.z == 0 ? 1 : hit.normal.z) * 0.5f);
            }

            //round that up to the nearest full number (nearest meter)
            newPos = new(Mathf.CeilToInt(newPos.x), Mathf.CeilToInt(newPos.y), Mathf.CeilToInt(newPos.z));
            return newPos;
        }

        return new Vector3(0, -99, 0);
    }
}
