using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class CharacterLoader : MonoBehaviour
{
    public enum CharacterLoadType
    {
        NoneLoad = 0,
        GameLoad = 1,
        MissionMenuLoad = 2
    }

    [SerializeField] private AssetReference[] _characters;
    [SerializeField] private bool _testMode;
    [SerializeField] private int _testID;

    private AsyncOperationHandle _characterOperationHandle;

    public Character ActiveCharacterSample {get; set;}

    public IEnumerator LoadingPlayerShip(CharacterLoadType loadType = CharacterLoadType.GameLoad)
    {
        int id = GameSessionInfoHandler.GetSessionSave().ShipModel;

        int missionCustomId = GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>()._activeMissionSample.CustomShip;

        if (missionCustomId != -1)
            id = missionCustomId;

        if (_testMode)
            id = _testID;

        yield return StartCoroutine(LoadingCharacter(id, loadType));
    }

    public IEnumerator LoadingPlayerShip(int id, CharacterLoadType loadType = CharacterLoadType.GameLoad)
    {
        yield return StartCoroutine(LoadingCharacter(id, loadType));
    }

    public IEnumerator WritingShipHPData(int characterID, float hpPart = 1f)
    {
        if (_characterOperationHandle.IsValid())
        {
            Addressables.Release(_characterOperationHandle);
        }

        var characterReference = _characters[characterID];

        _characterOperationHandle = Addressables.LoadAssetAsync<Character>(characterReference);
        yield return _characterOperationHandle;

        int hp = ((Character)_characterOperationHandle.Result).HP;

        GameSessionSave save = GameSessionInfoHandler.GetSessionSave();

        save.MaxHealth = hp;
        save.HealthPoints = Mathf.CeilToInt((float)hp * hpPart);
        Debug.Log($"written character hp: {hp}");

        GameSessionInfoHandler.RewriteSessionSave(save);
    }

    private IEnumerator LoadingCharacter(int characterID, CharacterLoadType loadType = CharacterLoadType.GameLoad)
    {
        if (_characterOperationHandle.IsValid())
        {
            Addressables.Release(_characterOperationHandle);
        }

        var characterReference = _characters[characterID];

        _characterOperationHandle = Addressables.LoadAssetAsync<Character>(characterReference);
        yield return _characterOperationHandle;

        ActiveCharacterSample = (Character)_characterOperationHandle.Result;
        Debug.Log($"Active char sample: {ActiveCharacterSample == null}");

        if (loadType == CharacterLoadType.GameLoad)
            LoadCharacter(ActiveCharacterSample, characterID);

        else if (loadType == CharacterLoadType.MissionMenuLoad)
            MissionMenuLoadCharacter(ActiveCharacterSample);
    }

    private void LoadCharacter(Character character, int id)
    {
        // int id = 0;
        // if (Dev.RuStoreVersionSprites)
        //     id = 1;
        Player.Initialize(character.GetSkinnedShip(Skins.SOCurrentSkin()), character.Class);
        SceneStatics.SceneCore.GetComponent<PlayerShipData>().Initialize(character.HP, character.ARM, id);

        for (int i = 0; i < character.HandingModules.Length; i++)
        {
            ModuleCore.SpawnModule(character.HandingModules[i]);
        }
    }

    private void MissionMenuLoadCharacter(Character character)
    {
        for (int i = 0; i < character.HandingModules.Length; i++)
        {
            character.HandingModules[i].MissionMenuLoad();
        }
    }
}
