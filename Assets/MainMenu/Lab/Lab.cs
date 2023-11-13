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
    [Space]
    [SerializeField] private AssetReference[] _labCategories;

    private AsyncOperationHandle _categoryLoadingHandle;
    private LabCategory _activeCategory;
    private Blueprint _activeBlueprint;

    public void OpenCategory(int id)
    {
        StartCoroutine(LoadingCategory(id));
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
        _labBlueprint.Block();
        _controls.interactable = false;
        if (_categoryLoadingHandle.IsValid())
        {
            Addressables.Release(_categoryLoadingHandle);
        }

        var categoryReference = _labCategories[id];

        _categoryLoadingHandle = Addressables.LoadAssetAsync<LabCategory>(categoryReference);
        yield return _categoryLoadingHandle;

        _activeCategory = (LabCategory)_categoryLoadingHandle.Result;
        SelectBlueprint(_activeCategory.SelectBlueprint(0));
        _controls.interactable = true;
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
}
