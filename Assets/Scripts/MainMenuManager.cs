using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    [SerializeField] private Animator UIAnimator;
    [SerializeField] private Animator musicAnimator;

    [SerializeField] private int gameSceneIndex = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Debug.LogError("ERROR: Found other MainMenuManager!");
    }

    public void StartGame()
    {
        UIAnimator.SetTrigger("getBlack");
        musicAnimator.SetTrigger("calmDown");

        Invoke("LoadGameScene", .5f);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void Exit()
    {
        Debug.Log("MANAGER: Quit Application!");
        Application.Quit();
    }
}
