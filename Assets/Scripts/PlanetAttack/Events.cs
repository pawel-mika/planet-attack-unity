using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Events
{
    public static readonly string evtGameStart = "evtGameStart";
    public static readonly string evtGamePause = "evtGamePause";
    public static readonly string evtGameEnd = "evtGameEnd";
    public static readonly UnityEvent<string[]> onGameStateChange = new();
}
