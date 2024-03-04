using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GenericBulletDatabase : MonoBehaviour
{
    [SerializeField] private bool _logLoading = false;

    private static Dictionary<int, PullForObjects> BulletCodes = new Dictionary<int, PullForObjects>();
    private static Dictionary<int, PullForObjects> LaserCodes = new Dictionary<int, PullForObjects>();
    private static List<int> LoadingCodes = new List<int>();

    private static GenericBulletDatabase Instance;

    public bool Initialize()
    {
        Instance = this;

        BulletCodes.Clear();
        LaserCodes.Clear();

        return true;
    }

    public static void PreloadBullet(int index)
    {
        Instance.StartCoroutine(Instance.LoadingBulletResource(index));
    }

    public static void Preloadlaser(int index)
    {
        Instance.StartCoroutine(Instance.LoadingLaserResource(index));
    }

    private static void InitializeBulletSample(int index)
    {
        PullableObject obj = Resources.Load<PullableObject>($"Bullets/{index}");

        if (obj == null)
        {
            obj = Resources.Load<PullableObject>($"Bullets/0");
            print($"- - - Загружен экземпляр пули по умолчанию, вместо индекса: {index} - - -");
        }

        BulletCodes[index] = new PullForObjects(obj);
    }

    private static void InitializeLaserSample(int index)
    {
        PullableObject obj = Resources.Load<PullableObject>($"Lasers/{index}");

        if (obj == null)
        {
            obj = Resources.Load<PullableObject>($"Lasers/0");
            print($"- - - Загружен экземпляр лазера по умолчанию, вместо индекса: {index} - - -");
        }

        LaserCodes[index] = new PullForObjects(obj);
    }

    public static Bullet GetBullet(int index)
    {
        if (!BulletCodes.ContainsKey(index))
        {
            print($"Экстренная загрузка пули: {index}");
            InitializeBulletSample(index);
        }

        return BulletCodes[index].GetGameObject().GetComponent<Bullet>();
    }

    public static LaserObject GetLaser(int index)
    {
        if (!LaserCodes.ContainsKey(index))
        {
            print($"Экстренная загрузка лазера: {index}");
            InitializeLaserSample(index);
        }

        return LaserCodes[index].GetGameObject().GetComponent<LaserObject>();
    }

    public static Bullet GetForChangeBullet(int index)
    {
        if (!BulletCodes.ContainsKey(index))
        {
            InitializeBulletSample(index);
        }

        BulletCodes[index].SampleChanged();
        return BulletCodes[index].Sample as Bullet;
    }

    private IEnumerator LoadingBulletResource(int id)
    {
        if (BulletCodes.ContainsKey(id) || LoadingCodes.Contains(id * 2))
        {
            yield break;
        }

        if (_logLoading) print($"Загружается пуля: {id}");
        LoadingCodes.Add(id * 2);

        ResourceRequest bulletRequest = Resources.LoadAsync<PullableObject>("Bullets/" + id.ToString());
        yield return bulletRequest;

        if (bulletRequest.asset == null)
        {
            throw new System.Exception($"Пустой ассет: {"Bullets/" + id.ToString()}");
        }

        BulletCodes[id] = new PullForObjects((PullableObject)bulletRequest.asset);

        LoadingCodes.Remove(id * 2);

        if (_logLoading) print($"Пуля загружена!: {id}");
    }

    private IEnumerator LoadingLaserResource(int id)
    {
        if (LaserCodes.ContainsKey(id) || LoadingCodes.Contains(id * 2 + 1))
        {
            yield break;
        }

        if (_logLoading) print($"Загружается лазер: {id}");
        LoadingCodes.Add(id * 2 + 1);

        ResourceRequest laserRequest = Resources.LoadAsync<PullableObject>("Lasers/" + id.ToString());
        yield return laserRequest;

        if (laserRequest.asset == null)
        {
            throw new System.Exception($"Пустой ассет: {"Lasers/" + id.ToString()}");
        }

        LaserCodes[id] = new PullForObjects((PullableObject)laserRequest.asset);

        LoadingCodes.Remove(id * 2 + 1);

        if (_logLoading) print($"Лазер загружен!: {id}");
    }
}
