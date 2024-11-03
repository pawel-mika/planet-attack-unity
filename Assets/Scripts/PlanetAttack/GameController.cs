using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using PlanetAttack.Enums;
using PlanetAttack.ThePlanet;
using UnityEditor;
using UnityEngine;

public static partial class GameController
{

    // public Gamelevel = {easy: { bonusRangeMin: 1, bonusRangeMax: 3}, ..., brainiac: { bonusRangeMin: 0.25, bonusRangeMax: 1.5}};

    public static bool isDragging = false;


    private static EGameState mGameState = EGameState.UNKNOWN;
    public static EGameState GameState
    {
        get { return mGameState; }
        set
        {
            mGameState = value;
            Debug.Log(String.Format("Game state set to: {0}", Enum.GetName(typeof(EGameState), mGameState)));
        }
    }

}
