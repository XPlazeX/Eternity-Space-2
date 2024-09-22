using UnityEngine;

public class DevIngame : MonoBehaviour
{
    [SerializeField] private GameObject _panel;

    private VictoryHandler _victoryHandler;

    private void Start() {
        _victoryHandler = SceneStatics.CharacterCore.GetComponent<VictoryHandler>();
    }

    public void CompleteLevel()
    {
        _victoryHandler.LevelVictory();
    }

    public void CompleteMission()
    {
        _victoryHandler.LevelVictory(true);
    }

    public void Add500Cos()
    {
        _victoryHandler.AddCosmilite(500);
    }

    public void Add5Pos()
    {
        _victoryHandler.AddPositronium(5);
    }

    public void Add1000Au()
    {
        _victoryHandler.AddAurite(1000);
    }

    public void Close()
    {
        _panel.SetActive(false);
    }
}
