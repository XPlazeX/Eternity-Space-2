using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    private List<PullForObjects> ExplosionPools = new List<PullForObjects>();
    private Dictionary<int, int> Codes = new Dictionary<int, int>(); // ключ - код взрыва, значение - индекс в пуле
    private List<int> LoadingCodes = new List<int>();

    public void PreloadExplosion(int type)
    {
        StartCoroutine(LoadingExplosionResource(type));
    }

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
            print($"Экстренная загрузка взрыва: {index}");
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

    private IEnumerator LoadingExplosionResource(int id)
    {
        if (Codes.ContainsKey(id) || LoadingCodes.Contains(id))
        {
            yield break;
        }

        print($"Загружается взрыв: {id}");
        LoadingCodes.Add(id);
        ResourceRequest explosionRequest = Resources.LoadAsync<Explosion>("Explosions/" + id.ToString());

        yield return explosionRequest;

        if (explosionRequest.asset == null)
        {
            print($"- - - Пустой взрыв! Попытка обратиться к индексу: {id}");
            LoadingCodes.Remove(id);

            if (!Codes.ContainsKey(0))
            {
                print($"- - - Загружается взрыв по-умолчанию. Попытка обратиться к индексу: {id}");
                explosionRequest = Resources.LoadAsync<Explosion>("Explosions/0");
                yield return explosionRequest;
            }
            else
            {
                yield break;
            }
        }

        Codes[id] = ExplosionPools.Count;
        Explosion exp = (Explosion)explosionRequest.asset;

        ExplosionPools.Add(new PullForObjects(exp));

        if (LoadingCodes.Contains(id))
            LoadingCodes.Remove(id);

        print($"Загружен взрыв: {id}!");
    }
}
