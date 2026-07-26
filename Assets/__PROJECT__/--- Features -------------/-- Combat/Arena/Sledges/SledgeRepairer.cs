using UnityEngine;

public class SledgeRepairer : MonoBehaviour
{
    [SerializeField] private SledgeCore sledgeCore;
    [SerializeField] private Transform pivot;
    [SerializeField] private float connectionRadius = 2f;
    [SerializeField] private float connectionTime = 5f;
    [Header("Visual")]
    [SerializeField] private CircleLineRenderer radiusRenderer;
    [SerializeField] private Transform tower;
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;
    [SerializeField] private float topArmAngle = 45f;
    [SerializeField] private LineRenderer leftLR;
    [SerializeField] private LineRenderer rightLR;
    [SerializeField] private Gradient armConnectionGradient;
    [SerializeField] private Gradient circleConnectionGradient;
    [SerializeField] private Color resetArmColor;
    [SerializeField] private Color readyCircleColor;
    [SerializeField] private Color notreadyCircleColor;

    private float _connectionProgress = 0f;
    private bool _connected = false;
    private bool _released = false;

    public float ConnectionProgress => _connectionProgress;

    private void OnEnable() {
        SledgeCore.RepairChanged += OnRepairChargeChanged;
    }

    void OnDisable()
    {
        SledgeCore.RepairChanged -= OnRepairChargeChanged;
    }

    private void Start() {
        ResetRenderers();
    }

    private void Update() 
    {
        Transform player = Player.PlayerTransform;

        if (player == null) return;

        if (!_connected && (player.position - pivot.position).magnitude <= connectionRadius)
        {
            StartConnection();
        }   
        else if (_connected && (player.position - pivot.position).magnitude > connectionRadius)
        {
            LoseConnection();
            ResetRenderers();
        }

        if (_connected && SledgeCore.HasCharge && !_released)
        {
            ProgressConnection(ESTime.worldDeltaTime);
        }

        tower.up = player.position - tower.position;
    }

    private void ProgressConnection(float dt)
    {
        _connectionProgress += dt;

        if (!_released && _connectionProgress >= connectionTime)
        {
            ReleaseConnection();
            return;
        }

        ProgressRenderers(_connectionProgress / connectionTime);
    }

    private void StartConnection()
    {
        _connected = true;
    }

    private void LoseConnection()
    {
        _connectionProgress = 0f;
        _connected = false;
        _released = false;

        ResetRenderers();
    }

    private void ReleaseConnection()
    {
        _released = true;
        sledgeCore.ReleaseCharge();
        ResetRenderers();
    }

    private void OnRepairChargeChanged(int ch)
    {
        if (!_connected)
        {
            radiusRenderer.SetColor(SledgeCore.HasCharge ? readyCircleColor : notreadyCircleColor);
        }
    }


    private void ProgressRenderers(float t)
    {
        t = Mathf.Clamp01(t);

        leftLR.startColor = armConnectionGradient.Evaluate(t);
        rightLR.startColor = armConnectionGradient.Evaluate(t);
        radiusRenderer.SetColor(circleConnectionGradient.Evaluate(t));

        leftArm.eulerAngles = new Vector3(0f, 0f, tower.eulerAngles.z + Mathf.Lerp(0f, topArmAngle, 1f - t));
        rightArm.eulerAngles = new Vector3(0f, 0f, tower.eulerAngles.z - Mathf.Lerp(0f, topArmAngle, 1f - t));
    }

    private void ResetRenderers()
    {
        leftLR.startColor = resetArmColor;
        rightLR.startColor = resetArmColor;
        radiusRenderer.SetColor(SledgeCore.HasCharge ? readyCircleColor : notreadyCircleColor);

        leftArm.eulerAngles = new Vector3(0f, 0f, topArmAngle);
        rightArm.eulerAngles = new Vector3(0f, 0f, -topArmAngle);

        radiusRenderer.SetRadius(connectionRadius);
    }
}
