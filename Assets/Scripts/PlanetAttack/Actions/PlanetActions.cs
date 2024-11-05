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
            && PlanetsController.dragOverPlanet != PlanetsController.dragStartPlanet
            && PlanetsController.dragOverPlanet.PlanetState != EPlanetState.POTENTIAL_TARGET)
        {
            PlanetsController.dragOverPlanet.SetPlanetState(EPlanetState.POTENTIAL_TARGET);
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
    }

    public void OnMouseUp()
    {
        Debug.Log(String.Format("OnMouseUp in: {0}", planet.name));
        TestPlanetSelection();
        PlanetsController.dragStartPlanet = null;
        ActionsController.dragStartPoint = Vector3.zero;
        ActionsController.dragTargetPoint = Vector3.zero;
        if (!GetPlanetUnderCursor())
        {
            ClearDrawTargetAttackArrows();
            PlanetsController.ClearAllPotentnialTargets();
        }
    }

    public void OnMouseDrag()
    {
        Debug.Log(String.Format("OnMouseDrag in: {0}", planet.name));
        HandleDrag();
    }

    private void HandleDrag()
    {
        GameController.isDragging = true;
        ActionsController.dragTargetPoint = GetCurrentMousePositionInSpace();

        MainPlanet mp = GetPlanetUnderCursor();

        // we only want to set drag start etc for drag started on our owned/selected planet
        if (mp && mp.name == name && (mp.PlanetState == EPlanetState.OWNED || mp.PlanetState == EPlanetState.SELECTED))
        {
            ActionsController.dragStartPoint = transform.position;
            PlanetsController.dragStartPlanet = planet;
        }

        // update positions of the ends of attack arrows for selected/owned planets
        if (planet.PlanetState == EPlanetState.SELECTED || planet.PlanetState == EPlanetState.OWNED)
        {
            SetDragAttackActionArrows();
        }

        // do not set drag target for the same planet we initialized drag on... it doesn't make sense
        if (mp && mp.name != name && mp.PlanetState != EPlanetState.OWNED && mp.PlanetState != EPlanetState.SELECTED)
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
        Debug.Log(String.Format("DrawDragAttackActionArrows in: {0}", planet.name));
        IEnumerable<MainPlanet> selectedPlanets = PlanetUtils.GetSelectedPlanets(EPlayerType.PLAYER);
        if (selectedPlanets.Count() == 0 && PlanetsController.dragStartPlanet)
        {
            // single planet action
            PlanetsController.dragStartPlanet.DrawTarget = GetCurrentMousePositionInSpace();
        }
        else
        {
            //multi planet action
            selectedPlanets.Append(PlanetsController.dragStartPlanet);
            foreach (MainPlanet p in selectedPlanets)
            {
                p.DrawTarget = GetCurrentMousePositionInSpace();
            }
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
}
