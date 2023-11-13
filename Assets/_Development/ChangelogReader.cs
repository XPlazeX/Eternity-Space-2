using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ChangelogReader : MonoBehaviour
{
    [SerializeField] private AssetReference _changelogTextAsset;
    
    private AsyncOperationHandle _currentchangelogTextAssetOperationHandle;

    private void Start() {
        StartCoroutine(OpenDialogueInternal());
    }

    private IEnumerator OpenDialogueInternal()
    {
        if (_currentchangelogTextAssetOperationHandle.IsValid())
        {
            Addressables.Release(_currentchangelogTextAssetOperationHandle);
        }

        var changelogReference = _changelogTextAsset;
        _currentchangelogTextAssetOperationHandle = changelogReference.LoadAssetAsync<TextAsset>();
        yield return _currentchangelogTextAssetOperationHandle;
        if (_currentchangelogTextAssetOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            TextAsset changelog = (TextAsset)_currentchangelogTextAssetOperationHandle.Result;

            GetComponent<Text>().text = changelog.text;
        }
    }
}
