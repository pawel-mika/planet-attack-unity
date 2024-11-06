using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using PlanetAttack.Enums;
using PlanetAttack.ThePlanet;
using UnityEditor;
using UnityEngine;

public class GameController
{
    // public Gamelevel = {easy: { bonusRangeMin: 1, bonusRangeMax: 3}, ..., brainiac: { bonusRangeMin: 0.25, bonusRangeMax: 1.5}};
    private static readonly Lazy<GameController> instance = new(() => new GameController());

    private GameController()
    {
        Debug.Log("GameController created");
    }

    // Public static method to access the Singleton instance.
    public static GameController Instance => instance.Value;

    public bool isDragging = false;

    private EGameState mGameState = EGameState.UNKNOWN;
    public EGameState GameState
    {
        get { return mGameState; }
        set
        {
            mGameState = value;
            Debug.Log(String.Format("Game state set to: {0}", Enum.GetName(typeof(EGameState), mGameState)));
        }
    }

}