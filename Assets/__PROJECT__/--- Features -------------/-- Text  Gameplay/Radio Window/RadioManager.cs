using System;
using System.Collections.Generic;
using UnityEngine;

public class RadioManager : MonoBehaviour
{
    [SerializeField] private LocalizableMessageData[] speakersDatabase; // База данных говорящих, для получения их имен по id
    [SerializeField] private RadioMessageData[] messagesDatabase; // Дескриптор сообщений, для получения их параметров по id
    [SerializeField] private float charactersPerSecond = 20f;
    [SerializeField] private ChannelTimePolicy[] channelTimePolicies;
    [Space()]
    [SerializeField] private RadioMonitorUI radioMonitorUI;

    private List<OrderedRadioMessage> messageQueue = new List<OrderedRadioMessage>();
    private RadioRuntimeMessage currentShowingMessage = null;
    private Dictionary<RadioChannel, float> channelTimers = new Dictionary<RadioChannel, float>();

    private static RadioManager Instance { get; set; }

    void Awake()
    {
        Instance = this;
        channelTimers = new Dictionary<RadioChannel, float>(){
            { RadioChannel.Story, 100f },
            { RadioChannel.Commentary, 100f },
            { RadioChannel.Reaction, 100f }
        };
    }

    private void OnEnable() {
        radioMonitorUI.OnMessageComplete += OnMessageComplete;
    }

    void OnDisable()
    {
        radioMonitorUI.OnMessageComplete -= OnMessageComplete;
    }

    void Update()
    {
        Tick(Time.deltaTime, Time.time);
    }

    public static void RequestMessage(RadioMessageRequest request)
    {
        RadioMessageData radioMessageData = null;

        for (int i = 0; i < Instance.messagesDatabase.Length; i++)
        {
            if (Instance.messagesDatabase[i].Id.ToLower() == request.messageId.ToLower())
            {
                radioMessageData = Instance.messagesDatabase[i];
                break;
            }
        }

        if (radioMessageData == null)
        {
            Debug.LogError($"[RadioManager] Не получилось найти данные сообщения с ID {request.messageId}");
            return;
        }

        string localizedSpeakerName = null;

        for (int i = 0; i < Instance.speakersDatabase.Length; i++)
        {
            if (Instance.speakersDatabase[i].Id.ToLower() == radioMessageData.SpeakerId.ToLower())
            {
                localizedSpeakerName = MessageLocalizator.LocalizeMessage(Instance.speakersDatabase[i]);
                break;
            }
        }

        if (localizedSpeakerName == null)
        {
            Debug.LogError($"[RadioManager] Не получилось найти данные говорящего с ID {radioMessageData.SpeakerId} для сообщения с ID {request.messageId}");
            return;
        }

        string localizedMessageText = MessageLocalizator.LocalizeMessage(radioMessageData);
        string[] localizedMessagePhrases = SplitLocalizedMessagePhrases(localizedMessageText);

        if (localizedMessagePhrases.Length == 0)
        {
            Debug.LogError($"[RadioManager] Пустая локализация сообщения с ID {request.messageId}");
            return;
        }

        OrderedRadioMessage orderedMessage = new OrderedRadioMessage()
        {
            localizedSpeakerName = localizedSpeakerName,
            localizedMessagePhrases = localizedMessagePhrases,
            createdAt = request.createdAt,
            printSpeedMultiplier = radioMessageData.PrintSpeedMultiplier,
            priority = radioMessageData.Priority,
            channel = request.channel,
            waitTime = radioMessageData.WaitOrderTime,
            expiryTime = radioMessageData.ExpiryTime,
            blackBoxPolicy = radioMessageData.BlackBoxPolicy
        };

        if (Instance.channelTimers.ContainsKey(request.channel))
        {
            float abortingMinimalTime = Instance.GetAbortingTimeForChannel(request.channel);

            if ((abortingMinimalTime < 0f) || Instance.channelTimers[request.channel] >= abortingMinimalTime)
            {
                Instance.channelTimers[request.channel] = 0f;
            }
            else
            {
                Debug.Log($"[RadioManager] Сообщение с ID {request.messageId} в канал {request.channel} было отброшено из-за политики времени канала.");
                return;
            }
        }
        else
        {
            Debug.Log($"[RadioManager] Несуществующий канал сообщения с ID {request.messageId}");
            return;
        }

        bool isHigherPriority = Instance.currentShowingMessage != null && Instance.IsHigherPriority(orderedMessage.priority, Instance.currentShowingMessage.priority);
        if (Instance.currentShowingMessage == null || isHigherPriority)
        {
            Debug.Log($"[Radio Manager] Показ сообщения: {request.messageId}");
            Instance.ShowMessage(orderedMessage);
        }
        else
        {
            Debug.Log($"[Radio Manager] Сообщение добавлено в очередь: {request.messageId}");
            Instance.messageQueue.Add(orderedMessage);
        }
    }

    private void Tick(float dt, float currentTime)
    {
        List<RadioChannel> timerChannels = new List<RadioChannel>(channelTimers.Keys);

        foreach (var key in timerChannels)
        {
            if (channelTimers.ContainsKey(key))
            {
                channelTimers[key] += dt;
            }
        }

        if (currentShowingMessage == null && messageQueue.Count > 0)
        {
            PickQueueMessage();
            return;
        } 
        else if (currentShowingMessage == null && messageQueue.Count == 0)
        {
            return;
        }

        List<OrderedRadioMessage> tickedQueue = new List<OrderedRadioMessage>(messageQueue);

        for (int i = 0; i < messageQueue.Count; i++)
        {
            if (messageQueue[i].waitTime < 0f) continue;
            
            if (messageQueue[i].createdAt + messageQueue[i].waitTime < currentTime)
            {
                tickedQueue.Remove(messageQueue[i]);
            }
        }

        messageQueue = tickedQueue;
    }

    private float GetAbortingTimeForChannel(RadioChannel channel)
    {
        for (int i = 0; i < channelTimePolicies.Length; i++)
        {
            if (channelTimePolicies[i].channel == channel)
            {
                return channelTimePolicies[i].abortingMinimalTime;
            }
        }

        Debug.LogError($"[RadioManager] Не получилось найти политику времени для канала {channel}");
        return -1f;
    }

    private bool IsHigherPriority(RadioMessagePriority newMessagePriority, RadioMessagePriority currentMessagePriority)
    {
        if (newMessagePriority == currentMessagePriority)
            return false;

        if (newMessagePriority == RadioMessagePriority.Story)
            return true;
        if (newMessagePriority == RadioMessagePriority.Critical && currentMessagePriority != RadioMessagePriority.Story)
            return true;
        if (newMessagePriority == RadioMessagePriority.Standard && currentMessagePriority == RadioMessagePriority.Low)
            return true;

        return false;
    }

    private void PickQueueMessage()
    {
        ShowMessage(messageQueue[0]);
        messageQueue.RemoveAt(0);
    }

    private void ShowMessage(OrderedRadioMessage message)
    {
        currentShowingMessage = new RadioRuntimeMessage()
        {
            localizedSpeakerName = message.localizedSpeakerName,
            localizedMessagePhrases = message.localizedMessagePhrases,
            createdAt = Time.time,
            printSpeed = charactersPerSecond * message.printSpeedMultiplier,
            priority = message.priority,
            expiryTime = message.expiryTime,
            blackBoxPolicy = message.blackBoxPolicy
        };

        radioMonitorUI.ShowMessage(currentShowingMessage);
    }

    private static string[] SplitLocalizedMessagePhrases(string localizedMessageText)
    {
        if (string.IsNullOrEmpty(localizedMessageText))
        {
            return new string[0];
        }

        string[] rawPhrases = localizedMessageText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        List<string> phrases = new List<string>();

        for (int i = 0; i < rawPhrases.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(rawPhrases[i]))
            {
                continue;
            }

            phrases.Add(rawPhrases[i].TrimEnd('\r'));
        }

        return phrases.ToArray();
    }

    private void OnMessageComplete(bool isAborted)
    {
        if (isAborted)
        {
            return; // Если сообщение было прервано, то уже показывается другое сообщение, не нужно ничего делать
        }
        currentShowingMessage = null;
    }

    public class RadioRuntimeMessage
    {
        public string localizedSpeakerName;
        public string[] localizedMessagePhrases;
        public float createdAt;
        public float printSpeed;
        public RadioMessagePriority priority;
        public float expiryTime;
        public BlackBoxPolicy blackBoxPolicy;
    }

    public struct OrderedRadioMessage
    {
        public string localizedSpeakerName;
        public string[] localizedMessagePhrases;
        public float createdAt;
        public float printSpeedMultiplier;
        public RadioMessagePriority priority;
        public RadioChannel channel;
        public float waitTime;
        public float expiryTime;
        public BlackBoxPolicy blackBoxPolicy;
    }

    [System.Serializable]
    public struct ChannelTimePolicy
    {
        public RadioChannel channel;
        [Tooltip("Если сообщения в этом канале пришли быстрее, чем прошло время - они сбрасываются и не попадают в очередь. Установить -1 чтобы отключить политику для канала.")]
        public float abortingMinimalTime;
    }
}
