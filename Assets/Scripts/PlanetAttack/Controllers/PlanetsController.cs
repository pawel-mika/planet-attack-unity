using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PlanetAttack;
using PlanetAttack.Enums;
using PlanetAttack.ThePlanet;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlanetsController
{
    private static readonly Lazy<PlanetsController> instance = new(() => new PlanetsController());

    private PlanetsController()
    {
        Debug.Log("PlanetsController created");
    }

    // Public static method to access the Singleton instance.
    public static PlanetsController Instance => instance.Value;

    public MainPlanet dragStartPlanet = null;

    public MainPlanet dragOverPlanet = null;

    private Dictionary<MainPlanet, EPlanetState> previousStates = new Dictionary<MainPlanet, EPlanetState>();

    private Camera _cachedCamera;

    private Camera GetCamera()
    {
        if (_cachedCamera == null)
        {
            _cachedCamera = Camera.main;
        }
        return _cachedCamera;
    }

    public void ClearDrawTargetAttackArrows()
    {
        Debug.Log(string.Format("CancelDrawingDragAttackArrows"));
        foreach (MainPlanet p in PlanetUtils.GetAllThePlanets())
        {
            p.DrawTarget = p.transform.position;
        }
    }

    public void ClearAllPotentnialTargets()
    {
        foreach (MainPlanet planet in PlanetUtils.GetPotentialTargetMarkedPlanets())
        {
            //planet.SetPlanetState(previousStates.GetValueOrDefault(planet, EPlanetState.NONE));
            planet.RevertPreviousState();
        }
    }

    // public MainPlanet GetPlanetUnderCursor()
    // {
    //     Camera cam = GetCamera();
    //     if (cam == null) return null;

    //     Ray ray = cam.ScreenPointToRay(Input.mousePosition);

    //     // Create a bitmask for the "Planets" layer
    //     int layerIndex = LayerMask.NameToLayer("Planets");

    //     // Check if the layer exists in the project settings
    //     if (layerIndex == -1)
    //     {
    //         Debug.LogWarning("Layer 'Planets' not found! Check your Tags and Layers settings.");
    //         // Fallback: raycast against everything if layer is missing
    //         if (Physics.Raycast(ray, out RaycastHit hit, 100f))
    //             return hit.transform.GetComponent<MainPlanet>();
    //         return null;
    //     }

    //     // Shift bit to create the mask
    //     int layerMask = 1 << layerIndex;

    //     // Perform Raycast only against the specified layer
    //     if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f, layerMask))
    //     {
    //         if (raycastHit.transform != null)
    //         {
    //             return raycastHit.transform.gameObject.GetComponent<MainPlanet>();
    //         }
    //     }

    //     return null;
    // }

    public int GetPlayerOwnedPlanetsCount()
    {
        return PlanetUtils.GetPlanetsByOwner(EPlayerType.PLAYER).Count();
    }

    public int GetEnemyOwnedPlanetsCount()
    {
        return PlanetUtils.GetPlanetsByOwner(EPlayerType.ENEMY).Count();
    }
}