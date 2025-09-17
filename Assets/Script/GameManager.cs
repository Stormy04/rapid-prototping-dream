using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Scenes")]
    public string[] scenes = { "Dream1", "Dream2", "Dream3" };
    public string loseScene = "LoseScene";
    public string winScene = "WinScene";

    [Header("Timer Settings")]
    public TMP_Text timerTextPrefab; // assign TMP prefab in inspector
    private TMP_Text timerTextInstance;
    public float startingTime = 60f;
    private float currentTime;
    private bool timerRunning = false;

    [Header("UI Message")]
    public TMP_Text messageTextPrefab; // assign TMP prefab in inspector for messages
    private TMP_Text messageTextInstance;
    private Coroutine messageCoroutine;

    [Header("Exit Tracking")]
    private bool[] exitsFound = new bool[3];

    private int currentSceneIndex = 0;
    private int loopCount = 0;
    private bool[] itemFound = new bool[3];
    private Canvas persistentCanvas;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Create persistent Canvas
            GameObject canvasGO = new GameObject("PersistentCanvas");
            persistentCanvas = canvasGO.AddComponent<Canvas>();
            persistentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasGO);

            // Instantiate timer text as child of canvas
            if (timerTextPrefab != null)
            {
                timerTextInstance = Instantiate(timerTextPrefab, persistentCanvas.transform);

                // Anchor to top-right
                RectTransform rect = timerTextInstance.rectTransform;
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                rect.anchoredPosition = new Vector2(-10, -10); // 10px from top-right

                DontDestroyOnLoad(timerTextInstance.gameObject);
            }

            // Instantiate message text as child of canvas
            if (messageTextPrefab != null)
            {
                messageTextInstance = Instantiate(messageTextPrefab, persistentCanvas.transform);
                RectTransform msgRect = messageTextInstance.rectTransform;
                msgRect.anchorMin = new Vector2(0.5f, 0.1f);
                msgRect.anchorMax = new Vector2(0.5f, 0.1f);
                msgRect.pivot = new Vector2(0.5f, 0.5f);
                msgRect.anchoredPosition = Vector2.zero;
                messageTextInstance.text = "";
                messageTextInstance.gameObject.SetActive(false);
                DontDestroyOnLoad(messageTextInstance.gameObject);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartScene(currentSceneIndex);
    }

    private void Update()
    {
        if (timerRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                UpdateTimerUI();
            }
            else
            {
                timerRunning = false;
                OnTimerEnd();
            }
        }
    }

    private void UpdateTimerUI()
    {
        if (timerTextInstance != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerTextInstance.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void PlayerFoundExit()
    {
        if (!itemFound[currentSceneIndex])
        {
            ShowMessage("This door is locked.", 2f);
            Debug.Log("You must find the item before exiting!");
            return;
        }

        exitsFound[currentSceneIndex] = true;

        // Check win condition: all items and all exits found
        bool allItemsFound = true;
        bool allExitsFound = true;
        for (int i = 0; i < scenes.Length; i++)
        {
            if (!itemFound[i]) allItemsFound = false;
            if (!exitsFound[i]) allExitsFound = false;
        }

        if (allItemsFound && allExitsFound)
        {
            SceneManager.LoadScene(winScene);
        }
        else
        {
            NextScene();
        }
    }

    private void OnTimerEnd()
    {
        if (loopCount == 2 && currentSceneIndex == scenes.Length - 1)
        {
            // Last scene of last round, player loses
            SceneManager.LoadScene(loseScene);
        }
        else
        {
            NextScene();
        }
    }

    private void NextScene()
    {
        currentSceneIndex++;

        if (currentSceneIndex >= scenes.Length)
        {
            currentSceneIndex = 0;
            loopCount++; // Move to next round after all scenes played
        }

        // Set timer based on round
        if (loopCount == 0)
            currentTime = 60f;
        else if (loopCount == 1)
            currentTime = 30f;
        else
            currentTime = 15f;

        timerRunning = true;
        SceneManager.LoadScene(scenes[currentSceneIndex]);
    }

    private void StartScene(int index)
    {
        currentSceneIndex = index;
        loopCount = 0;
        currentTime = 60f;
        timerRunning = true;

        // Reset tracking for new game
        for (int i = 0; i < itemFound.Length; i++)
        {
            itemFound[i] = false;
            exitsFound[i] = false;
        }

        SceneManager.LoadScene(scenes[currentSceneIndex]);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Optional: scene-specific logic
    }

    public void PlayerFoundItem()
    {
        itemFound[currentSceneIndex] = true;
        ShowMessage("I found a key.", 2f);
    }

    // Show a message for a set duration
    public void ShowMessage(string message, float duration = 2f)
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);
        messageCoroutine = StartCoroutine(ShowMessageCoroutine(message, duration));
    }

    private IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        if (messageTextInstance != null)
        {
            messageTextInstance.text = message;
            messageTextInstance.gameObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            messageTextInstance.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        StartScene(0); // or your preferred starting logic
    }
}
