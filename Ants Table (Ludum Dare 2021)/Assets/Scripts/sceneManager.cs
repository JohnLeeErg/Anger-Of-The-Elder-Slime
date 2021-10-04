using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    static public int currentLevel = 0;

    [SerializeField] string[] levelScenes;
    [SerializeField] string frontPage = "FrontPage";
    [SerializeField] string instructionPage = "Instructions";
    [SerializeField] string endingSceneGood = "GoodEnding";
    [SerializeField] string endingSceneBad = "BadEnding";
    [SerializeField] string levelSelect = "LevelSelect";

    [SerializeField] bool goToEnding = false;

    public static sceneManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            print("duplicate rage managers!!");

        }
    }

    private void Update()
    {
        if (Input.GetButtonUp("Cancel"))
        {
            if (SceneManager.GetActiveScene().name == frontPage)
            {
                print("quitting");
                Application.Quit();
            }
            else
            {
                SceneManager.LoadScene(frontPage, LoadSceneMode.Single);

            }
        }
    }

    public void StartGame() {
        SceneManager.LoadScene(levelScenes[0], LoadSceneMode.Single);
    }

    public void loadNextLevel() {
        if (levelScenes.Length-1 > currentLevel && currentLevel > -1)
        {
            currentLevel++;
            print("current level:" + currentLevel);
            SceneManager.LoadScene(levelScenes[currentLevel], LoadSceneMode.Single);
        }
        else {
            EndGameGood();
        }
    }

    public void ReplayLevel() {
        if (levelScenes.Length - 1 > currentLevel && currentLevel > -1)
        {
            print("current level:" + currentLevel);
            SceneManager.LoadScene(levelScenes[currentLevel], LoadSceneMode.Single);
        }
    }

    public void PlayLevel(int level)
    {
        if (levelScenes.Length - 1 >= level && level > -1)
        {
            print("current level:" + level);
            currentLevel = level;
            SceneManager.LoadScene(levelScenes[level], LoadSceneMode.Single);
        }
        else {
            print("there is no level: "+level);
        }
    }

    public void Instructions() {
        SceneManager.LoadScene(instructionPage, LoadSceneMode.Single);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void EndGameGood()
    {
        if(goToEnding)
            SceneManager.LoadScene(endingSceneGood, LoadSceneMode.Single);
    }

    public void EndGameBad()
    {
        if (goToEnding)
            SceneManager.LoadScene(endingSceneBad, LoadSceneMode.Single);
    }

    public void TitleScreen()
    {
        SceneManager.LoadScene(frontPage, LoadSceneMode.Single);
    }

    public bool OnLastLevel() {
        return (levelScenes.Length - 1 == currentLevel);
    }

    public void LevelSelect()
    {
        SceneManager.LoadScene(levelSelect, LoadSceneMode.Single);
    }
}
