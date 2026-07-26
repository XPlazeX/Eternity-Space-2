using UnityEngine;

public class InitialSpawnPoint : MonoBehaviour
{
    private bool _sub = false;

    private void OnEnable() {
        bool hasPlayer = Player.PlayerTransform != null;

        if (hasPlayer)
        {
            ResetPlayerPosition();
            return;
        }

        Player.PlayerChanged += ResetPlayerPosition;
        _sub = true;
    }

    public void ResetPlayerPosition()
    {
        Player.PlayerTransform.position = transform.position;

        if (_sub)
        {
            Player.PlayerChanged -= ResetPlayerPosition;
        }
    }
}
