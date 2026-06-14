using UnityEngine;

public struct RadioMessageRequest
{
    public string messageId;
    public RadioChannel channel;
    public float createdAt;

    public RadioMessageRequest(string messageId, RadioChannel channel)
    {
        this.messageId = messageId;
        this.channel = channel;
        this.createdAt = Time.time;
    }
}

public enum RadioChannel {
    Story, // для сюжетных сообщений, без лимита
    Commentary, // для комментариев к действиям систем, явный лимит соббщений / время
    Reaction // для реактивных сообщений систем, щадящий лимит сообщений / времени
}