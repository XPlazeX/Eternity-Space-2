using UnityEngine;

[CreateAssetMenu(fileName = "_LMD", menuName = "Messages/LocalizableMessageData", order = 0)]
public class LocalizableMessageData : ScriptableObject 
{
    [SerializeField] private string id = "resource.speaker.message";
    [SerializeField][TextArea] private string messageText = "This is a localizable message.";

    public string Id { get => id; set => id = value; }
    public string MessageText { get => messageText; set => messageText = value; }
}
