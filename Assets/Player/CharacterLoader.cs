using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class CharacterLoader : MonoBehaviour
{
    [SerializeField] private AssetReference[] _characters;
    [SerializeField] private bool _testMode;
    [SerializeField] private int _testID;

    private AsyncOperationHandle _characterOperationHandle;

    public IEnumerator LoadingPlayerShip()
    {
        int id = GameSessionInfoHandler.GetSessionSave().ShipModel;

        if (_testMode)
            id = _testID;

        Mission activeMission = GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>()._activeMissionSample;

        if (activeMission != null && activeMission.CustomShip != -1)
            id = activeMission.CustomShip;

        yield return StartCoroutine(LoadingCharacter(id));
    }

    private IEnumerator LoadingCharacter(int characterID)
    {
        if (_characterOperationHandle.IsValid())
        {
            Addressables.Release(_characterOperationHandle);
        }

        var characterReference = _characters[characterID];

        _characterOperationHandle = Addressables.LoadAssetAsync<Character>(characterReference);
        yield return _characterOperationHandle;

        LoadCharacter((Character)_characterOperationHandle.Result);
    }

    private void LoadCharacter(Character character)
    {
        int id = 0;
        if (Dev.RuStoreVersionSprites)
            id = 1;
        Player.Initialize(character.GetSkinnedShip(id));
        SceneStatics.SceneCore.GetComponent<PlayerShipData>().Initialize(character.HP, character.ARM);

        for (int i = 0; i < character.HandingModules.Length; i++)
        {
            ModuleCore.SpawnModule(character.HandingModules[i]);
        }
    }
}
