using UnityEngine;
using System.Collections;
 
public class MobileUtilsScript : MonoBehaviour {
 
    private int FramesPerSec;
    private float frequency = 1.0f;
    private string fps = "N/A";
    [SerializeField] private bool _checkUnlockMode;
    [SerializeField] private UnlockRequire _checkingUnlock;
    [SerializeField] private bool _deleteUnlockMode;
    [SerializeField] private int[] _deletingUnlocks;
    [SerializeField] private float _freeCamSpeed;
    [SerializeField] private GUIStyle _guiStyle;
    //private
    private GUIStyle _largeFont;
    private bool _work = false;
 
    void Awake()
    {
        // Application.targetFrameRate = Mathf.RoundToInt(PlayerPrefs.GetFloat("TargetFPS", 60));
        // ESTime.worldFixedDeltaTime = PlayerPrefs.GetFloat("FixedUpdateStep", 1f / 60f);
        
        if (PlayerPrefs.GetFloat("FPSmeter", 0) == 0)
            return;

        _work = true;

        StartCoroutine(FPS());

        _largeFont = _guiStyle;
    }

    #if UNITY_EDITOR

    private GameObject _canvas;
    private bool _paused;
    private Transform _cameraTransform;
    private bool _freeCam;
    private Vector3 _targetCamPos;
    private bool _camLerp;

    private const float lerp_freecam_speed = 3f;

    private void Start() {
        if (_deleteUnlockMode)
        {
            GlobalSave gsave = GlobalSaveHandler.GetSave();
            for (int i = 0; i < _deletingUnlocks.Length; i++)
            {
                gsave.RemoveUnlock(_deletingUnlocks[i]);
            }
            GlobalSaveHandler.RewriteSave(gsave);
        }
        if (_checkUnlockMode)
        {
            Debug.Log($"<color=magenta>Unlock has status ({Unlocks.HasUnlock(_checkingUnlock)})</color>");
        }
        _cameraTransform = Camera.main.transform;
        _canvas = GameObject.Find("Canvas");
        // GlobalSave gsave = GlobalSaveHandler.GetSave();
        // gsave.RemoveUnlock(622);
        // GlobalSaveHandler.RewriteSave(gsave);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Bank.PutCash(BankSystem.Currency.Cosmilite, 500);
            print("+500 cos");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Bank.PutCash(BankSystem.Currency.Positronium, 3);
            print("+3 pos");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Unlocks.ProgressUnlock(7, 1);
            print("+1 beacon");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Bank.PutCash(BankSystem.Currency.Aurite, 20);
            print("+20 aurite");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            _paused = !_paused;
            ESTime.worldTimeScale = _paused ? 0f : 1f;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            _camLerp = !_camLerp;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _freeCam = !_freeCam;
            _canvas.SetActive(_freeCam ? false : true);
            CameraController cc = Camera.main.GetComponent<CameraController>();

            // if (cc != null)
            //     cc.CanMoving = _freeCam ? false : true;
        }
        if (_freeCam)
        {
            //print(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f));
            if (_camLerp)
            {
                _targetCamPos += new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f) * ESTime.unscaledDeltaTime * (Input.GetKey(KeyCode.LeftShift) ? 3f : 1f) * _freeCamSpeed;
                _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, new Vector3(_targetCamPos.x, _targetCamPos.y, _cameraTransform.position.z), lerp_freecam_speed * ESTime.unscaledDeltaTime);
            }else
                _cameraTransform.position += new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f) * ESTime.unscaledDeltaTime * (Input.GetKey(KeyCode.LeftShift) ? 3f : 1f) * _freeCamSpeed;
        }

    }

    private void LateUpdate() {
        
    }
    #endif
 
    private IEnumerator FPS() {
        for(;;){
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;
           
            // Display it
 
            fps = string.Format("FPS: {0} \nFDT: {1} \nDT: {2}" , Mathf.RoundToInt(frameCount / timeSpan), Mathf.Round(ESTime.worldFixedDeltaTime * 10000f)/10000f, Mathf.Round(ESTime.worldDeltaTime* 10000f)/10000f);
        }
    }
 
 
    void OnGUI(){
        if (!_work)
            return;
            
        GUI.Label(new Rect(Screen.width - 170,45,210,150), fps, _largeFont);
    }
}