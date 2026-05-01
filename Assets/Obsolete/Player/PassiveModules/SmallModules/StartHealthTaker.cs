using UnityEngine;

public class StartHealthTaker : MonoBehaviour
{
    [Header("отрицательное число = регенерация, положительное - отнятие")]
    [SerializeField] private int _takingValue;

    void Start()
    {
        print("started");
        if (_takingValue > 0)
        {
            PlayerShipData.ConsumeHP(_takingValue);
            print("consumed");
        }

        else if (_takingValue < 0)
            PlayerShipData.RegenerateHP(-_takingValue);
    }
}
