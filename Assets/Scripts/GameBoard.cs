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
    private ActionsController ActionsController = GameManager.ActionsController;
    private PlanetsController PlanetsController = GameManager.PlanetsController;

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
    void Update()
    {
        if (GameController.GameState == EGameState.IN_GAME)
        {
            CheckWinCondition();
        }
    }

    private void CheckWinCondition()
    {
        if (PlanetsController.GetPlayerOwnedPlanetsCount() >= 1 && this.PlanetsController.GetEnemyOwnedPlanetsCount() == 0)
        {
            GameController.GameState = EGameState.GAME_OVER_PLAYER_WON;
            Invoke(nameof(EndGameNow), 1.5f);
        }
        else if (PlanetsController.GetEnemyOwnedPlanetsCount() >= 1 && this.PlanetsController.GetPlayerOwnedPlanetsCount() == 0)
        {
            GameController.GameState = EGameState.GAME_OVER_ENEMY_WON;
            Invoke(nameof(EndGameNow), 1.5f);
        }
    }

    void EndGameNow()
    {
        MenuCamControl.EndGame();
    }

}
