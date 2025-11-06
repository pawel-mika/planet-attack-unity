using System;
using System.Collections.Generic;
using System.Linq;
using PlanetAttack;
using PlanetAttack.Enums;
using PlanetAttack.ThePlanet;
using Unity.VisualScripting;
using UnityEngine;

public class PlanetActions : MonoBehaviour
{
    private readonly PlanetsController PlanetsController = GameManager.PlanetsController;
    private readonly ActionsController ActionsController = GameManager.ActionsController;

    public MainPlanet planet;

    public TheLabel DebugLabel;

    private readonly float dragDistanceThreshold = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetDebugMode(Debug.isDebugBuild && !DebugLabel.gameObject.activeInHierarchy);
        }

        if(Input.GetMouseButtonUp(0)) {
            TestDeselectSelectedPlanets();
        }

        if (PlanetsController.dragStartPlanet
            && PlanetsController.dragStartPlanet.PlanetOwner == EPlayerType.PLAYER
            && PlanetsController.dragOverPlanet
            && PlanetsController.dragOverPlanet != PlanetsController.dragStartPlanet)
        {
            if (PlanetsController.dragOverPlanet.PlanetOwner != EPlayerType.PLAYER && PlanetsController.dragOverPlanet.PlanetState != EPlanetState.POTENTIAL_TARGET)
            {
                PlanetsController.dragOverPlanet.SetPlanetState(EPlanetState.POTENTIAL_TARGET);
            }
            else if (PlanetsController.dragOverPlanet.PlanetOwner == EPlayerType.PLAYER && PlanetsController.dragOverPlanet.PlanetState != EPlanetState.POTENTIAL_TRANSFER)
            {
                PlanetsController.dragOverPlanet.SetPlanetState(EPlanetState.POTENTIAL_TRANSFER);
            }
        }
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

        ActionsController.CreateActionFromCurrentState(GetSourcePlanets(), GetPlanetUnderCursor());

        // if (!GetPlanetUnderCursor())
        // {
        ClearDragOverPlanet();
        ClearDrawTargetAttackArrows();
        PlanetsController.ClearAllPotentnialTargets();
        // }

        // clear all dragging left overs
        PlanetsController.dragStartPlanet = null;
        PlanetsController.dragOverPlanet = null;
        ActionsController.mouseDownPoint = Vector3.negativeInfinity;
        ActionsController.dragStartPoint = Vector3.negativeInfinity;
        ActionsController.dragTargetPoint = Vector3.negativeInfinity;
    }

    public void OnMouseDrag()
    {
        Debug.Log(String.Format("OnMouseDrag in: {0}", planet.name));
        if (GetDragDistance() > dragDistanceThreshold)
        {
            HandleDrag();
        }
    }

    private void OnDrawGizmos()
    {
        MainPlanet mp = PlanetsController.dragOverPlanet;
        if (mp)
        {
            DebugLabel.LabelText = string.Format("DragOver: {0}, {1}, {2}", mp.name, mp.PlanetOwner, mp.PlanetState);
        }
        else
        {
            DebugLabel.LabelText = "...";
        }
    }

    public void SetDebugMode(bool isDebugMode)
    {
        if (DebugLabel)
        {
            DebugLabel.gameObject.SetActive(isDebugMode);
        }
    }

    private void HandleDrag()
    {
        MainPlanet mp = GetPlanetUnderCursor();

        ActionsController.isDragging = true;
        ActionsController.dragTargetPoint = GetCurrentMousePositionInSpace();

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
        // and only set drag target if drag is initialized from player owned planet
        if (mp && mp.name != name && PlanetsController.dragStartPlanet && PlanetsController.dragStartPlanet.PlanetOwner == EPlayerType.PLAYER)
        {
            SetDragOverPlanet();
        }

        // if we're dragging and there's no planet under cursor, clear the drag over planet
        if (!mp)
        {
            ClearDragOverPlanet();
        }

        ActionsController.isDragging = false;
    }

    private void TestPlanetSelection()
    {
        MainPlanet mp = GetPlanetUnderCursor();
        if (mp && mp.name == name)
        {
            Debug.Log(String.Format("TestPlanetSelection: {0}, isDragging: {1}, dragOverPlanet: {2}", planet.name, ActionsController.isDragging, PlanetsController.dragOverPlanet));
            if (Debug.isDebugBuild && Input.GetKey(KeyCode.LeftControl))
            {
                planet.SetPlanetOwner(EPlayerType.PLAYER);
                planet.SetPlanetState(planet.PlanetState == EPlanetState.OWNED ? EPlanetState.SELECTED : EPlanetState.OWNED);
            } else if(planet.PlanetOwner == EPlayerType.PLAYER){
                planet.SetPlanetState(planet.PlanetState == EPlanetState.OWNED ? EPlanetState.SELECTED : EPlanetState.OWNED);
            }
        }
    }

    private void TestDeselectSelectedPlanets()
    {
        MainPlanet mp = GetPlanetUnderCursor(); // that wont work here - need to move somewhere outside to check ig clicked outside planet
        if (!mp)
        {
            Debug.Log(String.Format("TestDeselectSelected - deselect selected planets"));
            foreach (MainPlanet selected in PlanetUtils.GetSelectedPlanets(EPlayerType.PLAYER))
            {
                selected.SetPlanetState(EPlanetState.OWNED);
            }
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

    private Vector3 GetCurrentMousePositionInSpace()
    {
        Ray R = Camera.main.ScreenPointToRay(Input.mousePosition); // Get the ray from mouse position
        Vector3 PO = planet.transform.position; // Take current position of this draggable object as Plane's Origin
        Vector3 PN = -Camera.main.transform.forward; // Take current negative camera's forward as Plane's Normal
        float t = Vector3.Dot(PO - R.origin, PN) / Vector3.Dot(R.direction, PN); // plane vs. line intersection in algebric form. It find t as distance from the camera of the new point in the ray's direction.
        Vector3 P = R.origin + R.direction * t; // Find the new point.
        return P;
    }

    private void SetDragAttackActionArrows()
    {
        Debug.Log(String.Format("SetDragAttackActionArrows in: {0}", planet.name));

        foreach (MainPlanet mp in GetSourcePlanets())
        {
            mp.DrawTarget = GetCurrentMousePositionInSpace();
        }
    }

    private void ClearDrawTargetAttackArrows()
    {
        Debug.Log(string.Format("CancelDrawingDragAttackArrows in: {0}", planet.name));
        foreach (MainPlanet p in PlanetUtils.GetAllThePlanets())
        {
            p.DrawTarget = p.transform.position;
        }
    }

    private LinkedList<MainPlanet> GetSourcePlanets()
    {
        Debug.Log(string.Format("GetSourcePlanets in: {0}", planet.name));
        LinkedList<MainPlanet> sourcePlanets = new();
        IEnumerable<MainPlanet> selectedPlanets = PlanetUtils.GetSelectedPlanets(EPlayerType.PLAYER);
        if (PlanetsController.dragStartPlanet && PlanetsController.dragStartPlanet.PlanetOwner == EPlayerType.PLAYER && !selectedPlanets.Contains(PlanetsController.dragStartPlanet))
        {
            // single planet action
            sourcePlanets.AddLast(PlanetsController.dragStartPlanet);
        }
        else if (PlanetsController.dragStartPlanet && PlanetsController.dragStartPlanet.PlanetOwner == EPlayerType.PLAYER && selectedPlanets.Contains(PlanetsController.dragStartPlanet))
        {
            //multi planet action
            sourcePlanets.AddRange(selectedPlanets);
        }
        return sourcePlanets;
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

    private float GetDragDistance()
    {
        if (ActionsController.mouseDownPoint != null && ActionsController.mouseDownPoint != Vector3.negativeInfinity)
        {
            return Vector3.Distance(ActionsController.mouseDownPoint, GetCurrentMousePositionInSpace());
        }
        return 0;
    }

}
