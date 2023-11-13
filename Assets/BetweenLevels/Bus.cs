﻿using System.Collections;
using UnityEngine;

public class Bus : MonoBehaviour
{
    [SerializeField] private Transform _bus;
    [SerializeField] private Transform _target;
    [SerializeField] private float _catchSpeed;
    [SerializeField] private float _busAcceleration;
    [SerializeField] private LaserAttackModule _laserModule;
    //[SerializeField] private GearBarterHandler _gearBarterHandler;
    [SerializeField] private int _gateChoice;
    [Header("Animations")]
    [SerializeField] private float _flySpeed;

    //public int InitID {get; private set;} = -1;

    //private IEnumerator _controllingBus;
    private bool _tiggered = false;

    //public void Initialize(int id) => InitID = id;

    private void OnEnable() {
        StartCoroutine(Entering());
        //SceneStatics.SceneCore.GetComponent<GateHandler>().BusConnected += OnOtherBusConnected;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !_tiggered)
        {
            _tiggered = true;
            StopAllCoroutines();
            StartCoroutine(PlayerCatch());
        }
    }

    private void OnOtherBusConnected()
    {
        if (!_tiggered)
            StartCoroutine(FlyAway());
    }

    private IEnumerator PlayerCatch()
    {
        PlayerShipData.DeactivateAllBindedSystems();
        TimeHandler.Resume();

        Transform player = Player.PlayerTransform;

        player.GetComponent<Collider2D>().enabled = false;

        while ((transform.position - player.position).magnitude > 0.01f)
        {
            player.Translate((transform.position - player.position) * _catchSpeed * Time.deltaTime);
            
            yield return new WaitForFixedUpdate();
        }

        //_controllingBus = ControllingBus();

        for (int i = 0; i < player.childCount; i++)
        {
            Destroy(player.GetChild(i).gameObject);
        }

        //GameSessionInfoHandler.AddDataCollection("BusChoice", new List<int>() {InitID});

        //StartCoroutine(_controllingBus);
        Player.PlayerTransform.SetParent(_bus);
        StartEngine();
    }

    public void StartEngine()
    {
        //GameSessionInfoHandler.DischargeLevel();

        SceneTransition.BlockUI();
        //StopCoroutine(_controllingBus);

        // запись индекса уровня

        Camera.main.GetComponent<CameraController>().CanMoving = false;
        PlayerController.CanControl = false;
        _target.gameObject.SetActive(false);

        StartCoroutine(Transition());
    }

    private IEnumerator Transition()
    {
        if (_laserModule != null)
            _laserModule.HandFire();

        yield return new WaitForSeconds(1f);

        yield return FlyAway(true);

        VictoryHandler.VictorySession();
    }

    private IEnumerator FlyAway(bool shaking = false)
    {
        float animationTimer = 2.3f;
        float speedPower = 1f;

        while (animationTimer > 0)
        {
            speedPower += Time.deltaTime * _busAcceleration;

            _bus.position += (Vector3.up) * speedPower * Time.deltaTime;
            if (shaking)
                CameraController.Shake(0.075f);
            
            animationTimer -= Time.deltaTime;

            yield return new WaitForFixedUpdate();
            
        }
    }

    private IEnumerator Entering()
    {
        Vector3 toPosition = transform.position;
        Vector3 fromPosition = new Vector3(toPosition.x, CameraController.Borders_xXyY.z - 7f, 0f);

        transform.position = fromPosition;
        while ((transform.position - toPosition).magnitude > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, toPosition, _flySpeed * Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }
    }

    
}
