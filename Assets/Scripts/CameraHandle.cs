using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Resolution resolution = Screen.currentResolution;
        GameObject sceneCamObj = GameObject.Find("SceneCamera");

        if (sceneCamObj != null)
        {
            // Should output the real dimensions of scene viewport
            Camera camera = sceneCamObj.GetComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = resolution.width/resolution.height;
            Debug.Log(camera.pixelRect);
            Debug.Log(camera.projectionMatrix);
            Debug.Log(camera.cameraToWorldMatrix);
            Debug.Log(camera.farClipPlane);
            Debug.Log(camera.fieldOfView);
            Debug.Log(camera.orthographicSize);
            Debug.Log(camera.orthographic);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
