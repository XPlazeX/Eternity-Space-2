using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentDB", menuName = "Teleplane2/EnvironmentDB", order = 0)]
public class EnvironmentDB : ScriptableObject {
    [SerializeField] private EnvironmentObject[] _environmentObjects;

    public EnvironmentObject[] Objects => _environmentObjects;
}

[System.Serializable]
public class EnvironmentObject
{
    [SerializeField] private PullableObject _sample;
    [SerializeField][Range(0.01f, 1000f)] private float _chance;

    public PullableObject Sample => _sample;
    public float Chance => _chance;
}
