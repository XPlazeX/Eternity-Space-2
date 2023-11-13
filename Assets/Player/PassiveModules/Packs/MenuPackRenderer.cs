using UnityEngine;

public class MenuPackRenderer : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform[] _packs;

    private Transform _activePack;

    public void RenderPack(int id)
    {
        if (_activePack != null)
            Destroy(_activePack.gameObject);

        Transform pack = Instantiate(_packs[id], _playerTransform.position, Quaternion.identity);

        pack.SetParent(_playerTransform);
        _activePack = pack;
    }
}
