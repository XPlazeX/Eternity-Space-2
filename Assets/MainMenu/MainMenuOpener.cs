using UnityEngine;
using System.Collections;

public class MainMenuOpener : MonoBehaviour
{
    // private void Start() {
    //     GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>().LoadMenuDialogue(GameSessionInfoHandler.GetSessionSave().LocationID);
    // }
    // [SerializeField] private float _settingShakePower;

    // private Transform _cameraTransform;
    // private IEnumerator _shakeIE;
    // // Start is called before the first frame update
    // void OnEnable()
    // {
    //     PlayerPrefs.SetInt("TargetGameMode", 0);

    //     Animator transitionAnimator = GameObject.FindWithTag("SceneTransitionScreen").GetComponent<Animator>();
    //     if (transitionAnimator == null)
    //         throw new System.Exception("Не найден аниматор перехода сцены");

    //     transitionAnimator.SetTrigger("SceneOpens"); 
    //     _shakeIE = Shaking();

    //     _cameraTransform = Camera.main.transform;

    //     // if (SceneStatics.CoresFinded)
    //     //     InitBG();
    //     // else
    //     //     SceneStatics.CoresLoaded += InitBG;
    //     //StartShake();
    // }

    // // private void InitBG()
    // // {
    // //     if (GetComponent<LevelLoader>())
    // //         GetComponent<LevelLoader>().Initialize(true);
    // // }

    // public void StartShake()
    // {
    //     StartCoroutine(_shakeIE);
    // }

    // public void StopShake()
    // {
    //     StopCoroutine(_shakeIE);
    // }

    // private IEnumerator Shaking()
    // {
    //     float totalPower = 0.1f * _settingShakePower;
    //     while (true)
    //     {
    //         _cameraTransform.position += new Vector3(Random.Range(-totalPower, totalPower), 0, 0f);

    //         yield return null;
    //     }
    // }

}
