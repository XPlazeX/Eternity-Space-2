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
        lastMessageTime = Time.time - ( repeatInterval - initialDelay );
    }

    void Update()
    {
        if (Time.time - lastMessageTime >= repeatInterval)
        {
            RadioManager.RequestMessage(new RadioMessageRequest(messageId, channel));
            lastMessageTime = Time.time;
        }
    }
}
