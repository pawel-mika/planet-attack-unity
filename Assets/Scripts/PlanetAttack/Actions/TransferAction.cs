using System.Diagnostics;
using PlanetAttack.ThePlanet;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TransferAction : InterplanetaryAction
{
    private float food = 0;
    private float minerals = 0;
    private float ships = 0;

    public TransferAction(MainPlanet srcPlanet, MainPlanet dstPlanet, int speed = 1) : base(srcPlanet, dstPlanet, speed)
    {
        Debug.Log(string.Format("TransferAction from {0} to {1}", srcPlanet.name, dstPlanet.name));
        food = srcPlanet.Food / 2;
        minerals = srcPlanet.Minerals / 2;
        ships = srcPlanet.Ships / 2;
        srcPlanet.Food -= food;
        srcPlanet.Minerals -= minerals;
        srcPlanet.Ships -= ships;
        Debug.Log(string.Format("Transfer of m: {0}, f: {1}, s: {2} at dist: {3} in {4}ms", minerals, food, ships, distance, actionLength));
    }

    public override void FinalizeAction()
    {
        dstPlanet.Ships += ships;
        dstPlanet.Minerals += minerals;
        dstPlanet.Food += food;
    }
}