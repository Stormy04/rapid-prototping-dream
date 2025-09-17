// File: Assets/Script/SceneLoader.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(LoadNextScene);
        else
            Debug.LogError("Start Button not assigned in SceneLoader!");
    }

    void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // Optional: Loop to first scene if at the end
        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
            nextIndex = 0;

        SceneManager.LoadScene(nextIndex);
    }
}
