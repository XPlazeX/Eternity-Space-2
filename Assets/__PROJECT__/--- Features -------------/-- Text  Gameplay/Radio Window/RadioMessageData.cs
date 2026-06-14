using UnityEngine;

[CreateAssetMenu(fileName = "_RMD", menuName = "Messages/RadioMessageData", order = 0)]
public class RadioMessageData : LocalizableMessageData 
{
    [Tooltip("Id говорящего, RadioManager будет искать его в своей базе данных LMD и использовать переведенное имя.")]
    [SerializeField] private string speakerId = "speaker_debug";
    [Tooltip("Standard перебивает Low, Critical перебивает Standard, Story перебивает всё.")]
    [SerializeField] private RadioMessagePriority priority = RadioMessagePriority.Standard;
    [SerializeField] private float printSpeedMultiplier = 1f;
    [Tooltip("Через это время сообщение удалится из очереди, если оно не дождалось своей очереди на показ. Установить -1 чтобы сообщение гарантированно дождалось очереди.")]
    [SerializeField] private float waitOrderTime = 3f;
    [Tooltip("Время жизни сообщения после начала его показа. Может прерываться сообщениями с более высоким приоритетом.")]
    [SerializeField][Range(0f, 10f)] private float expiryTime = 3f;
    [SerializeField] private BlackBoxPolicy blackBoxPolicy = BlackBoxPolicy.OnlyPrintedMessage;

    public string SpeakerId { get => speakerId; set => speakerId = value; }
    public RadioMessagePriority Priority { get => priority; set => priority = value; }
    public float PrintSpeedMultiplier { get => printSpeedMultiplier; set => printSpeedMultiplier = value; }
    public float WaitOrderTime { get => waitOrderTime; set => waitOrderTime = value; }
    public float ExpiryTime { get => expiryTime; set => expiryTime = value; }
    public BlackBoxPolicy BlackBoxPolicy { get => blackBoxPolicy; set => blackBoxPolicy = value; }
}

public enum RadioMessagePriority {
    Low,
    Standard,
    Critical,
    Story
}

public enum BlackBoxPolicy {
    Never,
    AlwaysFullMessage,
    OnlyPrintedMessage
}