using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneManager : MonoBehaviour
{
    private int currentSceneIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		currentSceneIndex = 0;
		DontDestroyOnLoad(gameObject);
	}

    public void LoadNextScene()
    {
        currentSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCount;
        SceneManager.LoadScene(currentSceneIndex);
    }

	public void LoadMainMenu()
	{
        currentSceneIndex = 0;
		SceneManager.LoadScene(0);
	}

    public void StartGame()
    {
        currentSceneIndex = 1;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void ExitGame()
    {
        Debug.Log("Exit the game now!");
        Application.Quit();
    }
}
