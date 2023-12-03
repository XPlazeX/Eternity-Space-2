using UnityEngine;

namespace StatsManipulating
{
    [System.Serializable]
    public class StatOperator
    {
        [SerializeField] private string _statName;
        [SerializeField] private StatTypeOeration _typeOperation;
        [SerializeField] private float _operationValue;
        [SerializeField] private bool _negativeSetsToZero = false;

        private float _oldValue = 0f;

        public void Enforce()
        {
            Complete(_operationValue);
        }

        public void Negative()
        {
            switch (_typeOperation)
            {
                case StatTypeOeration.SetNewValue:
                    if (_negativeSetsToZero)
                        Complete(0f);
                    else
                        Complete(_oldValue);
                    break;
                
                case StatTypeOeration.Multiplying:
                    if (_negativeSetsToZero)
                        Complete(0f);
                    else
                        Complete(1f / _operationValue);
                    break;

                case StatTypeOeration.Summation:
                    if (_negativeSetsToZero)
                        Complete(0f);
                    else
                        Complete(-_operationValue);
                    break;

                case StatTypeOeration.AddNewStat:
                    if (_negativeSetsToZero)
                        Complete(0f);

                    break;

                default:
                    return;
            }
        }

        private void Complete(float value)
        {
            switch (_typeOperation)
            {
                case StatTypeOeration.SetNewValue:
                    _oldValue = ShipStats.GetValue(_statName);
                    ShipStats.ModifyStat(_statName, value);
                    break;
                
                case StatTypeOeration.Multiplying:
                    ShipStats.MultiplyStat(_statName, value);
                    break;

                case StatTypeOeration.Summation:
                    ShipStats.IncreaseStat(_statName, value);
                    break;

                case StatTypeOeration.AddNewStat:
                    ShipStats.AddNewStat(_statName, value);
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
