using Unity.VisualScripting;
using UnityEngine;
using Input = UnityEngine.Input;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MenuCamControl : MonoBehaviour
{
    public Transform currentMount;
    public float speedFactor = 0.01f;

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update()
    {
        if(currentMount) {
            transform.position = Vector3.Lerp(transform.position, currentMount.position, speedFactor);
            transform.rotation = Quaternion.Slerp(transform.rotation, currentMount.rotation, speedFactor);
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            GameObject go = GameObject.FindGameObjectWithTag("MainMenu");
            SetTransform(go.transform);
            Events.onGameStateChange.Invoke(new string[]{Events.evtGameEnd});
        }
    }

    public void SetTransform(Transform transform) {
        currentMount = transform;
        if(currentMount.gameObject.name.ContainsInsensitive("P_BoardGuide")) {
            Events.onGameStateChange.Invoke(new string[]{Events.evtGameStart});
        }
    }
}
