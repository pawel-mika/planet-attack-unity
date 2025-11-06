using System;
using System.Collections.Generic;
using System.Linq;
using PlanetAttack;
using PlanetAttack.Enums;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    private GameController GameController = GameManager.GameController;

    void Start()
    {
        Events.onGameStateChange.AddListener(GameStateChangeListener);
    }

    private void GameStateChangeListener(string[] evt)
    {
        if (evt.Contains(Events.evtGameStart))
        {
            if (GameController.GameState == EGameState.TRANSITION_TO_MENU)
            {
                // prevent error of starting while still in transit
                GameBoardUtils.CleanupBoard();
            }

            GameBoardUtils.GeneratePlanets();
            GameBoardUtils.RandomizePlanetsInSpace();
            GameBoardUtils.InitializePlanetsState();
            GameBoardUtils.RandomizeStartingPlanets();
        }

        if (evt.Contains(Events.evtGameInMenu))
        {
            GameBoardUtils.CleanupBoard();
        }
    }

    // Update is called once per frame
    void Update() { }

}
