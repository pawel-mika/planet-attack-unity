using System;
using System.Collections.Generic;
using System.Linq;
using PlanetAttack;
using PlanetAttack.Enums;
using PlanetAttack.ThePlanet;
using UnityEngine;

public class PlanetActions : MonoBehaviour
{
    private readonly GameController GameController = GameManager.GameController;
    private readonly PlanetsController PlanetsController = GameManager.PlanetsController;
    private readonly ActionsController ActionsController = GameManager.ActionsController;

    public MainPlanet planet;

    private float dragDistanceThreshold = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(String.Format("Start planet actions object: {0}", planet.name));
    }

    // Update is called once per frame
    void Update()
    {
        if (PlanetsController.dragStartPlanet
            && PlanetsController.dragStartPlanet.PlanetOwner == EPlayerType.PLAYER
            && PlanetsController.dragOverPlanet
            && PlanetsController.dragOverPlanet != PlanetsController.dragStartPlanet)
            {
                if (PlanetsController.dragOverPlanet.PlanetOwner != EPlayerType.PLAYER && PlanetsController.dragOverPlanet.PlanetState != EPlanetState.POTENTIAL_TARGET) {
                    PlanetsController.dragOverPlanet.SetPlanetState(EPlanetState.POTENTIAL_TARGET);
                } else if(PlanetsController.dragOverPlanet.PlanetOwner == EPlayerType.PLAYER && PlanetsController.dragOverPlanet.PlanetState != EPlanetState.POTENTIAL_TRANSFER) {
                    PlanetsController.dragOverPlanet.SetPlanetState(EPlanetState.POTENTIAL_TRANSFER);
                }
            }
    }


    private void TestPlanetSelection()
    {
        MainPlanet mp = GetPlanetUnderCursor();
        if (mp && mp.name == name)
        {
            Debug.Log(String.Format("TestPlanetSelection: {0}, isDragging: {1}, dragOverPlanet: {2}", planet.name, GameController.isDragging, PlanetsController.dragOverPlanet));
            planet.SetPlanetOwner(EPlayerType.PLAYER);
            planet.SetPlanetState(planet.PlanetState == EPlanetState.OWNED ? EPlanetState.SELECTED : EPlanetState.OWNED);
        }
    }

    private void SetDragOverPlanet()
    {
        // do some cleaning before we potentially set dragOverPlanet
        PlanetsController.dragOverPlanet?.RevertPreviousState();
        PlanetsController.dragOverPlanet = GetPlanetUnderCursor();
    }

    private void ClearDragOverPlanet()
    {
        PlanetsController.dragOverPlanet?.RevertPreviousState();
        PlanetsController.dragOverPlanet = null;
    }

    public void OnMouseDown()
    {
        Debug.Log(String.Format("OnMouseDown in: {0}", planet.name));
        ActionsController.mouseDownPoint = GetCurrentMousePositionInSpace();
    }

    public void OnMouseUp()
    {
        Debug.Log(String.Format("OnMouseUp in: {0}", planet.name));
        TestPlanetSelection();
        PlanetsController.dragStartPlanet = null;
        ActionsController.mouseDownPoint = Vector3.negativeInfinity;
        ActionsController.dragStartPoint = Vector3.negativeInfinity;
        ActionsController.dragTargetPoint = Vector3.negativeInfinity;
        if (!GetPlanetUnderCursor())
        {
            ClearDrawTargetAttackArrows();
            PlanetsController.ClearAllPotentnialTargets();
        }
    }

    public void OnMouseDrag()
    {
        Debug.Log(String.Format("OnMouseDrag in: {0}", planet.name));
        if(GetDragDistance() > dragDistanceThreshold) {
            HandleDrag();
        }
    }

    private void HandleDrag()
    {
        GameController.isDragging = true;
        ActionsController.dragTargetPoint = GetCurrentMousePositionInSpace();

        MainPlanet mp = GetPlanetUnderCursor();

        // we only want to set drag start etc for drag started on our owned/selected planet
        if (mp && mp.name == name && (mp.PlanetState == EPlanetState.OWNED || mp.PlanetState == EPlanetState.SELECTED))
        {
            Debug.Log(String.Format("HandleDrag - start drag from: {0}", planet.name));
            ActionsController.dragStartPoint = transform.position;
            PlanetsController.dragStartPlanet = planet;
        }

        // update positions of the ends of attack arrows for selected/owned planets
        if (planet.PlanetState == EPlanetState.SELECTED || planet.PlanetState == EPlanetState.OWNED)
        {
            ClearDrawTargetAttackArrows(); // clear first to start fresh
            SetDragAttackActionArrows();
        }

        // do not set drag target for the same planet we initialized drag on... it doesn't make sense
        if (mp && mp.name != name /* && mp.PlanetState != EPlanetState.OWNED && mp.PlanetState != EPlanetState.SELECTED */ )
        {
            SetDragOverPlanet();
        }

        // if we're dragging and there's no planet under cursor, clear the drag over planet
        if (!mp)
        {
            ClearDragOverPlanet();
        }

        GameController.isDragging = false;
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

    private void SetDragAttackActionArrows()
    {
        Debug.Log(String.Format("SetDragAttackActionArrows in: {0}", planet.name));
        IEnumerable<MainPlanet> selectedPlanets = PlanetUtils.GetSelectedPlanets(EPlayerType.PLAYER);
        if (PlanetsController.dragStartPlanet.PlanetOwner == EPlayerType.PLAYER && !selectedPlanets.Contains(PlanetsController.dragStartPlanet))
        {
            // single planet action
            PlanetsController.dragStartPlanet.DrawTarget = GetCurrentMousePositionInSpace();
            Debug.Log(string.Format("SetDragAttackActionArrows in single planet: {0} ",PlanetsController.dragStartPlanet.name));
        }
        else if (PlanetsController.dragStartPlanet.PlanetOwner == EPlayerType.PLAYER && selectedPlanets.Contains(PlanetsController.dragStartPlanet))
        {
            //multi planet action
            if(GetPlanetUnderCursor()) {
                selectedPlanets = selectedPlanets.Concat(new [] {GetPlanetUnderCursor()});
            }
            foreach (MainPlanet p in selectedPlanets)
            {
                p.DrawTarget = GetCurrentMousePositionInSpace();
            }
            Debug.Log(string.Format("SetDragAttackActionArrows in planets: {0}", string.Join(", ", selectedPlanets.Select(p => p.name))));
        }
    }

    private void ClearDrawTargetAttackArrows()
    {
        Debug.Log(String.Format("CancelDrawingDragAttackArrows in: {0}", planet.name));
        foreach (MainPlanet p in PlanetUtils.GetAllThePlanets())
        {
            p.DrawTarget = p.transform.position;
        }
    }

    private MainPlanet GetPlanetUnderCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f))
        {
            if (raycastHit.transform != null)
            {
                return raycastHit.transform.gameObject.GetComponent<MainPlanet>();
            }
        }
        return null;
    }

    private float GetDragDistance() {
        if(ActionsController.mouseDownPoint != null && ActionsController.mouseDownPoint != Vector3.negativeInfinity) {
            return Vector3.Distance(ActionsController.mouseDownPoint, GetCurrentMousePositionInSpace());
        }
        return 0;
    }
}
