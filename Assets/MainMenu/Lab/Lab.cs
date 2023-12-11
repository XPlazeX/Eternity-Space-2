using UnityEngine;
using BankSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class Lab : MonoBehaviour
{
    [SerializeField] private LabBlueprint _labBlueprint;
    [SerializeField] private CanvasGroup _controls;
    [SerializeField] private GameObject _asquiredPanel;
    [SerializeField] private LabLamps _labLamps;
    [Space]
    [SerializeField] private AssetReference[] _labCategories;

    private AsyncOperationHandle _categoryLoadingHandle;
    private LabCategory _activeCategory;
    private Blueprint _activeBlueprint;
    private bool _categoryLoaded;
    private int _loadingCategoryID = -1;

    private void Start() {
        BlockControls();
    }

    public void LoadCategory(int id)
    {
        if (_loadingCategoryID == id)
            return;

        if (id >= _labCategories.Length)
        {
            BlockControls();
            return;
        }

        StartCoroutine(LoadingCategory(id));
        _loadingCategoryID = id;
    }

    public void OpenActiveCategory()
    {
        if (!_categoryLoaded)
            return;

        SelectBlueprint(_activeCategory.SelectBlueprint(0));
        _labLamps.PlaceLamps(_activeCategory);
    }

    public void TryBuy()
    {
        if (!Bank.EnoughtCash(Currency.Cosmilite, _activeBlueprint.cosPrice) || !Bank.EnoughtCash(Currency.Positronium, _activeBlueprint.posPrice))
            return;

        if (Unlocks.HasUnlock(_activeBlueprint.handingID))
            return;

        Bank.ConsumeCash(Currency.Cosmilite, _activeBlueprint.cosPrice);
        Bank.ConsumeCash(Currency.Positronium, _activeBlueprint.posPrice);

        Unlocks.NewUnlock(_activeBlueprint.handingID);

        SelectBlueprint(_activeBlueprint);
        _asquiredPanel.SetActive(true);
        print($"> успешная покупка чертежа {_activeBlueprint.handingID}!");
    }

    private IEnumerator LoadingCategory(int id)
    {
        BlockControls();
        if (_categoryLoadingHandle.IsValid())
        {
            Addressables.Release(_categoryLoadingHandle);
        }

        var categoryReference = _labCategories[id];

        _categoryLoadingHandle = Addressables.LoadAssetAsync<LabCategory>(categoryReference);
        yield return _categoryLoadingHandle;

        if (_loadingCategoryID == -1)
            yield break;

        _activeCategory = (LabCategory)_categoryLoadingHandle.Result;
        _controls.interactable = true;
        _categoryLoaded = true;
    }

    private void SelectBlueprint(Blueprint bp)
    {
        _activeBlueprint = bp;
        _labBlueprint.SetData(_activeBlueprint);
    }

    public void NextBlueprint()
    {
        SelectBlueprint(_activeCategory.NextBlueprint());
    }

    public void PreviousBlueprint()
    {
        SelectBlueprint(_activeCategory.PreviousBlueprint());
    }

    public void BlockControls()
    {
        _labBlueprint.Block();
        _controls.interactable = false;
        _categoryLoaded = false;
        _loadingCategoryID = -1;
    }
}
