using System;
using System.Numerics;
using TMPro;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActionRenderer : MonoBehaviour
{
    public InterplanetaryAction action;
    public Material PlayerActionMaterial;
    public Material EnemyActionMaterial;

    [Header("Animation Settings")]
    public float textureScrollSpeed = 0.25f;

    private LineRenderer lineRenderer;

    public Canvas Canvas;
    public GameObject IconShips;
    public GameObject IconMinerals;
    public GameObject IconFood;

    public GameObject LabelShips;
    public GameObject LabelMineral;
    public GameObject LabelFood;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupActionType();

        // WAŻNE: Wymuś utworzenie instancji materiału,
        // aby animacja działała tylko na tej linii, a nie na innych liniach
        // używających tego samego materiału.
        if (lineRenderer.material != null)
        {
            lineRenderer.material = new Material(lineRenderer.material);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 A = action.GetDstPlanet().transform.position;
        Vector3 B = action.GetSrcPlanet().transform.position;
        Vector3 endPoint = B - ((B - A) * action.GetProgress());
        lineRenderer.SetPosition(0, A);
        lineRenderer.SetPosition(1, endPoint);
        lineRenderer.endWidth = 0.25f;
        Canvas.transform.position = endPoint;
        if (action.GetProgress() >= 1)
        {
            FinishAction();
        }

        // ------------------
        // NOWA LOGIKA: ANIMACJA MATERIAŁU
        // ------------------
        // 1. Oblicz offset. Używamy Time.time * scrollSpeed, by uzyskać ciągły ruch.
        float offset = Time.time * textureScrollSpeed;

        // 2. Ustaw offset na materiale LineRenderer.
        // Używamy "_MainTex" jako nazwy głównej tekstury.
        // Przesunięcie na osi Y (V) jest standardowe dla przesuwania wzdłuż linii.
        lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(offset, 0f));
    }

    void FinishAction()
    {
        action.FinalizeAction();
        Destroy(gameObject);
    }

    private void SetupActionType()
    {
        TextMeshProUGUI tmShips = this.LabelShips.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI tmFood = this.LabelFood.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI tmMineral = this.LabelMineral.GetComponent<TextMeshProUGUI>();
        switch (action)
        {
            case AttackAction:
                IconShips.transform.position = IconMinerals.transform.position;
                IconMinerals.SetActive(false);
                IconFood.SetActive(false);
                LabelMineral.SetActive(false);
                LabelFood.SetActive(false);
                tmShips.text = String.Format("{0:F0}", ((AttackAction)action).Ships);
                break;
            case TransferAction:
                TransferAction ta = (TransferAction)action;
                tmFood.text = String.Format("{0:F0}", ta.Food);
                tmShips.text = String.Format("{0:F0}", ta.Ships);
                tmMineral.text = String.Format("{0:F0}", ta.Minerals);
                break;
        }
    }

}
