using UnityEngine;
using UnityEngine.UI;

public class ReloadSpawnButton : MonoBehaviour
{
    [SerializeField] private Button _controllingButton;
    [SerializeField] private float _reloadTime;
    [SerializeField] private Image _fillingImage;
    [Space()]
    [SerializeField] private GameObject _spawnObject;

    private float _timer;

    private void Start() {
        _timer = _reloadTime;
    }

    private void Update() {

        _fillingImage.fillAmount = (1f - (_timer / _reloadTime));

        _controllingButton.interactable = _timer <= 0f;

        _timer -= ESTime.worldDeltaTime;
    }

    public void SpawnObject()
    {
        GameObject obj = Instantiate(_spawnObject, Player.PlayerTransform.position, Quaternion.identity);
        obj.transform.SetParent(Player.PlayerTransform);

        _timer = _reloadTime;
    }
}
