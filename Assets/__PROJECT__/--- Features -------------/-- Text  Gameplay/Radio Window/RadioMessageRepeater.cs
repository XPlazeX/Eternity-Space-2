using UnityEngine;

public class RadioMessageRepeater : MonoBehaviour
{
    [SerializeField] private string messageId;
    [SerializeField] private RadioChannel channel;
    [SerializeField] private float initialDelay = 0f;
    [SerializeField] private float repeatInterval = 5f;

    private float lastMessageTime = -Mathf.Infinity;

    void Start()
    {
        lastMessageTime = ESTime.worldTime - ( repeatInterval - initialDelay );
    }

    void Update()
    {
        if (ESTime.worldTime - lastMessageTime >= repeatInterval)
        {
            RadioManager.RequestMessage(new RadioMessageRequest(messageId, channel));
            lastMessageTime = ESTime.worldTime;
        }
    }
}
