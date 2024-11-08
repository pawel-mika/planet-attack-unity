using System;
using System.Collections.Generic;
using System.Linq;
using PlanetAttack;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    void Start()
    {
        Events.onGameStateChange.AddListener(GameStateChangeListener);
    }

    private void GameStateChangeListener(string[] evt)
    {
        if (evt.Contains(Events.evtGameStart))
        {
            GameBoardUtils.GeneratePlanets();
            GameBoardUtils.RandomizePlanetsInSpace();
            GameBoardUtils.InitializePlanetsState();
            GameBoardUtils.RandomizeStartingPlanets();
        }

        if (evt.Contains(Events.evtGameInMenu)) {
            GameBoardUtils.CleanupBoard();
        }
    }

    // Update is called once per frame
    void Update() { }

}
