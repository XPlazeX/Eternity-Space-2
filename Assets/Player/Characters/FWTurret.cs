using UnityEngine;

public class FWTurret : MonoBehaviour
{
    [SerializeField] private float _yPos;
    [SerializeField] private GameObject[] _turretsByWeaponLevel = new GameObject[6];
    [SerializeField] private bool _testMode;
    [SerializeField] private int _testID;

    public GameObject SpawnedTurret {get; private set;}

    private void Start() 
    {
        int id = GameSessionInfoHandler.GetSessionSave().WeaponLevel;
        if (_testMode)
            id = _testID;

        GameObject turret = Instantiate(_turretsByWeaponLevel[id]);
        turret.transform.SetParent(transform);
        turret.transform.localPosition = new Vector3(0, _yPos, 0);

        SpawnedTurret = turret;
    }
}
