using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DesktopCameraManager : MonoBehaviour
{
    public List<Camera> cameras;
    public int currentlyActive = 0;

    public InputAction CycleCamera;


    void Start()
    {
        CycleCamera.Enable();
        CycleCamera.performed += (InputAction.CallbackContext ctx) =>
        {
            NextCamera();
        };
    }

    public void NextCamera()
    {
        currentlyActive = (currentlyActive + 1) % cameras.Count;
        int currentIndex = 0;
        foreach (var camera in cameras)
            camera.gameObject.SetActive(currentIndex++ == currentlyActive);
    }

}
