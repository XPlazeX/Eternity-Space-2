using System.Collections;
using UnityEngine;

public class UltratechSimple : Ultratech
{
    [SerializeField] private GameObject[] _additiveModules;

    public override void Load()
    {
        base.Load();
        UTUsed += SpawnAdditive;
    }

    private void OnDisable() {
        UTUsed -= SpawnAdditive;
    }

    protected void SpawnAdditive()
    {
        Transform playerTransform = Player.PlayerTransform;

        for (int i = 0; i < _additiveModules.Length; i++)
        {
            var module = Instantiate(_additiveModules[i], playerTransform.position, Quaternion.identity);
            module.transform.SetParent(playerTransform); // обьект уничтожится вместе с трансформацией
            //print(module.gameObject.name);
        }
    }
}
