using System.Collections;
using TMPro;
using UnityEngine;

public class RadioMonitorUI : MonoBehaviour
{
    public const float MESSAGE_COMPLETE_DELAY = 0.5f;

    public event System.Action<bool> OnMessageComplete;

    [Header("Text")]
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text messageText;
    [Header("Animation")]
    [SerializeField] private RectTransform maskRect;
    [SerializeField] private float maskAnimationDuration = 0.5f;

    private string _printedMessageText;
    private Vector3 _openedMaskScale;
    private Coroutine _messageCoroutine;
    private Coroutine _maskAnimationCoroutine;
    private bool _messageAwaitingCompletion;

    public string PrintedMessageText => _printedMessageText;

    private void Awake()
    {
        _openedMaskScale = maskRect != null ? maskRect.localScale : Vector3.one;
        SetMaskOpenProgress(0f);
        ClearText();
    }

    public void ShowMessage(RadioManager.RadioRuntimeMessage message)
    {
        if (message == null)
        {
            return;
        }

        if (_messageAwaitingCompletion)
        {
            CompleteMessage(true);
        }

        if (_messageCoroutine != null)
        {
            StopCoroutine(_messageCoroutine);
            _messageCoroutine = null;
        }

        ClearText();

        PlayMaskAnimation(1f);

        _messageCoroutine = StartCoroutine(ShowMessageRoutine(message));
    }

    public void CompleteMessage(bool isAborted)
    {
        _messageAwaitingCompletion = false;
        OnMessageComplete?.Invoke(isAborted);
    }

    private IEnumerator ShowMessageRoutine(RadioManager.RadioRuntimeMessage message)
    {
        _messageAwaitingCompletion = true;
        if (message.localizedMessagePhrases == null || message.localizedMessagePhrases.Length == 0)
        {
            CompleteMessage(false);
            _messageCoroutine = null;
            yield break;
        }

        if (speakerNameText != null)
        {
            speakerNameText.text = message.localizedSpeakerName;
        }

        for (int i = 0; i < message.localizedMessagePhrases.Length; i++)
        {
            ClearMessageText();
            yield return PrintMessageRoutine(message.localizedMessagePhrases[i], message.printSpeed);

            yield return new WaitForSeconds(Mathf.Max(0f, message.expiryTime));
        }

        CompleteMessage(false);

        yield return new WaitForSeconds(MESSAGE_COMPLETE_DELAY);

        _messageCoroutine = null;
        ClearText();
        PlayMaskAnimation(0f);
    }

    private IEnumerator PrintMessageRoutine(string message, float printSpeed)
    {
        if (string.IsNullOrEmpty(message))
        {
            yield break;
        }

        float delay = printSpeed > 0f ? 1f / printSpeed : 0f;

        for (int i = 0; i < message.Length;)
        {
            if (TryReadRichTextElement(message, i, out string richTextElement, out int nextIndex))
            {
                AppendPrintedText(richTextElement);
                i = nextIndex;
                continue;
            }

            AppendPrintedText(message[i].ToString());
            i++;

            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }
        }
    }

    private bool TryReadRichTextElement(string message, int startIndex, out string richTextElement, out int nextIndex)
    {
        richTextElement = null;
        nextIndex = startIndex;

        if (message[startIndex] != '<')
        {
            return false;
        }

        int closeIndex = message.IndexOf('>', startIndex + 1);
        if (closeIndex < 0)
        {
            return false;
        }

        richTextElement = message.Substring(startIndex, closeIndex - startIndex + 1);
        nextIndex = closeIndex + 1;
        return true;
    }

    private void AppendPrintedText(string text)
    {
        _printedMessageText += text;

        if (messageText != null)
        {
            messageText.text = _printedMessageText;
        }
    }

    private void ClearText()
    {
        ClearMessageText();

        if (speakerNameText != null)
        {
            speakerNameText.text = string.Empty;
        }
    }

    private void ClearMessageText()
    {
        _printedMessageText = string.Empty;

        if (messageText != null)
        {
            messageText.text = string.Empty;
        }
    }

    private void PlayMaskAnimation(float targetProgress)
    {
        if (_maskAnimationCoroutine != null)
        {
            StopCoroutine(_maskAnimationCoroutine);
        }

        _maskAnimationCoroutine = StartCoroutine(AnimateMaskRoutine(targetProgress));
    }

    private IEnumerator AnimateMaskRoutine(float targetProgress)
    {
        if (maskRect == null)
        {
            yield break;
        }

        float startProgress = _openedMaskScale.y == 0f ? 0f : maskRect.localScale.y / _openedMaskScale.y;
        float duration = Mathf.Max(0f, maskAnimationDuration);

        if (duration == 0f)
        {
            SetMaskOpenProgress(targetProgress);
            _maskAnimationCoroutine = null;
            yield break;
        }

        for (float time = 0f; time < duration; time += Time.deltaTime)
        {
            float progress = Mathf.SmoothStep(startProgress, targetProgress, time / duration);
            SetMaskOpenProgress(progress);
            yield return null;
        }

        SetMaskOpenProgress(targetProgress);
        _maskAnimationCoroutine = null;
    }

    private void SetMaskOpenProgress(float progress)
    {
        if (maskRect == null)
        {
            return;
        }

        Vector3 scale = _openedMaskScale;
        scale.y = _openedMaskScale.y * Mathf.Clamp01(progress);
        maskRect.localScale = scale;
    }

    private void BlackBoxLog(string speakerName, string loggedMessageText)
    {
        Debug.Log($"[BlackBox] {speakerName}: {loggedMessageText}");
    }
}
