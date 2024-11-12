using System;
using System.Collections.Generic;
using System.Diagnostics;
using PlanetAttack;
using PlanetAttack.Enums;
using PlanetAttack.ThePlanet;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ActionsController
{
    PlanetsController PlanetsController = GameManager.PlanetsController;
    private static readonly Lazy<ActionsController> instance = new(() => new ActionsController());

    private ActionsController()
    {
        Debug.Log("ActionsController created");
    }

    // Public static method to access the Singleton instance.
    public static ActionsController Instance => instance.Value;

    public bool isDragging = false;

    public Vector3 mouseDownPoint;
    public Vector3 dragStartPoint;
    public Vector3 dragTargetPoint;

    /// <summary>
    /// Create an interplanetary aciton to be performed from the current state of the board:
    /// * selected planet(s)
    /// * target planet
    /// NEEDS to be called in proper moment - just after the mouse pointer has been released but before next update
    /// that will clean some board elements to their default states (?)
    /// </summary>
    public void CreateActionFromCurrentState(LinkedList<MainPlanet> sourcePlanets, MainPlanet targetPlanet) {
        Debug.Log(string.Format("Create action"));
        
        if(sourcePlanets.First == null || !sourcePlanets.First.Value || targetPlanet == null) {
            return;
        }

        if(targetPlanet.PlanetOwner == sourcePlanets.First.Value.PlanetOwner) {
            // fransfer action
            foreach(MainPlanet mp in sourcePlanets) {
                TransferAction transfer = new(mp, targetPlanet);
                CreateActionGameObject(transfer);
            }
        } else {
            // attac action
            foreach(MainPlanet mp in sourcePlanets) {
                AttackAction attack = new(mp, targetPlanet);
                CreateActionGameObject(attack);
            }
        }
    }

    private void CreateActionGameObject(InterplanetaryAction action) {
        // instantiate vs action pool? investigate later using perfrmance monitor perhaps, for now POC - just intantiate here
        ActionRenderer ar = UnityEngine.Object.Instantiate<ActionRenderer>(Resources.Load<ActionRenderer>("ActionRenderer"));
        ar.transform.position = action.GetDstPlanet().transform.position;
        ar.action = action;
    }
}