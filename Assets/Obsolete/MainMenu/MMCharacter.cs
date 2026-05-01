using System.Collections;
using UnityEngine;

public class MMCharacter : MonoBehaviour
{
    [SerializeField] private bool _testMode;
    [SerializeField] private int _testID;

    private void OnEnable() 
    {
        if (!SceneStatics.CoresFinded)
        {
            SceneStatics.CoresLoaded += OnCoresLoaded;
        }
        else
        {
            StartCoroutine(LoadingCharacter());
        }
    }

    private IEnumerator LoadingCharacter()
    {
        int id = GameSessionInfoHandler.GetSessionSave().ShipModel;

        if (_testMode)
            id = _testID;

        CharacterLoader characterLoader = SceneStatics.CharacterCore.GetComponent<CharacterLoader>();

        print("<color=lime>Start loading character</color>");
        yield return characterLoader.StartCoroutine(characterLoader.LoadingPlayerShip(id, CharacterLoader.CharacterLoadType.MissionMenuLoad));

        GameObject.FindWithTag("Player").GetComponent<MMShip>().RenderShip(id);

        SceneTransition.SceneLoaded();
    }

    private void OnCoresLoaded()
    {
        SceneStatics.CoresLoaded -= OnCoresLoaded;
        StartCoroutine(LoadingCharacter());
    }
}
