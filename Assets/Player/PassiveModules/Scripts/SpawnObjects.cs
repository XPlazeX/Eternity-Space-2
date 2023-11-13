using UnityEngine;

public class SpawnObjects : Module
{
    [SerializeField] private GameObject[] _spawningObjects;
    [SerializeField] private bool _onPlayerPosition;
    [SerializeField] private bool _randomPosition;
    [SerializeField] private bool _bindToPlayer;

    public override void Load()
    {
        for (int i = 0; i < _spawningObjects.Length; i++)
        {
            Transform obj = Instantiate(_spawningObjects[i], Vector3.zero, Quaternion.identity).transform;

            if (_onPlayerPosition)
                obj.position = Player.PlayerTransform.position;
            else if (_randomPosition)
                obj.position = new Vector3(
                    Random.Range(CameraController.Borders_xXyY.x, CameraController.Borders_xXyY.y),
                    Random.Range(CameraController.Borders_xXyY.z, CameraController.Borders_xXyY.w), 0f
                );

            if (_bindToPlayer)
                obj.SetParent(Player.PlayerTransform);
        }
    }
}
