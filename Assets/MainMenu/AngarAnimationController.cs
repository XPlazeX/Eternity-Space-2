using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngarAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _angarAnimator;
    [SerializeField] private Text _airLabel;
    //[SerializeField] private Transform _player;
    

    public void StartGame()
    {
        _angarAnimator.SetTrigger("NewGame");
        // msgs
    }

    public void StopGame()
    {
        _angarAnimator.SetTrigger("StopGame");
    }

    public void OnSessionInitialized()
    {
        _angarAnimator.SetTrigger("OnSessionInitialized");
    }

    public void LoadGameScene()
    {
        SceneTransition.SwitchToScene("Game");
    }

    public void ShowConsole()
    {
        GameObject.FindWithTag("SceneCore").GetComponent<MenuController>().OpenGroup(0);
        Camera.main.GetComponent<MainMenuCamera>().ExtraNoise();

    }

    private void OnEnable() {
        ContagionHandler.ContagionChanged += OnContagionChanged;
    }

    private void OnDisable() {
        ContagionHandler.ContagionChanged -= OnContagionChanged;
    }

    public void OnContagionChanged()
    {
        SetAirLabelState();
    }

    public void SetAirLabelState(int state = 0)
    {
        switch (state)
        {
            case 0:
                if (ContagionHandler.ContagionLevel >= 11)
                    _airLabel.text = SceneLocalizator.GetLocalizedString("MissionMenu", 0, 0);
                else
                    _airLabel.text = SceneLocalizator.GetLocalizedString("MissionMenu", 0, 1);
                break;
            case 1:
                _airLabel.text = SceneLocalizator.GetLocalizedString("MissionMenu", 0, 2);
                break;
            case 2:
                if (ContagionHandler.ContagionLevel >= 11)
                    _airLabel.text = SceneLocalizator.GetLocalizedString("MissionMenu", 0, 3);
                else
                    _airLabel.text = SceneLocalizator.GetLocalizedString("MissionMenu", 0, 4);
                break;
            case 3:
                _airLabel.text = SceneLocalizator.GetLocalizedString("MissionMenu", 0, 5);
                break;
            default:
                throw new System.Exception("Неопределённое состояние индикатора воздуха");
        }
    }
}
