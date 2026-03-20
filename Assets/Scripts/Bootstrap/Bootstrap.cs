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
        loadingPanel.SetActive(true);
        _visualProgress = 0f;

        AsyncOperation op = SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);
        op.allowSceneActivation = false;

        while (_visualProgress < 0.99f) // Loop until visually almost full
        {
            float targetProgress = Mathf.Clamp01(op.progress / 0.9f);

            // Easing logic: 5f is the speed multiplier.
            // Higher value = faster initial movement.
            _visualProgress = Mathf.Lerp(_visualProgress, targetProgress, Time.deltaTime * 5f);

            if (fillImage != null) fillImage.fillAmount = _visualProgress;
            if (progressText != null) progressText.text = $"{Mathf.RoundToInt(_visualProgress * 100)}%";

            // Allow scene to activate when progress is high enough
            if (_visualProgress >= 0.95f && op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;
            }

            if (op.isDone && _visualProgress >= 0.98f) break;

            await Task.Yield();
        }

        // Final snap to 100%
        _visualProgress = 1f;
        if (fillImage != null) fillImage.fillAmount = 1f;
        if (progressText != null) progressText.text = "100%";

        // Set Active Scene logic...
        loadingPanel.SetActive(false);
    }
}