using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public delegate void sceneTransitionOperation();
    public delegate void sceneOperation();
    public static event sceneTransitionOperation SceneTransit;
    public static event sceneOperation SceneClosing;
    public static event sceneOperation SceneOpened;
    public static event sceneOperation SceneRestarted;

    private static Animator _animator;
    private static AsyncOperation _loadingSceneOperation;
    public static bool SceneReady {get; private set;} = false;
    private static bool TimeResetAnimation {get; set;} = false;
    //private static AsyncOperation _unloadingSceneOperation;

    private static SceneTransition _instance = null;
    private static string _lastSceneName;

    public static string ActiveSceneName => SceneManager.GetActiveScene().name;

    private void Awake() {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else 
            Destroy(gameObject); 

        if (SceneManager.GetActiveScene().name != "Game" && SceneManager.GetActiveScene().name != "MissionMenu")
        {
            SceneStatics.CoresLoaded += SceneLoaded;
            
            if (SceneStatics.CoresFinded)
                SceneLoaded();
        } 
    }

    public static void SceneLoaded()
    {
        TimeResetAnimation = false;

        if (SceneManager.GetActiveScene().name != "Game" && SceneManager.GetActiveScene().name != "MissionMenu")
        {
            SceneStatics.CoresLoaded -= SceneLoaded;
            //SceneLoaded();
        }
        Debug.Log("!!!---Сцена загружена---!!!");
        GameObject.FindWithTag("SceneTransitionScreen").GetComponent<LoadingCaller>().OpenMask();
        SceneOpened?.Invoke();
        // TimeHandler.Resume(1f);
        SceneReady = true;

        if (_lastSceneName == SceneManager.GetActiveScene().name)
        {
            SceneRestarted?.Invoke();
        }
    }

    public static void BlockUI()
    {
        GameObject.FindWithTag("SceneTransitionScreen").GetComponent<UnityEngine.UI.Image>().raycastTarget = true;
    }

    public static void ReloadScene()
    {
        SwitchToScene(SceneManager.GetActiveScene().name);
    }

    public static void SwitchToScene(string name, int codeReason = -1)
    {
        if (codeReason == 4)
        {
            TimeResetAnimation = true;
        }

        print($"LoadScene : {name}");
        SceneLocalizator.Reload();
        // TimeHandler.Resume(1f);
        if (SceneManager.GetActiveScene().name == "Game")
        {
            PlayerShipData.DeactivateAllBindedSystems();
        }
        
        Animator transitionAnimator = GameObject.FindWithTag("SceneTransitionScreen").GetComponent<Animator>();
        if (transitionAnimator == null)
            throw new System.Exception("Не найден аниматор перехода сцены");

        transitionAnimator.SetTrigger("SceneCloses");

        if (TimeResetAnimation)
        {
            GameObject.FindWithTag("SceneTransitionScreen").GetComponent<LoadingCaller>().TimebackAnimation();
        }

        _loadingSceneOperation = SceneManager.LoadSceneAsync(name);
        _loadingSceneOperation.allowSceneActivation = false;

        SceneTransit?.Invoke();
        _lastSceneName = SceneManager.GetActiveScene().name;
    }

    public static void OpenRelevantLobbyScene()
    {
        if (GameSessionInfoHandler.GetSessionSave().SessionInitialized)
        {
            SceneTransition.SwitchToScene("MissionMenu");
        } else 
        {
            SceneTransition.SwitchToScene("Lobby");
        }
    }

    public static void OnAnimationOver()
    {
        SceneReady = false;
        SceneClosing?.Invoke();
        
        if (!TimeResetAnimation)
            _loadingSceneOperation.allowSceneActivation = true;
    }

    public static void OnTimeResetAnimationOver()
    {
        TimeResetAnimation = false;
        _loadingSceneOperation.allowSceneActivation = true;
    }
    
}
