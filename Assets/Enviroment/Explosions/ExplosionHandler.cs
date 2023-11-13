using System.Collections.Generic;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    private List<PullForObjects> ExplosionPools = new List<PullForObjects>();
    private Dictionary<int, int> Codes = new Dictionary<int, int>();

    public void SpawnExplosion(Vector3 position, int type)
    {
        var explosionObject = GetExplosion(type);
        if (explosionObject == null)
            return;
        explosionObject.transform.position = position;
        
    }

    public GameObject InstantiateExplosion(Vector3 position, int type)
    {
        var explosionObject = GetExplosion(type);
        if (explosionObject == null)
            return null;
        explosionObject.transform.position = position;
        
        return explosionObject;
    }

    private GameObject GetExplosion(int index)
    {
        if (!Codes.ContainsKey(index))
        {
            Codes[index] = ExplosionPools.Count;
            Explosion exp = Resources.Load<Explosion>("Explosions/" + index.ToString());

            if (exp == null)
            {
                exp = Resources.Load<Explosion>("Explosions/0");
                print($"- - - Был загружен взрыв по-умолчанию. Попытка обратиться к индексу: {index}");
            }
            ExplosionPools.Add(new PullForObjects(exp));
        }

        return ExplosionPools[Codes[index]].GetGameObject();
    }
}
