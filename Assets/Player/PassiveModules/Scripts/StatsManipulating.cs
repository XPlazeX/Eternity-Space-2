using UnityEngine;

namespace StatsManipulating
{
    [System.Serializable]
    public class StatOperator
    {
        [SerializeField] private string _statName;
        [SerializeField] private StatTypeOeration _typeOperation;
        [SerializeField] private float _operationValue;

        public void Enforce()
        {
            switch (_typeOperation)
            {
                case StatTypeOeration.SetNewValue:
                    ShipStats.ModifyStat(_statName, _operationValue);
                    break;
                
                case StatTypeOeration.Multiplying:
                    ShipStats.MultiplyStat(_statName, _operationValue);
                    break;

                case StatTypeOeration.Summation:
                    ShipStats.IncreaseStat(_statName, _operationValue);
                    break;

                case StatTypeOeration.AddNewStat:
                    ShipStats.AddNewStat(_statName, _operationValue);
                    break;

                default:
                    return;
            }
        }
    }

    [System.Serializable]
    public class HealthStatOperator
    {
        private enum StatName
        {
            Health = 0,
            Armor = 1
        }

        [SerializeField] private StatName _statType;
        [SerializeField] private float _multiplier;

        public void Enforce()
        {
            switch (_statType)
            {
                case StatName.Health:
                    PlayerShipData.MultiplyHP(_multiplier);
                    break;

                case StatName.Armor:
                    PlayerShipData.MultiplyARM(_multiplier);
                    break;

                default:
                    return;
            }
        }
    }

    public enum StatTypeOeration
    {
        SetNewValue = 0,
        Multiplying = 1,
        Summation = 2,
        AddNewStat = 3
    }
}
