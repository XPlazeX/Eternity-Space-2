using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageBody))]
public class BossIntroAI : MonoBehaviour
{
    public const string boss_events_data_collection = "BossEvents";

    [SerializeField] private bool _damageBody1HP;
    [SerializeField] private bool _victoryOnDefeat;
    [Header("INTRO")]
    [SerializeField] private bool _useCameraIntro;
    [SerializeField] private float _introDuration;
    [SerializeField] private Vector2 _cameraOffset;
    [SerializeField] private float _customDumping;
    [SerializeField] private SoundObject _introOST;
    [SerializeField] private string _introDialog;
    [Header("MAIN LOOP")]
    [SerializeField] private bool _startAttackModules;
    [SerializeField] private AttackModule[] _startingAttackModules;
    [SerializeField] private bool _startEnemyAI;
    [SerializeField] private SoundObject _themeOST;
    [Header("OUTRO")]
    [SerializeField] private bool _useOutro;
    [SerializeField] private float _outroDuration;
    [SerializeField] private SoundObject _outroOST;
    [SerializeField] private string _outroDialog;

    private InteriorSoundController _interiorSoundController;

    private void OnEnable() 
    {
        if (!_damageBody1HP)
            GetComponent<DamageBody>().Deathed += OnDeathed;
        else
            GetComponent<DamageBody1HP>().Deathed += OnDeathed;
    }

    private void Start() 
    {
        _interiorSoundController = GameObject.FindWithTag("AudioCore").GetComponent<InteriorSoundController>();

        if (_useCameraIntro && !GameSessionInfoHandler.ExistDataCollection(boss_events_data_collection))
        {
            StartCoroutine(Intro());
            GameSessionInfoHandler.AddDataCollection(boss_events_data_collection, new List<int>());
        } else
        {
            StartMainLoop();
        }
    }

    public void StartIntro()
    {
        StopAllCoroutines();
        StartCoroutine(Intro());
    }
    public void StartMainLoop()
    {
        StopAllCoroutines();
        _interiorSoundController.SetOSTSoundObject(_themeOST, 0);

        if (_startAttackModules)
        {
            for (int i = 0; i < _startingAttackModules.Length; i++)
            {
                _startingAttackModules[i].HandFire();
            }
        }
        if (_startEnemyAI)
        {
            if (GetComponent<MinibossAI>() != null)
            {
                GetComponent<MinibossAI>().StartMoving();
            } else
            {
                GetComponent<EnemyAIRoot>().StartMoving();
            }
        }
    }
    public void StartOutro()
    {
        StopAllCoroutines();
        StartCoroutine(Outro());
    }

    private void OnDeathed()
    {
        if (!Player.Alive)
            return;

        if (_victoryOnDefeat)
            SceneStatics.CharacterCore.GetComponent<VictoryHandler>().LevelVictory();

        if (_useOutro)
        {
            StartCoroutine(Outro());
        } else
        {
            StopAllCoroutines();
            _interiorSoundController.SetInteriorOST(0f);
        }
    }

    private IEnumerator Intro()
    {
        float timer = _introDuration;
        bool camBackTrigger = false;

        if (!string.IsNullOrEmpty(_introDialog))
            GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>().TriggerDialogue(_introDialog);

        yield return new WaitForSeconds(1.5f);

        _interiorSoundController.SetOSTSoundObject(_introOST, 0);

        Player.CanAttack = false;
        PlayerController.CanControl = false;
        // PlayerShipData.TryToggleInvulnerability(true);
        TimeHandler.Workable = false;

        // CameraController.ToggleCustomTarget(true, transform);
        // CameraController.SetCustomOffset(_cameraOffset);
        // CameraController.ToggleCustomDumping(true, _customDumping);

        while (timer > 0)
        {
            if (!camBackTrigger && timer < _introDuration / 3f)
            {
                // CameraController.ToggleCustomTarget(true, Player.PlayerTransform);
                // CameraController.DisableCustomOffset();
                // CameraController.ToggleCustomDumping(true, _customDumping * 2);
                camBackTrigger = true;
            }

            timer -= ESTime.unscaledDeltaTime;
            yield return null;
        }

        // CameraController.ToggleCustomTarget(false);
        // CameraController.ToggleCustomDumping(false);
        // PlayerShipData.TryToggleInvulnerability(false);
        Player.CanAttack = true;
        PlayerController.CanControl = true;
        TimeHandler.Workable = true;

        StartMainLoop();
    }

    private IEnumerator Outro()
    {
        float timer = _outroDuration;
        bool interiorTrigger = false;

        _interiorSoundController.SetOSTSoundObject(_outroOST, 0);
        ParryingHandler.ConstParry();

        if (!string.IsNullOrEmpty(_outroDialog))
            GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>().TriggerDialogue(_outroDialog);

        while (timer > 0)
        {
            if (!interiorTrigger && timer < _outroDuration / 3f)
            {
                GameObject.FindWithTag("AudioCore").GetComponent<InteriorSoundController>().SetInteriorOST(_outroDuration / 3f);
                interiorTrigger = true;
            }

            timer -= ESTime.unscaledDeltaTime;
            yield return null;
        }

    }
}
