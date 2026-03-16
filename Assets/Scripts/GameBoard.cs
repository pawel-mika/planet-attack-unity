using System;
using System.Linq;
using PlanetAttack;
using PlanetAttack.Enums;
using Unity.VisualScripting;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    private GameController gameController = GameManager.GameController;
    private ActionsController actionsController = GameManager.ActionsController;
    private PlanetsController planetsController = GameManager.PlanetsController;

    void Start()
    {
        Events.onGameStateChange.AddListener(GameStateChangeListener);
    }

    private void GameStateChangeListener(string[] evt)
    {
        if (evt.Contains(Events.evtGameStart))
        {
            if (gameController.GameState == EGameState.TRANSITION_TO_MENU)
            {
                // prevent error of starting while still in transit
                GameManager.EndGame();
            }

            GameManager.StartGame();
        }

        // if (evt.Contains(Events.evtGameInGame))
        // {
        // GameBoardUtils.GeneratePlanets();
        // GameBoardUtils.RandomizePlanetsInSpace();
        // GameBoardUtils.InitializePlanetsState();
        // GameBoardUtils.RandomizeStartingPlanets();
        // }

        if (evt.Contains(Events.evtGameInMenu))
        {
            GameManager.EndGame();
        }

        if(evt.Contains(Events.evtGameEnd))
        {
            GameManager.EndGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.GameState == EGameState.IN_GAME)
        {
            CheckWinCondition();
        }
    }

    private void CheckWinCondition()
    {
        if (planetsController.GetPlayerOwnedPlanetsCount() >= 1 && this.planetsController.GetEnemyOwnedPlanetsCount() == 0)
        {
            gameController.GameState = EGameState.GAME_OVER_PLAYER_WON;
            Invoke(nameof(EndGameNow), 1.5f);
        }
        else if (planetsController.GetEnemyOwnedPlanetsCount() >= 1 && this.planetsController.GetPlayerOwnedPlanetsCount() == 0)
        {
            gameController.GameState = EGameState.GAME_OVER_ENEMY_WON;
            Invoke(nameof(EndGameNow), 1.5f);
        }
    }

    void EndGameNow()
    {
        MenuCamControl.EndGame();
    }

}
