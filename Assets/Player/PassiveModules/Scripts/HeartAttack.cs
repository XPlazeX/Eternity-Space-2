using System.Collections.Generic;
using UnityEngine;

public class HeartAttack : Module
{
    [SerializeField] private int _minimumHealth;

    private List<HeartAttacker> HeartAttackers = new List<HeartAttacker>();
    private bool _loaded = false;

    public override void Load()
    {
        Spawner.DamageBodySpawned += OnDamageBodySpawned;
        _loaded = true;
    }

    private void OnDisable() {
        if (_loaded)
        {
            Spawner.DamageBodySpawned -= OnDamageBodySpawned;
        }
    }

    public void OnDamageBodySpawned(DamageBody db)
    {
        HeartAttacker ha = new HeartAttacker(db, _minimumHealth);
        ha.Attacked += OnAttacked;

        HeartAttackers.Add(ha);
    }

    private void OnAttacked(HeartAttacker ha)
    {
        HeartAttackers.Remove(ha);
        print($"attacked! last: {HeartAttackers.Count}");
    }

    public class HeartAttacker
    {
        public delegate void operation(HeartAttacker ha);
        public event operation Attacked;

        public DamageBody _bindedDB;
        public int minimumHP;

        public HeartAttacker (DamageBody bindingDB, int minHP)
        {
            _bindedDB = bindingDB;
            minimumHP = minHP;

            _bindedDB.DamageTaking += OnDamageTaken;
        }

        public void OnDamageTaken(int hitPoints)
        {
            if (hitPoints < minimumHP)
            {
                _bindedDB.TakeDamage(_bindedDB.HitPoints);
                _bindedDB.TakeDamage(_bindedDB.HitPoints);
                Attacked?.Invoke(this);
            }
        }

    }
}
