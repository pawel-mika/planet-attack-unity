using System.Collections.Generic;
using System.Net;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ActionRenderer : MonoBehaviour
{
    public InterplanetaryAction action;
    public Material PlayerActionMaterial;
    public Material EnemyActionMaterial;
    private LineRenderer lineRenderer;

    public Canvas Canvas;
    public GameObject IconShips;
    public GameObject IconMinerals;
    public GameObject IconFood;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupActionType();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 A = action.GetDstPlanet().transform.position;
        Vector3 B = action.GetSrcPlanet().transform.position;
        Vector3 endPoint = B - ((B - A) * action.GetProgress());
        lineRenderer.SetPosition(0, A);
        lineRenderer.SetPosition(1, endPoint);
        Canvas.transform.position = endPoint;
        if (action.GetProgress() >= 1)
        {
            FinishAction();
        }
    }

    void FinishAction()
    {
        action.FinalizeAction();
        Destroy(gameObject);
    }

    private void SetupActionType()
    {
        switch (action)
        {
            case AttackAction:
                IconShips.transform.position = IconMinerals.transform.position;
                IconMinerals.SetActive(false);
                IconFood.SetActive(false);
                break;
            case TransferAction:
                break;
        }
    }

}
