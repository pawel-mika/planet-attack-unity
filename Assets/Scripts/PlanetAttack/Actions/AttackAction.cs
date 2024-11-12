using System;
using System.Diagnostics;
using PlanetAttack.ThePlanet;
using Debug = UnityEngine.Debug;

public class AttackAction : InterplanetaryAction
{
    private float ships = 0;
    public AttackAction(MainPlanet srcPlanet, MainPlanet dstPlanet, float speed = 2f) : base(srcPlanet, dstPlanet, speed)
    {
        Debug.Log(string.Format("AttackAction from {0} to {1}", srcPlanet.name, dstPlanet.name));
        ships = srcPlanet.Ships / 2;
        srcPlanet.Ships -= ships;
    }

    public override void FinalizeAction()
    {
        if (dstPlanet.PlanetOwner != PlanetAttack.Enums.EPlayerType.PLAYER)
        {
            dstPlanet.Ships -= ships;
        }
        else
        {
            dstPlanet.Ships += ships;
        }

        if (dstPlanet.Ships < 0)
        {
            // conquered!
            dstPlanet.Ships = Math.Abs(dstPlanet.Ships);
            dstPlanet.SetPlanetOwner(srcPlanet.PlanetOwner);
            dstPlanet.SetPlanetState(PlanetAttack.Enums.EPlanetState.OWNED);
            dstPlanet.PlayExplosion();
        }
    }
}