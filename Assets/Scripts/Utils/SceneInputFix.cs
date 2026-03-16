using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class SceneSetupFix : MonoBehaviour
{
    void Awake()
    {
        // Ensure the camera has a PhysicsRaycaster for 3D object interaction (OnMouseDown/Up)
        if (GetComponent<PhysicsRaycaster>() == null)
        {
            gameObject.AddComponent<PhysicsRaycaster>();
        }

        // Handle Audio Listener conflicts or absence
        HandleAudioListener();
    }

    void Start()
    {
        // Handle EventSystem conflicts or absence
        HandleEventSystem();
    }

    private void HandleAudioListener()
    {
        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);

        if (listeners.Length == 0)
        {
            // If testing standalone, add a listener to this camera
            gameObject.AddComponent<AudioListener>();
            Debug.Log("No AudioListener found. Added one to the camera for testing.");
        }
        else if (listeners.Length > 1)
        {
            // If Master scene already provides a listener, remove this one to avoid log warnings
            // This assumes the Master listener is the primary one
            AudioListener localListener = GetComponent<AudioListener>();
            if (localListener != null)
            {
                Destroy(localListener);
                Debug.Log("Duplicate AudioListener removed. Using the one from the Master scene.");
            }
        }
    }

    private void HandleEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            // Create a temporary EventSystem if it doesn't exist (useful for standalone scene testing)
            Debug.Log("No EventSystem found. Creating temporary one for testing.");
            GameObject es = new GameObject("Temp_EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }
}