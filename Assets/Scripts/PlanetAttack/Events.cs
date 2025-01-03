using UnityEngine.Events;

public class Events
{
    public static readonly string evtGameStart = "evtGameStart";
    public static readonly string evtGamePause = "evtGamePause";
    public static readonly string evtGameEnd = "evtGameEnd";
    public static readonly string evtGameInMenu = "evtGameInMenu";
    public static readonly string evtGameInGame = "evtGameInGame";
    public static readonly UnityEvent<string[]> onGameStateChange = new();
}
