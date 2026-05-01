using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RuntimeStatDefaults", menuName = "Eternity Space/RR Runtime Stat Defaults")]
public class RuntimeStatDefaultsAsset : ScriptableObject
{
    [SerializeField] private List<Entry> entries = new();

    public IReadOnlyList<Entry> Entries => entries;

    [Serializable]
    public class Entry
    {
        public RuntimeStat stat;
        public float value = 1f;
        public RuntimeStatGroup group = RuntimeStatGroup.Player;
    }
}