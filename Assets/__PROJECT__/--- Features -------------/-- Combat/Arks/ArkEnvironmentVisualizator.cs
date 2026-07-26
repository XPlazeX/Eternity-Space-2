
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArkEnvironmentVisualizator : MonoBehaviour
{
    private const string PAUSE_CODE = "ARK_ENVIRONMENT_VISUALIZATOR";

    public event System.Action EnterAnimationFinished;
    public event System.Action ExitAnimationFinished;

    [SerializeField] private GameObject keyObject;
    [Header("Initialize Animation")]
    [SerializeField] private float initAnimationTime = 1f;
    [SerializeField] private GameObject initObject;
    [SerializeField] private Image initFillProgress;
    [Header("Dive Animation")]
    [SerializeField] private ArkVisualLayer[] diveLayers;
    [SerializeField] private ArkCameraController arkCameraController;
    [Header("Exit Animation")]
    [SerializeField] private float exitAnimationTime = 1f;
    [SerializeField] private GameObject exitObject;
    [SerializeField] private Image exitFillProgress;

    public float AnimationTime => Mathf.Max(
        initAnimationTime + GetLayersAnimationTime(false),
        GetLayersAnimationTime(true) + exitAnimationTime);

    private Coroutine _animationCoroutine;
    private PauseHandle _worldPauseHandle;
    private PauseHandle _arkPauseHandle;

    private void Start() {
        _arkPauseHandle = ESTime.AcquirePause(new PauseRequest(TimeDomain.Ark, PAUSE_CODE));
    }

    public void EnterAnimation()
    {
        StartAnimation(EnterAnimationRoutine());
    }

    public void ExitAnimation()
    {
        StartAnimation(ExitAnimationRoutine());
    }

    private void StartAnimation(IEnumerator animation)
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(animation);
    }

    private IEnumerator EnterAnimationRoutine()
    {
        if (exitObject != null)
            exitObject.SetActive(false);
        if (initObject != null)
            initObject.SetActive(true);
        if (initFillProgress != null)
            initFillProgress.fillAmount = 0f;

        arkCameraController.Activate();

        float initialTimeScale = ESTime.worldTargetTimeScale;
        yield return AnimateProgress(initAnimationTime, progress =>
        {
            ESTime.worldTargetTimeScale = Mathf.Lerp(initialTimeScale, 0f, progress);
            if (initFillProgress != null)
                initFillProgress.fillAmount = progress;
        });

        ESTime.worldTargetTimeScale = 0f;
        if (_worldPauseHandle == null)
        {
            _worldPauseHandle = ESTime.AcquirePause(new PauseRequest(TimeDomain.World, PAUSE_CODE));
            ESTime.Release(_arkPauseHandle);
            _arkPauseHandle = null;
        }
            
        if (initObject != null)
            initObject.SetActive(false);

        yield return AnimateLayers(false);

        _animationCoroutine = null;
        EnterAnimationFinished?.Invoke();
    }

    private IEnumerator ExitAnimationRoutine()
    {
        if (initObject != null)
            initObject.SetActive(false);

        if (exitObject != null)
            exitObject.SetActive(true);
        if (exitFillProgress != null)
            exitFillProgress.fillAmount = 0f;
        yield return AnimateProgress(exitAnimationTime, progress =>
        {
            ESTime.worldTargetTimeScale = Mathf.Lerp(0f, 1f, progress);
            if (exitFillProgress != null)
                exitFillProgress.fillAmount = progress;
        });

        if (exitObject != null)
            exitObject.SetActive(false);

        yield return AnimateLayers(true);

        ReleaseWorldPause();

        

        ESTime.worldTargetTimeScale = 1f;
        _animationCoroutine = null;
        arkCameraController.Deactivate();
        ExitAnimationFinished?.Invoke();
    }

    private IEnumerator AnimateProgress(float duration, System.Action<float> update)
    {
        if (duration <= 0f)
        {
            update(1f);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            update(Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
    }

    private IEnumerator AnimateLayers(bool isExit)
    {
        float duration = GetLayersAnimationTime(isExit);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            for (int i = 0; i < diveLayers.Length; i++)
            {
                ArkVisualLayer layer = diveLayers[i];
                if (layer.renderer == null)
                    continue;

                float waitTime = isExit ? layer.exitWaitTime : layer.waitTime;
                if (elapsed < waitTime)
                    continue;
                float progress = layer.animationTime <= 0f
                    ? (elapsed >= waitTime ? 1f : 0f)
                    : Mathf.Clamp01((elapsed - waitTime) / layer.animationTime);

                if (!isExit && layer.isEnterKeyLayer && progress >= 1f)
                {
                    keyObject.SetActive(true);
                } else if (isExit && layer.isExitKeyLayer && progress >= 1f)
                {
                    keyObject.SetActive(false);
                }

                // Several timeline entries may control the same renderer. Only
                // the most recently started one is allowed to write its color.
                if (!IsLatestStartedLayerForRenderer(i, elapsed, isExit))
                    continue;

                Color from = isExit ? layer.endColor : layer.startColor;
                Color to = isExit ? layer.startColor : layer.endColor;
                layer.renderer.color = Color.Lerp(from, to, progress);
            }

            yield return null;
        }

        SetLayersToFinalColor(isExit);
    }

    private float GetLayersAnimationTime(bool isExit)
    {
        float duration = 0f;
        if (diveLayers == null)
            return duration;

        foreach (ArkVisualLayer layer in diveLayers)
        {
            float waitTime = isExit ? layer.exitWaitTime : layer.waitTime;
            duration = Mathf.Max(duration, waitTime + layer.animationTime);
        }

        return duration;
    }

    private void SetLayersToFinalColor(bool isExit)
    {
        if (diveLayers == null)
            return;

        for (int i = 0; i < diveLayers.Length; i++)
        {
            ArkVisualLayer layer = diveLayers[i];
            if (layer.renderer != null)
            {
                if (IsLatestStartedLayerForRenderer(i, float.PositiveInfinity, isExit))
                    layer.renderer.color = isExit ? layer.startColor : layer.endColor;
            }
        }
    }

    private bool IsLatestStartedLayerForRenderer(int layerIndex, float elapsed, bool isExit)
    {
        ArkVisualLayer candidate = diveLayers[layerIndex];
        float candidateWait = isExit ? candidate.exitWaitTime : candidate.waitTime;

        for (int i = 0; i < diveLayers.Length; i++)
        {
            if (i == layerIndex || diveLayers[i].renderer != candidate.renderer)
                continue;

            float otherWait = isExit ? diveLayers[i].exitWaitTime : diveLayers[i].waitTime;
            if (otherWait > elapsed)
                continue;

            if (otherWait > candidateWait || (otherWait == candidateWait && i > layerIndex))
                return false;
        }

        return true;
    }

    private void ReleaseWorldPause()
    {
        if (_worldPauseHandle == null)
            return;

        _arkPauseHandle = ESTime.AcquirePause(new PauseRequest(TimeDomain.Ark, PAUSE_CODE));
        ESTime.Release(_worldPauseHandle);
        _worldPauseHandle = null;
    }

    private void OnDisable()
    {
        bool controlledWorldTime = _animationCoroutine != null || _worldPauseHandle != null;

        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }

        ReleaseWorldPause();
        if (controlledWorldTime)
            ESTime.worldTargetTimeScale = 1f;
    }

    [System.Serializable]
    public struct ArkVisualLayer
    {
        public SpriteRenderer renderer;
        public float waitTime;
        public float exitWaitTime;
        public Color startColor;
        public Color endColor;
        public float animationTime;
        public bool isEnterKeyLayer;
        public bool isExitKeyLayer;
    }
}
