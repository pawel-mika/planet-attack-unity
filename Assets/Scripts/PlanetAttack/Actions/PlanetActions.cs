using System;
using System.Collections.Generic;
using PlanetAttack;
using PlanetAttack.Enums;
using PlanetAttack.ThePlanet;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetActions : MonoBehaviour, IDragHandler, IInitializePotentialDragHandler
{
    public MainPlanet planet;
    private Transform selection;
    private RaycastHit raycastHit;

    Vector3 dragStartPoint;
    Vector3 dragTargetPoint;

    EPlanetState previousState = EPlanetState.NONE;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckClicked();

        if (GameController.isDragging && planet.PlanetState == EPlanetState.SELECTED)
        {
            DrawDragAttackActionArrows();
        }

        if (GameController.isDragging
            && GameManager.PlanetsController.dragOverPlanet
            && GameManager.PlanetsController.dragOverPlanet != GameManager.PlanetsController.dragStartPlanet)
        {
            GameManager.PlanetsController.dragOverPlanet.SetPlanetState(EPlanetState.POTENTIAL_TARGET);
        }
        if(!GameManager.PlanetsController.dragOverPlanet)
        {
            GameManager.PlanetsController.ClearAllPotentnialTargets();
        }
    }

    private void CheckClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    if (raycastHit.transform.gameObject.name == name)
                    {
                        Debug.Log(String.Format("Raycast hit object: {0}, isDragging: {1}", planet.name, GameController.isDragging));
                    }
                    if (raycastHit.transform.gameObject.name == name && !GameController.isDragging)
                    {
                        planet.SetPlanetOwner(EPlayerType.PLAYER);
                        planet.SetPlanetState(planet.PlanetState == EPlanetState.OWNED ? EPlanetState.SELECTED : EPlanetState.OWNED);
                    }
                }
            }
        }
    }


    private void CheckDragOverPlanet()
    {
        GameManager.PlanetsController.dragOverPlanet = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f))
        {
            if (raycastHit.transform != null)
            {
                GameObject go = raycastHit.transform.gameObject;
                if (go.GetComponent<MainPlanet>())
                {
                    GameManager.PlanetsController.dragOverPlanet = go.GetComponent<MainPlanet>();
                }
            }
        }
    }

    public void OnMouseDown()
    {
        // isDragging = true;
        dragStartPoint = transform.position;
        GameManager.PlanetsController.dragStartPlanet = planet;
        Debug.Log(String.Format("Begin drag from: {0}", planet.name));
    }

    public void OnMouseUp()
    {
        // attack() ?
        GameController.isDragging = false;
        GameManager.PlanetsController.dragStartPlanet = null;
        dragStartPoint = Vector3.zero;
        dragTargetPoint = Vector3.zero;
        Debug.Log(String.Format("End drag of: {0}", planet.name));
        CancelDrawingDragAttackArrows();
        if (!GameManager.PlanetsController.dragOverPlanet)
        {
            GameManager.PlanetsController.ClearAllPotentnialTargets();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // DragPlanetToMousePosition();
        GameController.isDragging = true;
        dragTargetPoint = GetCurrentMousePositionInSpace();
        // Debug.Log(String.Format("Dragging from: {0}", planet.name));
        // Debug.Log(String.Format("{0} {1} {2}", eventData.pointerClick?.name, eventData.pointerEnter?.name, eventData.pointerDrag?.name));
        // CheckPotentialTargetPlanet();
        CheckDragOverPlanet();
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {

        eventData.useDragThreshold = true;
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

    private void DrawDragAttackActionArrows()
    {
        IEnumerable<MainPlanet> selectedPlanets = PlanetUtils.GetSelectedPlanets(EPlayerType.PLAYER);
        foreach (MainPlanet planet in selectedPlanets)
        {
            planet.DrawTarget = GetCurrentMousePositionInSpace();
        }
    }

    private void CancelDrawingDragAttackArrows()
    {
        IEnumerable<MainPlanet> selectedPlanets = PlanetUtils.GetSelectedPlanets(EPlayerType.PLAYER);
        foreach (MainPlanet planet in selectedPlanets)
        {
            Debug.Log(String.Format("Cancel drag for: {0}", planet.name));
            planet.DrawTarget = planet.transform.position;
        }
    }
}
