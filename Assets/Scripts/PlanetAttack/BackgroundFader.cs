using System;
using System.Collections.Generic;
using System.Linq;
using PlanetAttack;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundFader : MonoBehaviour
{
    Color inMenuColor = new(1, 1, 1, 1);
    Color inGameColor = new(1, 1, 1, 0.25f);
    Color bgColorTo = new(1, 1, 1, 1);

    public float speedFactor = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Events.onGameStateChange.AddListener(GameStateChangeListener);
    }

    private void GameStateChangeListener(string[] evt)
    {
        if (evt.Contains(Events.evtGameStart))
        {
            bgColorTo = inGameColor;
        }
        if (evt.Contains(Events.evtGameEnd))
        {
            bgColorTo = inMenuColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        IEnumerable<Image> bg = FindObjectsByType<Image>(FindObjectsSortMode.None).Where((o) => o.name.Contains("BackgroundImage"));
        if (bg.Count() > 0)
        {
            Image img = bg.ToArray()[0];
            img.material.color = Color.LerpUnclamped(img.material.color, bgColorTo, speedFactor);
        }
    }
}
