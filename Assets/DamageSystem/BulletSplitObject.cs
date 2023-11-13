using UnityEngine;

public class BulletSplitObject : MonoBehaviour
{
    [SerializeField] private bool _splitOnEnemyDeath;
    [SerializeField] private int _bulletCode;
    [SerializeField] private int _countShards;
    [SerializeField] private float _startAngle;
    [SerializeField] private float _angleStep;
    [SerializeField] private float _randomAngleStep;
    [SerializeField][Range(0, 3f)] private float _randomizeSpeed = 0f;
    [SerializeField] private SoundObject _sound;

    private void Start() {
        if (!_splitOnEnemyDeath)
            return;

        GetComponent<DamageBody>().Deathed += Split;        
    }

    public void Split()
    {
        float startAngle = transform.eulerAngles.z + _startAngle;

        for (int i = 0; i < _countShards; i++)
        {
            Bullet bullet = GenericBulletDatabase.GetBullet(_bulletCode);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.Euler(0, 0, startAngle + Random.Range(-_randomAngleStep, _randomAngleStep));
            bullet.MultiplySpeedParams(Random.Range(1f / (1f + _randomizeSpeed), 1f * (1f + _randomizeSpeed)));

            startAngle += _angleStep;
        }

        SoundPlayer.PlaySound(_sound);
    }
}
