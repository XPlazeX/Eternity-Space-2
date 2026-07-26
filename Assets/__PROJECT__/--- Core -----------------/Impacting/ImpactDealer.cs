using UnityEngine;

public class ImpactDealer : MonoBehaviour
{
    [SerializeField] private Impact[] impacts;

    protected Impact[] Impacts => impacts;

    public ImpactResult DealImpact(IImpactReceiver receiver, in ImpactContext context)
    {
        return receiver.ReceiveImpact(context);
    }
}
