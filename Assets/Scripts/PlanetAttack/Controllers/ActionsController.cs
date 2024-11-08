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

    private static readonly Lazy<ActionsController> instance = new(() => new ActionsController());

    private ActionsController()
    {
        Debug.Log("ActionsController created");
    }

    // Public static method to access the Singleton instance.
    public static ActionsController Instance => instance.Value;

    private LinkedList<InterplanetaryAction> ActionsStack = new();

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
    public InterplanetaryAction CreateActionFromCurrentState() {
        return null;
    }
}