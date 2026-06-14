using UnityEngine;

public class MessageLocalizator : MonoBehaviour
{
    public static string LocalizeMessage(LocalizableMessageData messageData)
    {
        // Будет иметь свои файлы локализации, искать по id и возвращать переведенный текст.
        return messageData.MessageText; // Пока просто возвращаем оригинальный текст
    }
}
