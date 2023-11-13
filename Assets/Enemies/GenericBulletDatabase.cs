using System.Collections.Generic;
using UnityEngine;

public class GenericBulletDatabase : MonoBehaviour
{
    private static Dictionary<int, PullForObjects> BulletCodes = new Dictionary<int, PullForObjects>();
    private static Dictionary<int, PullForObjects> LaserCodes = new Dictionary<int, PullForObjects>();

    public bool Initialize()
    {
        BulletCodes.Clear();
        LaserCodes.Clear();

        return true;
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
            InitializeBulletSample(index);
        }

        return BulletCodes[index].GetGameObject().GetComponent<Bullet>();
    }

    public static LaserObject GetLaser(int index)
    {
        if (!LaserCodes.ContainsKey(index))
        {
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
}
