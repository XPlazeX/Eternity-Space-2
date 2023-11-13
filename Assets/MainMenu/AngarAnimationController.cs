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

    public void SetAirLabelState(int state = 0)
    {
        switch (state)
        {
            case 0:
                _airLabel.text = "ВОЗДУХ";
                break;
            case 1:
                _airLabel.text = "ТЯГА";
                break;
            case 2:
                _airLabel.text = "ВОЗДУХА НЕТ";
                break;
            case 3:
                _airLabel.text = "ПРОДУВ";
                break;
            default:
                throw new System.Exception("Неопределённое состояние индикатора воздуха");
        }
    }
}
