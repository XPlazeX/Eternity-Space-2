using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Doc : MonoBehaviour
{
    [SerializeField] private AssetReference _docPanelReference;
    [Header("Animations")]
    [SerializeField] private Transform _palmCatch;
    [SerializeField] private Transform _targetCatch;
    [SerializeField] private Transform _targetUncatch;
    [SerializeField] private float _catchSpeed;
    [SerializeField] private float _flySpeed;

    private bool _tiggered = false;
    private int startSortingLayerID = 0;
    private int startOrderInLayer = 0;
    private AsyncOperationHandle _docPanelOperation;

    private void OnEnable() 
    {
        StartCoroutine(Entering());
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && !_tiggered)
        {
            _tiggered = true;
            StopAllCoroutines();
            StartCoroutine(PlayerCatch());
        }
    }

    private IEnumerator PlayerCatch()
    {
        PlayerController.CanControl = false;
        TimeHandler.Resume(1f);
        TimeHandler.Workable = false;
        Player.CanAttack = false;

        Transform player = Player.PlayerTransform;
        SpriteRenderer spriteRenderer = Player.PlayerObject.GetComponent<SpriteRenderer>();

        startSortingLayerID = spriteRenderer.sortingLayerID;
        startOrderInLayer = spriteRenderer.sortingOrder;

        spriteRenderer.sortingLayerID = 0;
        spriteRenderer.sortingOrder = -1;

        player.GetComponent<Collider2D>().enabled = false;

        while ((_palmCatch.position - player.position).magnitude > 0.03f)
        {
            _palmCatch.Translate((player.position - _palmCatch.position) * _catchSpeed * Time.deltaTime);
            
            yield return new WaitForFixedUpdate();
        }

        while ((_targetCatch.position - player.position).magnitude > 0.03f)
        {
            player.Translate((_targetCatch.position - player.position) * _catchSpeed * Time.deltaTime);
            _palmCatch.position = player.position;
            
            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(ShowDocPanel());
    }

    private IEnumerator ShowDocPanel()
    {
        if (_docPanelOperation.IsValid())
        {
            Addressables.Release(_docPanelOperation);
        }

        _docPanelOperation = Addressables.LoadAssetAsync<GameObject>(_docPanelReference);
        yield return _docPanelOperation;

        TimeHandler.Pause();

        RectTransform docPanel = SceneStatics.UICore.GetComponent<UIPlacer>().PlaceInteractiveUI(((GameObject)_docPanelOperation.Result).GetComponent<RectTransform>(), Vector2.zero, true);

        docPanel.GetComponent<DocModuleChoice>().BindDoc(this);
        PlayerShipData.LoadHealth();
    }

    public void ChoiceMade()
    {
        print("uncatch");
        StartCoroutine(PlayerUncatch());
    }

    private IEnumerator PlayerUncatch()
    {
        Transform player = Player.PlayerTransform;
        SpriteRenderer spriteRenderer = Player.PlayerObject.GetComponent<SpriteRenderer>();

        Camera.main.GetComponent<CameraController>().SetScale(1f / 1.3f, 0.4f);
        TimeHandler.Resume(1f);

        while ((_targetUncatch.position - player.position).magnitude > 0.03f)
        {
            player.Translate((_targetUncatch.position - player.position) * _catchSpeed * Time.deltaTime);
            _palmCatch.position = player.position;
            
            yield return new WaitForFixedUpdate();
        }

        PlayerController.CanControl = true;
        TimeHandler.Workable = true;
        Player.CanAttack = true;

        spriteRenderer.sortingLayerID = startSortingLayerID;
        spriteRenderer.sortingOrder = startOrderInLayer;

        player.GetComponent<Collider2D>().enabled = true;

        while ((_palmCatch.position - _targetCatch.position).magnitude > 0.03f)
        {
            _palmCatch.Translate((_targetCatch.position - _palmCatch.position) * _catchSpeed * Time.deltaTime);
            
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
