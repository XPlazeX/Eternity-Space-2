using UnityEngine;

public class PSEmitter : MonoBehaviour
{
    private static ParticleSystem _ps;

    private void Awake() {
        _ps = GetComponent<ParticleSystem>();
    }
    
    public static void Emit(int count)
    {
        _ps.Emit(count);
    }
}
