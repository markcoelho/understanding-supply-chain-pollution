using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    void OnPreCull()
    {
        // Flip the camera's projection matrix horizontally
        mainCamera.ResetWorldToCameraMatrix();
        mainCamera.ResetProjectionMatrix();
        Vector3 scale = new Vector3(-1, 1, 1);
        mainCamera.projectionMatrix = mainCamera.projectionMatrix * Matrix4x4.Scale(scale);
    }

    void OnPreRender()
    {
        // Invert the culling mode to render correctly
        GL.invertCulling = true;
    }

    void OnPostRender()
    {
        // Restore the culling mode
        GL.invertCulling = false;
    }
}