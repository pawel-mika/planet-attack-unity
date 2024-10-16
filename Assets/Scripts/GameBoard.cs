using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using PlanetAttack;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class GameBoard : MonoBehaviour
{
    float rotatePlanetsInterval = 0.025f;
    float nextTime = 0f;

    Color inMenuColor = new Color(1, 1, 1, 1);
    Color inGameColor = new Color(1, 1, 1, 0.5f);
    Color bgColorTo = new Color(1, 1, 1, 1);

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
            bgColorTo = inGameColor;
        }
        if(evt.Contains(Events.evtGameEnd)) {
            bgColorTo = inMenuColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextTime) {
            RotatePlanets();
            nextTime += rotatePlanetsInterval;
        }

        IEnumerable<Image> bg = GameObject.FindObjectsOfType<Image>().Where((o) => o.name.Contains("BackgroundImage"));
        if(bg.Count() > 0) {
            Image img = bg.ToArray()[0];
            img.material.color = Color.LerpUnclamped(img.material.color, bgColorTo, 0.01f);
        }
    }

    private void RotatePlanets()
    {
        var objects = PlanetUtils.GetAllPlanets();
        foreach(GameObject o in objects) {
            o.transform.Rotate(Vector3.up, 2);
        }
    }

}
