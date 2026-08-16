using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    void Update()
    {
        transform.position = Player.Position;
    }
}
