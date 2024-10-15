using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using PlanetAttack;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

public class GameBoard : MonoBehaviour
{
    float rotatePlanetsInterval = 0.025f;
    float nextTime = 0f;


    // Start is called before the first frame update
    void Start()
    {
        GameBoardUtils.GeneratePlanets();

        Events.onGameStateChange.AddListener(GameStateChangeListener);
    }

    private void GameStateChangeListener(string[] evt)
    {
        if(evt.Contains(Events.evtGameStart)) {
            GameBoardUtils.RandomizePlanetsInSpace();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextTime) {
            RotatePlanets();
            nextTime += rotatePlanetsInterval;
        }
    }

    private void RotatePlanets()
    {
        var objects = GameUtils.GetAllPlanets();
        foreach(GameObject o in objects) {
            o.transform.Rotate(Vector3.up, 2);
        }
    }

}
