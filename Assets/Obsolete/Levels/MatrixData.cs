using UnityEngine;

public class MatrixData : MonoBehaviour
{
    public static int Level {get; private set;}

    public static float MedkitChance {get; set;} = 0.01f;
    public static float StalkerChance {get; set;} = 0f;
    public static int EnemyBuffer {get; set;} = 0;

    public static float CosmilithiumMultiplier {get; set;} = 1f;
    public static float PositroniumMultiplier {get; set;} = 1f;
    public static int GearLevelBonus {get; set;} = 0;
}
