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

}