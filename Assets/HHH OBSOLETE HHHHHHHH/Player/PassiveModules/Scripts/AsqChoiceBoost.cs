using UnityEngine;

public class AsqChoiceBoost : Module
{
    [SerializeField] private int _choiceBoost;

    public override void Asquiring()
    {
        MissionEventsDistributor.BoostEventCountForMission(_choiceBoost);
    }
}
