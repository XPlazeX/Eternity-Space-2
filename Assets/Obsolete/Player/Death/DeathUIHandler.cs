using UnityEngine;
using System.Collections;

public class DeathUIHandler : MonoBehaviour
{
    public delegate void deathEvent();
    public static event deathEvent AnyDeathed;
    public static event deathEvent DeathedCleared;

    [SerializeField] private GameObject _deathPanel;
    [SerializeField] private GameObject _timeRift;
    [SerializeField] private GameObject _vectorErrorer;

    public static bool NoEraseData {get; set;} = false;
    public static bool FastRestart {get; set;} = false;

    public void TrySpawnErrorer() 
    {
        int errorSpawns = 0;
        if (Random.value <= 0.25f && GameSessionInfoHandler.GetSessionSave().LocationID != 26)
        {
            errorSpawns ++;
        }
        if (((GameSessionInfoHandler.CurrentLevel + 1) % 4 == 0) || GameSessionInfoHandler.IsSignalLevel)
        {
            errorSpawns ++;
        }
        if (EternityClock.Parsing)
        {
            errorSpawns ++;
        }

        for (int i = 0; i < errorSpawns; i++)
        {
            // Instantiate(_vectorErrorer, CameraController.GetRandomFieldPosition(6f, Vector3.zero, 1f), Quaternion.identity);
            print("<color=magenta>Spawn Vector errorer!</color>");
        }
    }

    public void Death()
    {
        if ((GameSessionInfoHandler.GetSessionSave().VectorError || EternityClock.Parsing) && GameSessionInfoHandler.GetSessionSave().LocationID != 26)
        {
            NoEraseData = true;
            FastRestart = true;
            GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
            save.LocationID = 26;
            save.CurrentLevel = 0;
            save.MaxLevel = 404;
            save.HealthPoints = save.MaxHealth;
            GameSessionInfoHandler.RewriteSessionSave(save);
            VictoryHandler.CustomSceneOnDeath = "Game";
            VictoryHandler.CustomTransitionAsDeath = true;
        }
        if (NoEraseData)
        {
            NoEraseData = false;
        } else 
        {
            GameSessionInfoHandler.ClearGameSession();
            DeathedCleared?.Invoke();
        }

        AnyDeathed?.Invoke();

        DeathCountRegister.RegisterDeath();
        if (!FastRestart)
        {
            _deathPanel.SetActive(true);
            TimeHandler.Pause();
        } else
        {
            FastRestart = false;
            EndLevel();
        }
    }

    public void ErrorVector()
    {
        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();
        save.VectorError = true;
        GameSessionInfoHandler.RewriteSessionSave(save);
    }

    public void EndLevel()
    {
        _deathPanel.SetActive(false);

        TimeHandler.Resume(1f);

        Instantiate(_timeRift, Player.PlayerTransform.position, Quaternion.Euler(0, 0, Random.Range(-180f, 180f)));

        StartCoroutine(ChangePlayerMaskInteraction());
    }

    private IEnumerator ChangePlayerMaskInteraction()
    {
        Transform playerTransform = Player.PlayerTransform;
        playerTransform.GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(1f);

        SpriteRenderer spriteRenderer = Player.PlayerObject.GetComponent<SpriteRenderer>();
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        for (int i = 0; i < playerTransform.childCount; i++)
        {
            Destroy(playerTransform.GetChild(i).gameObject);
        }
        yield return new WaitForSeconds(2.7f);

        VictoryHandler.LoseSession();
        //SceneTransition.SwitchToScene("Lobby");
    }
}
