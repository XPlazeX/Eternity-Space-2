using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public delegate void sceneTransitionOperation();
    public delegate void sceneOperation();
    public static event sceneTransitionOperation SceneTransit;
    public static event sceneOperation SceneClosing;
    public static event sceneOperation SceneOpened;

    private static Animator _animator;
    private static AsyncOperation _loadingSceneOperation;
    public static bool SceneReady {get; private set;} = false;
    //private static AsyncOperation _unloadingSceneOperation;

    private static SceneTransition _instance = null;

    public static string ActiveSceneName => SceneManager.GetActiveScene().name;

    private void Awake() {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else 
            Destroy(gameObject); 

        if (SceneManager.GetActiveScene().name != "Game")
        {
            SceneStatics.CoresLoaded += SceneLoaded;
            
            if (SceneStatics.CoresFinded)
                SceneLoaded();
            //SceneLoaded();
        } 
    }

    public static void SceneLoaded()
    {
        if (SceneManager.GetActiveScene().name != "Game")
        {
            SceneStatics.CoresLoaded -= SceneLoaded;
            //SceneLoaded();
        }
        Debug.Log("!!!---Сцена загружена---!!!");
        GameObject.FindWithTag("SceneTransitionScreen").GetComponent<LoadingCaller>().OpenMask();
        SceneOpened?.Invoke();
        SceneReady = true;
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
        print($"LoadScene : {name}");
        SceneLocalizator.Reload();
        TimeHandler.Resume(1f);
        if (SceneManager.GetActiveScene().name == "Game")
        {
            PlayerShipData.DeactivateAllBindedSystems();
        }
        
        Animator transitionAnimator = GameObject.FindWithTag("SceneTransitionScreen").GetComponent<Animator>();
        if (transitionAnimator == null)
            throw new System.Exception("Не найден аниматор перехода сцены");

        transitionAnimator.SetTrigger("SceneCloses");

        _loadingSceneOperation = SceneManager.LoadSceneAsync(name);
        _loadingSceneOperation.allowSceneActivation = false;

        SceneTransit?.Invoke();
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
        //shouldPlayAnim = true;
        _loadingSceneOperation.allowSceneActivation = true;
    }
    
}
