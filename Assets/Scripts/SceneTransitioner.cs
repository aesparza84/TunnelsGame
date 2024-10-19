using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitioner : MonoBehaviour
{
    [SerializeField] private Image _FadeBlackImage;
    [SerializeField] private float FadeSpeed;
    [SerializeField] private Color FadeColor;

    public static SceneTransitioner _instance;

    private bool GameSceneFadeIn;
    private bool CoroFading;
    private void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
        }

        MainMenu.OnMainGameEnter += OnLoadGameScene;

        SceneManager.activeSceneChanged += OnSceneChanged;

        PauseMenu.OnLeaveGame += ReturnToMain;
        DontDestroyOnLoad(gameObject);

        GameSceneFadeIn = false;
    }

    private void ReturnToMain(object sender, System.EventArgs e)
    {
        CallMainMenu();
    }

    private void OnSceneChanged(Scene arg0, Scene arg1)
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            //Fade in from Update Loop

            MainMenu.OnMainGameEnter -= OnLoadGameScene;
            FadeColor.a = 1;
            _FadeBlackImage.color = FadeColor;

            GameSceneFadeIn = true;
        }
        else
        {
            MainMenu.OnMainGameEnter += OnLoadGameScene;

        }
    }
    private void OnLoadGameScene(object sender, System.EventArgs e)
    {
        StartCoroutine(LoadSceneTransitionTo(1));
    }

   
    public void CallMainMenu()
    {
        StartCoroutine(LoadSceneTransitionTo(0));
    }

    private IEnumerator LoadSceneTransitionTo(int sceneIndex)
    {
        GameSceneFadeIn = false;
        CoroFading = true;
        FadeColor.a = 0.0f;
        _FadeBlackImage.color = FadeColor;

        while (_FadeBlackImage.color.a < 1)
        {
            FadeColor.a = Mathf.MoveTowards(FadeColor.a, 1.0f, FadeSpeed * Time.unscaledDeltaTime);
            _FadeBlackImage.color = FadeColor;
            yield return null;
        }

        var nextScene = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);

        while (!nextScene.isDone)
        {
            yield return null;
        }

        CoroFading = false;
        GameSceneFadeIn = true;
        yield return null;
    }

    private void Update()
    {
        if (CoroFading)
        {
            return;
        }

        if (GameSceneFadeIn)
        {
            if (_FadeBlackImage.color.a > 0)
            {
                FadeColor.a = Mathf.Lerp(FadeColor.a, 0, FadeSpeed * Time.deltaTime);
                _FadeBlackImage.color = FadeColor;
            }
            else
            {
                GameSceneFadeIn = false;
            }
        }
    }


    private void OnDisable()
    {
        MainMenu.OnMainGameEnter -= OnLoadGameScene;

        SceneManager.activeSceneChanged -= OnSceneChanged;
        PauseMenu.OnLeaveGame -= ReturnToMain;
    }
}
