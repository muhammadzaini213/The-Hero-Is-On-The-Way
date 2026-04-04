using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    public void Retry()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}