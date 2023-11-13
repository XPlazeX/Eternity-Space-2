using UnityEngine;
using UnityEngine.UI;

public class MainSettings : MonoBehaviour
{
    const string bmsg = "\n>> ";

    // [SerializeField] private Text _logLabel;
    // [SerializeField] private string[] _messages;
    // [Space()] 
    // [SerializeField] private Text _headResolutionLabel;
    // [SerializeField] private Text _resolutionLabel;

    private bool _loading = false;

    // private void Start(){
    //     if (!PlayerPrefs.HasKey("StartResolution"))
    //         PlayerPrefs.SetString("StartResolution", GetStringResolution(Screen.resolutions[0]));

    //     string msg = "";

    //     SetResolutionScale(PlayerPrefs.GetFloat("ResolutionScale", 1f));

    //     msg += (bmsg + "текущее разрешение : " + GetStringResolution(Screen.currentResolution));

    //     msg += (bmsg + "разрешение при первом запуске : \n" + PlayerPrefs.GetString("StartResolution"));

    //     // Resolution[] resolutions = Screen.resolutions;

    //     // string res = "";
    //     // for (int i = 0; i < resolutions.Length; i++)
    //     // {
    //     //     res += resolutions[i].ToString() + ", ";
    //     // }

    //     // msg += (bmsg + "доступные разрешения : " + res);

    //     msg += (bmsg + "целевая частота кадров : " + Application.targetFrameRate);
    //     msg += (bmsg + "обновление позиций в секунду : " + Mathf.RoundToInt(1f / PlayerPrefs.GetFloat("FixedUpdateStep")));

    //     _resolutionLabel.text = msg;
    //     _headResolutionLabel.text = GetStringResolution(Screen.currentResolution);
    // }

    public void ReloadScene()
    {
        if (_loading)
            return;

        SceneTransition.ReloadScene();
        _loading = true;
        //SetLogState(1);
    }

    // public void SetResolutionScale(float scale)
    // {
    //     if (!PlayerPrefs.HasKey("StartResolution"))
    //     {
    //         //SetLogState(4);
    //         return;
    //     }

    //     string[] tmp = PlayerPrefs.GetString("StartResolution").Split('x');
    //     print($"PP start resolution : {PlayerPrefs.GetString("StartResolution")}, tmp : {tmp[0]}, {tmp[1]}");
    //     int startWidth = 1000;
    //     int startHeight = 1000;

    //     try
    //     {
    //         startWidth = System.Convert.ToInt32(tmp[0]);
    //         startHeight = System.Convert.ToInt32(tmp[1]);
    //     }
    //     catch (System.FormatException)
    //     {
    //         //SetLogState(4);
    //         return;
    //     }
    //     catch
    //     {
    //         //SetLogState(3);
    //         return;
    //     }

    //     // if (startWidth / startHeight == 1)
    //     // {
    //     //     print("не получены");
    //     //     SetLogState(4);
    //     //     return;
    //     // }

    //     Screen.SetResolution(Mathf.RoundToInt(scale * startWidth), Mathf.RoundToInt(scale * startHeight), FullScreenMode.FullScreenWindow);
    //     PlayerPrefs.SetFloat("ResolutionScale", scale);
    //     //SetLogState(2);
    //     //_headResolutionLabel.text = Mathf.RoundToInt(scale * startWidth).ToString() + " x " + Mathf.RoundToInt(scale * startHeight).ToString();
    // }

    // // public void SetLogState(int state)
    // // {
    // //     _logLabel.text = _messages[state];
    // // }

    // public string GetStringResolution(Resolution resolution)
    // {
    //     return resolution.width.ToString() + "x" + resolution.height.ToString();
    // }

    // public void BackupSettings()
    // {
    //     PlayerPrefs.DeleteKey("StartResolution");
    //     PlayerPrefs.SetFloat("ResolutionScale", 1f);
    //     PlayerPrefs.SetFloat("TargetFPS", 60f);
    //     PlayerPrefs.SetFloat("FixedUpdateStep", 0.01666667f);
    //     PlayerPrefs.SetFloat("FPSmeter", 0);

    //     SetResolutionScale(1f);

    //     SceneTransition.ReloadScene();
    //     //ReloadScene();
    // }
}
