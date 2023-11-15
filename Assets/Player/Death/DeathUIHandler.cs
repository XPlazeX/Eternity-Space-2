using UnityEngine;
using System.Collections;

public class DeathUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject _deathPanel;
    [SerializeField] private GameObject _timeRift;

    public static bool NoEraseData {get; set;} = false;

    public void Death()
    {
        if (NoEraseData)
        {
            NoEraseData = false;
        } else 
        {
            GameSessionInfoHandler.ClearGameSession();
        }

        DeathCountRegister.RegisterDeath();
        _deathPanel.SetActive(true);
        TimeHandler.Pause();
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
