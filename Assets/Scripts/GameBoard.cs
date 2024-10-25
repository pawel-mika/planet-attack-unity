using System;
using System.Collections.Generic;
using System.Linq;
using PlanetAttack;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    Color inMenuColor = new(1, 1, 1, 1);
    Color inGameColor = new(1, 1, 1, 0.25f);
    Color bgColorTo = new(1, 1, 1, 1);

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
        IEnumerable<Image> bg = GameObject.FindObjectsOfType<Image>().Where((o) => o.name.Contains("BackgroundImage"));
        if(bg.Count() > 0) {
            Image img = bg.ToArray()[0];
            img.material.color = Color.LerpUnclamped(img.material.color, bgColorTo, 0.01f);
        }
    }

}
