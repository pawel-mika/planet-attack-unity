using System;
using System.Collections.Generic;
using PlanetAttack;
using PlanetAttack.Enums;
using PlanetAttack.ThePlanet;

public class PlanetsController
{

    private static readonly Lazy<PlanetsController> instance = new(() => new PlanetsController());

    private PlanetsController()
    {
        Console.WriteLine("Singleton instance created.");
    }

    // Public static method to access the Singleton instance.
    public static PlanetsController Instance => instance.Value;

    public MainPlanet dragStartPlanet = null;

    public MainPlanet dragOverPlanet = null;

    private Dictionary<MainPlanet, EPlanetState> previousStates = new Dictionary<MainPlanet,EPlanetState>();

    public void ClearAllPotentnialTargets() {
        foreach(MainPlanet planet in PlanetUtils.GetPotentialTargetMarkedPlanets()) {
            planet.SetPlanetState(previousStates.GetValueOrDefault(planet, EPlanetState.NONE));
        }
    }
}