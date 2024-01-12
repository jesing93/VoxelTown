using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    //Variables
    [Header("Camera movement")]
    public float moveSpeed = 5;
    public float minXRot = -90;
    public float maxXRot = -10;
    public float rotSpeed = 1;

    [Header("Camera zoom")]
    public float MinZoom = 5;
    public float MaxZoom = 40;
    public float zoomSpeed = 1;

    private float curXRot = -50;
    private float curZoom = 20;
    private float mouseX;
    private Vector2 moveInput;
    private bool isRotating;

    //Components
    private Camera cam;
    private PlayerInput inputAction;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        curZoom = cam.transform.localPosition.y;
        curXRot = -50;
    }

    public void OnZoom (InputAction.CallbackContext context)
    {
        curZoom += context.ReadValue<Vector2>().y * zoomSpeed;
        curZoom = Mathf.Clamp(curZoom, MinZoom, MaxZoom);
    }

    public void OnRotateKey(InputAction.CallbackContext context)
    {
        isRotating = context.ReadValueAsButton();
        if(!isRotating)
        {
            mouseX = 0;
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (isRotating)
        {
            mouseX = context.ReadValue<Vector2>().x;
            float mouseY = context.ReadValue<Vector2>().y;

            curXRot += -mouseY * rotSpeed;
            curXRot = Mathf.Clamp(curXRot, minXRot, maxXRot);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        //Zoom
        cam.transform.localPosition = Vector3.up * curZoom;
        //Rotation
        transform.eulerAngles = new Vector3(curXRot, transform.eulerAngles.y + (mouseX * rotSpeed), 0);

        //--Camera movement--
        //Normalize direction
        Vector3 forward = cam.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = cam.transform.right;

        Vector3 dir = forward * moveInput.y + right * moveInput.x;
        dir.Normalize();

        //Actual movement
        dir *= moveSpeed * Time.deltaTime;
        transform.position += dir;
    }
}
