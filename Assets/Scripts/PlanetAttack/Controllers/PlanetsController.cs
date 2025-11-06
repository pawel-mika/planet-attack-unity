using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public void ClearAllPotentnialTargets()
    {
        foreach (MainPlanet planet in PlanetUtils.GetPotentialTargetMarkedPlanets())
        {
            //planet.SetPlanetState(previousStates.GetValueOrDefault(planet, EPlanetState.NONE));
            planet.RevertPreviousState();
        }
    }

    public MainPlanet GetPlanetUnderCursor()
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