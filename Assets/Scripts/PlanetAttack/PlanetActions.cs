using System;
using System.Collections;
using System.Collections.Generic;
using PlanetAttack;
using PlanetAttack.ThePlanet;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetActions : MonoBehaviour, IDragHandler, IInitializePotentialDragHandler
{
    public MainPlanet planet;
    private Transform selection;
    private RaycastHit raycastHit;

    bool isDragging;
    Vector3 dragStartPoint;
    Vector3 dragTargetPoint;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckClicked();
        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // bool c = Physics.Raycast(ray, out raycastHit);
        // if (Input.GetKey(KeyCode.Mouse0) /* && !EventSystem.current.IsPointerOverGameObject() */)
        // {
        //     selection = raycastHit.transform;
        //     // Debug.Log(typeof selection.gameObject).SendMessage("");
        //     GameObject p = selection.GetComponent<GameObject>();
        //     Debug.Log(selection, p);
        //     if (p != null)
        //     {
        //         p.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //     }
        // }

        DrawDragAttackActionArrows();
    }

    private void CheckClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    if (raycastHit.transform.gameObject.name == name)
                    {
                        planet.SetPlanetOwner(GameController.EPlayerType.PLAYER);
                        planet.SetPlanetState(planet.PlanetState == GameController.EPlanetState.OWNED ? GameController.EPlanetState.SELECTED : GameController.EPlanetState.OWNED);
                    }
                }
            }
        }
    }

    public void OnBeginDrag()
    {
        dragStartPoint = transform.position;
        isDragging = true;
    }

    public void OnEndDrag()
    {
        // attack() ?
        isDragging = false;
        dragStartPoint = Vector3.zero;
        dragTargetPoint = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragTargetPoint = GetCurrentMousePositionInSpace();
        // DragPlanetToMousePosition();
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        eventData.useDragThreshold = false;
    }

    private void DragPlanetToMousePosition()
    {
        Vector3 P = GetCurrentMousePositionInSpace();

        planet.transform.position = P;
    }

    private Vector3 GetCurrentMousePositionInSpace()
    {
        Ray R = Camera.main.ScreenPointToRay(Input.mousePosition); // Get the ray from mouse position
        Vector3 PO = transform.position; // Take current position of this draggable object as Plane's Origin
        Vector3 PN = -Camera.main.transform.forward; // Take current negative camera's forward as Plane's Normal
        float t = Vector3.Dot(PO - R.origin, PN) / Vector3.Dot(R.direction, PN); // plane vs. line intersection in algebric form. It find t as distance from the camera of the new point in the ray's direction.
        Vector3 P = R.origin + R.direction * t; // Find the new point.
        return P;
    }

    private void DrawDragAttackActionArrows() {
        IEnumerable<MainPlanet> selectedPlanets = PlanetUtils.GetSelectedPlanets(GameController.EPlayerType.PLAYER);
        Vector3 dragTarget =
    }


}
