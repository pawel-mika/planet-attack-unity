using System;
using Unity.VisualScripting;
using UnityEngine;
using Input = UnityEngine.Input;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MenuCamControl : MonoBehaviour
{
    public Transform currentMount;
    public float speedFactor = 0.0f;

    private bool travelling = false;

    // Start is called before the first frame update
    void Start()
    {
        GameController.GameState = GameController.EGameState.IN_MENU;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMount)
        {
            transform.position = Vector3.Lerp(transform.position, currentMount.position, speedFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, currentMount.rotation, speedFactor);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject go = GameObject.FindGameObjectWithTag("MainMenu");
            SetTransform(go.transform);
            Events.onGameStateChange.Invoke(new string[] { Events.evtGameEnd });
            
            GameController.GameState = GameController.EGameState.TRANSITION_TO_MENU;
        }

        if (travelling && Vector3.Distance(transform.position, currentMount.position) <= 0.005){
            Debug.Log(String.Format("Target reached: {0}", currentMount.name));
            if(GameController.GameState == GameController.EGameState.TRANSITION_TO_MENU) {
                GameController.GameState = GameController.EGameState.IN_MENU;
                Events.onGameStateChange.Invoke(new string[] {Events.evtGameInMenu});
            } else if(GameController.GameState == GameController.EGameState.TRANSITION_TO_GAME) {
                GameController.GameState = GameController.EGameState.IN_GAME;
                Events.onGameStateChange.Invoke(new string[] {Events.evtGameInGame});
            }
            travelling = false;
        }
    }

    public void SetTransform(Transform transform)
    {
        currentMount = transform;
        travelling = true;
        if (currentMount.gameObject.name.ContainsInsensitive("P_BoardGuide"))
        {
            Events.onGameStateChange.Invoke(new string[] { Events.evtGameStart });
            GameController.GameState = GameController.EGameState.TRANSITION_TO_GAME;
        }
    }
}
