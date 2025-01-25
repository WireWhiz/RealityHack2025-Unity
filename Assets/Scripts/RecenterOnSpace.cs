using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecenterOnSpace : MonoBehaviour
{
    InputAction recenter;
    Transform playspaceRoot;
    Transform playerHead;

    void Start()
    {
        recenter.Enable();
        recenter.performed += (InputAction.CallbackContext ctx) =>
        {
            Recenter();
        };
    }

    public void Recenter()
    {
        // Center rotation
        playspaceRoot.rotation = Quaternion.AngleAxis(-playerHead.transform.rotation.eulerAngles.y, Vector3.up) * playspaceRoot.rotation;
        playspaceRoot.position -= Vector3.ProjectOnPlane(playerHead.transform.position, Vector3.up);
    }
}
