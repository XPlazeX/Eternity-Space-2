using UnityEngine;

[System.Serializable]
public class SectorObjetsActivationRules
{
    [SerializeField] private DistanceRuleGroup[] distanceRuleGroups;

    public void Evaluate(float distance)
    {
        for (int i = 0; i < distanceRuleGroups.Length; i++)
        {
            for (int j = 0; j < distanceRuleGroups[i].ObjectCount; j++)
            {
                GameObject togglingObject = distanceRuleGroups[i].TogglingObjects[j];
                bool flag = togglingObject.activeSelf;

                switch (distanceRuleGroups[i].ActivationRule)
                {
                    case DistanceActivationRule.AlwaysActive:
                        if (!flag)
                        {
                            togglingObject.SetActive(true);
                        }
                    break;
                    case DistanceActivationRule.ActivateWhenNear:
                        if (!flag && distance < distanceRuleGroups[i].ActivationDistance)
                        {
                            togglingObject.SetActive(true);
                        }
                        else if (flag && distance > distanceRuleGroups[i].ActivationDistance + distanceRuleGroups[i].DeactivationTolerance)
                        {
                            togglingObject.SetActive(false);
                        }
                    break;
                    case DistanceActivationRule.ActivateWhenFar:
                        if (!flag && distance > distanceRuleGroups[i].ActivationDistance)
                        {
                            togglingObject.SetActive(true);
                        }
                        else if (flag && distance < distanceRuleGroups[i].ActivationDistance - distanceRuleGroups[i].DeactivationTolerance)
                        {
                            togglingObject.SetActive(false);
                        }
                    break;
                    default:
                        continue;
                }
            }
        }
    }
}

[System.Serializable]
public class DistanceRuleGroup
{
    [SerializeField] private DistanceActivationRule activationRule = DistanceActivationRule.AlwaysActive;
    [SerializeField] private float activationDistance = 100f;
    [SerializeField] private float deactivationTolerance = 20f;
    [SerializeField] private GameObject[] togglingObjects;

    public DistanceActivationRule ActivationRule { get => activationRule; private set => activationRule = value; }
    public float ActivationDistance { get => activationDistance; private set => activationDistance = value; }
    public float DeactivationTolerance { get => deactivationTolerance; private set => deactivationTolerance = value; }
    public GameObject[] TogglingObjects { get => togglingObjects; private set => togglingObjects = value; }

    public int ObjectCount => TogglingObjects == null ? 0 : TogglingObjects.Length;
}

public enum DistanceActivationRule
{
    AlwaysActive,
    ActivateWhenNear,
    ActivateWhenFar
}
