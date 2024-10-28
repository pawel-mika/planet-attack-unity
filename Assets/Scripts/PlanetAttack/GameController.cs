using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class GameController
{
    public enum EGameState
    {
        IN_MENU,
        IN_GAME,
        PAUSED,
        GAME_OVER,
        TRANSITION_TO_MENU,
        TRANSITION_TO_GAME,
        UNKNOWN
    }

    // public Gamelevel = {easy: { bonusRangeMin: 1, bonusRangeMax: 3}, ..., brainiac: { bonusRangeMin: 0.25, bonusRangeMax: 1.5}};

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
