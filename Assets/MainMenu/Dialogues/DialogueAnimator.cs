using UnityEngine;
using DialogueSystem;

[CreateAssetMenu(fileName = "Dialogue Animator", menuName = "Teleplane2/Dialogue Animator", order = -1)]
public class DialogueAnimator : ScriptableObject
{
    [SerializeField] private DialogueFrame[] frames;

    public DialogueFrame[] GetFrames() => frames;
}

namespace DialogueSystem
{
    [System.Serializable]
    public struct DialogueFrame
    {
        public int phraseCode;
        public DialogueSide side;
        public Sprite settingIcon;
        public SoundObject sound;
        public int repeats;
    }

    public enum DialogueSide
    {
        Left = 0,
        Right = 1
    }
}
