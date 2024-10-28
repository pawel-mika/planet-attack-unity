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
        GameBoardUtils.GeneratePlanets();

        Events.onGameStateChange.AddListener(GameStateChangeListener);
    }

    private void GameStateChangeListener(string[] evt)
    {
        if (evt.Contains(Events.evtGameStart))
        {
            GameBoardUtils.RandomizePlanetsInSpace();
            GameBoardUtils.InitializePlanetsState();
        }
    }

    // Update is called once per frame
    void Update() { }

}
