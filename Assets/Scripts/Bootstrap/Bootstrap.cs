using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class Bootstrap : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "InGame";

    [Header("UI Elements (Filled Image)")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 0.5f;

    private float _visualProgress = 0f;

    void Awake()
    {
        // Mobile devices often default to 30 FPS.
        // Force 60 to check if it was just a system limit.
        Application.targetFrameRate = 60;
    }

    private async void Start()
    {
        // Keep bootstrap alive across scene changes
        DontDestroyOnLoad(gameObject);

        // Set everything to zero at start
        if (fillImage != null) fillImage.fillAmount = 0f;
        if (progressText != null) progressText.text = "0%";

        // Start the loading process
        await LoadInitialScene();
    }

    private async Task LoadInitialScene()
    {
        Debug.Log("Starting robust loader...");
        loadingPanel.SetActive(true);
        _visualProgress = 0f;

        AsyncOperation op = SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);

        // CRITICAL: This prevents the 'InGame' scene from starting until we are ready
        op.allowSceneActivation = false;

        while (_visualProgress < 1f)
        {
            // Unity progress goes 0 -> 0.9. We normalize it.
            float targetProgress = op.progress / 0.9f;

            // Smooth progress bar movement
            _visualProgress = Mathf.MoveTowards(_visualProgress, targetProgress, Time.deltaTime * smoothSpeed);

            if (fillImage != null) fillImage.fillAmount = _visualProgress;
            if (progressText != null) progressText.text = $"Loading: {Mathf.RoundToInt(_visualProgress * 100)}%";

            // When visual bar is at 100%, we allow Unity to finish the activation
            if (_visualProgress >= 0.99f)
            {
                op.allowSceneActivation = true;
            }

            // Break loop only when scene is truly activated and done
            if (op.isDone) break;

            await Task.Yield();
        }

        // Double check if scene is loaded before setting active
        Scene loadedScene = SceneManager.GetSceneByName(gameSceneName);
        if (loadedScene.isLoaded)
        {
            SceneManager.SetActiveScene(loadedScene);
        }

        Debug.Log("Loading finished. Hiding panel.");

        // Optional: Add a small Fade Out here instead of just SetActive(false)
        // await Task.Delay(500);
        loadingPanel.SetActive(false);
    }
}