using System.Collections.Generic;
using System.Net;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ActionRenderer : MonoBehaviour
{
    public InterplanetaryAction action;
    private LineRenderer lineRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // lineRenderer.material = 
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 A = action.GetDstPlanet().transform.position;
        Vector3 B = action.GetSrcPlanet().transform.position;
        Vector3 endPoint = B - ((B - A) * action.GetProgress());
        lineRenderer.SetPosition(0, A);
        lineRenderer.SetPosition(1, endPoint);
        if(action.GetProgress() >= 1) {
            FinishAction();
        }
    }

    void FinishAction()
    {
        action.FinalizeAction();
        Destroy(gameObject);
    }

}
