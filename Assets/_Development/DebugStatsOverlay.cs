using UnityEngine;

public class DebugStatsOverlay : MonoBehaviour
{
    [Header("Toggle")]
    [SerializeField] private KeyCode toggleKey = KeyCode.F;
    [SerializeField] private bool visible = true;

    [Header("Window")]
    [SerializeField] private Vector2 startPosition = new Vector2(10f, 10f);
    [SerializeField] private float width = 260f;
    [SerializeField] private float lineHeight = 22f;
    [SerializeField] private int fixedHistorySize = 10;

    private float fps;
    private float fpsSmoothVelocity;

    private int fixedUpdatesThisSecond;
    private int fixedUpdatesPerSecond;
    private float fixedSecondTimer;

    private float[] fixedDurationsMs;
    private int fixedDurationIndex;
    private int fixedDurationCount;

    private GUIStyle labelStyle;
    private GUIStyle boxStyle;

    private void Awake()
    {
        if (fixedHistorySize < 1)
            fixedHistorySize = 1;

        fixedDurationsMs = new float[fixedHistorySize];
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            visible = !visible;

        // Плавный FPS
        float currentFps = 1f / Mathf.Max(Time.unscaledDeltaTime, 0.0001f);
        fps = Mathf.SmoothDamp(fps, currentFps, ref fpsSmoothVelocity, 0.15f);

        // FixedUpdate в секунду
        fixedSecondTimer += Time.unscaledDeltaTime;
        if (fixedSecondTimer >= 1f)
        {
            fixedUpdatesPerSecond = fixedUpdatesThisSecond;
            fixedUpdatesThisSecond = 0;
            fixedSecondTimer -= 1f;
        }
    }

    private void FixedUpdate()
    {
        fixedUpdatesThisSecond++;

        // Примерная длительность одного fixed-тика в миллисекундах.
        // Это не "CPU time метода", а длительность шага симуляции.
        float durationMs = Time.fixedDeltaTime * 1000f;

        fixedDurationsMs[fixedDurationIndex] = durationMs;
        fixedDurationIndex = (fixedDurationIndex + 1) % fixedDurationsMs.Length;

        if (fixedDurationCount < fixedDurationsMs.Length)
            fixedDurationCount++;
    }

    private void OnGUI()
    {
        if (!visible)
            return;

        EnsureStyles();

        float x = startPosition.x;
        float y = startPosition.y;
        float height = (4 + fixedDurationCount) * lineHeight + 16f;

        GUI.Box(new Rect(x, y, width, height), GUIContent.none, boxStyle);

        float textX = x + 10f;
        float textY = y + 8f;

        GUI.Label(new Rect(textX, textY, width - 20f, lineHeight), $"FPS: {fps:0.0}", labelStyle);
        textY += lineHeight;

        GUI.Label(new Rect(textX, textY, width - 20f, lineHeight), $"FixedUpdate/sec: {fixedUpdatesPerSecond}", labelStyle);
        textY += lineHeight;

        GUI.Label(new Rect(textX, textY, width - 20f, lineHeight), $"Last {fixedDurationCount} FixedUpdate durations:", labelStyle);
        textY += lineHeight;

        for (int i = 0; i < fixedDurationCount; i++)
        {
            int index = fixedDurationIndex - 1 - i;
            if (index < 0)
                index += fixedDurationsMs.Length;

            GUI.Label(
                new Rect(textX, textY, width - 20f, lineHeight),
                $"{i + 1}. {fixedDurationsMs[index]:0.000} ms",
                labelStyle
            );

            textY += lineHeight;
        }
    }

    private void EnsureStyles()
    {
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                richText = false
            };
            labelStyle.normal.textColor = Color.white;
        }

        if (boxStyle == null)
        {
            boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.alignment = TextAnchor.UpperLeft;
            boxStyle.fontSize = 14;
        }
    }
}