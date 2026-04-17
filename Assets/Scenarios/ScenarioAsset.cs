using UnityEngine;
using ScenarioSystem;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Eternity Space/Scenario")]
public class ScenarioAsset : ScriptableObject
{
    public List<NodeData> nodes;
    public int startNodeIndex = 0;
}