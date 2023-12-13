using UnityEngine;
using System.Collections;
 
public class MobileUtilsScript : MonoBehaviour {
 
    private int FramesPerSec;
    private float frequency = 1.0f;
    private string fps = "N/A";
    [SerializeField] private UnlockRequire _checkingUnlock;
    [SerializeField] private GUIStyle _guiStyle;
    [SerializeField] private ScriptableObject[] _preloads;

    //private
    private GUIStyle _largeFont;
    private bool _work = false;
 
    void Awake()
    {
        Application.targetFrameRate = Mathf.RoundToInt(PlayerPrefs.GetFloat("TargetFPS", 60));
        Time.fixedDeltaTime = PlayerPrefs.GetFloat("FixedUpdateStep", 1f / 60f);
        
        if (PlayerPrefs.GetFloat("FPSmeter", 0) == 0)
            return;

        _work = true;

        StartCoroutine(FPS());

        _largeFont = _guiStyle;
    }

    #if UNITY_EDITOR

    private void Start() {
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
 
            fps = string.Format("FPS: {0} \nFDT: {1} \nDT: {2}" , Mathf.RoundToInt(frameCount / timeSpan), Mathf.Round(Time.fixedDeltaTime * 10000f)/10000f, Mathf.Round(Time.deltaTime* 10000f)/10000f);
        }
    }
 
 
    void OnGUI(){
        if (!_work)
            return;
            
        GUI.Label(new Rect(Screen.width - 170,45,210,150), fps, _largeFont);
    }
}