using System.Collections;
using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    public enum CharacterLoadType
    {
        NoneLoad = 0,
        GameLoad = 1,
        MissionMenuLoad = 2
    }

    [SerializeField] private Character _selectedCharacter;
    [SerializeField] private Character[] _characters;
    [SerializeField] private int _selectedCharacterID;
    [SerializeField] private int _selectedSkinID;
    [SerializeField] private int _startHitPoints = 100;

    public Character ActiveCharacterSample { get; private set; }

    public IEnumerator LoadingPlayerShip(CharacterLoadType loadType = CharacterLoadType.GameLoad)
    {
        LoadSelectedCharacter(loadType);
        yield break;
    }

    public IEnumerator LoadingPlayerShip(int id, CharacterLoadType loadType = CharacterLoadType.GameLoad)
    {
        LoadSelectedCharacter(loadType, id);
        yield break;
    }

    public IEnumerator WritingShipHPData(int characterID, float hpPart = 1f)
    {
        // Legacy save-writing hook is intentionally disabled for the PC rebuild.
        yield break;
    }

    public void LoadSelectedCharacter(CharacterLoadType loadType = CharacterLoadType.GameLoad)
    {
        LoadSelectedCharacter(loadType, _selectedCharacterID);
    }

    private void LoadSelectedCharacter(CharacterLoadType loadType, int characterID)
    {
        Character character = ResolveCharacter(characterID);

        if (character == null)
        {
            Debug.LogError($"{nameof(CharacterLoader)}: no character selected.");
            return;
        }

        ActiveCharacterSample = character;

        if (loadType == CharacterLoadType.GameLoad)
        {
            LoadCharacter(character, characterID);
        }
        else if (loadType == CharacterLoadType.MissionMenuLoad)
        {
            MissionMenuLoadCharacter(character);
        }
    }

    private Character ResolveCharacter(int characterID)
    {
        if (_selectedCharacter != null)
        {
            return _selectedCharacter;
        }

        if (_characters == null || _characters.Length == 0)
        {
            return null;
        }

        int safeCharacterID = Mathf.Clamp(characterID, 0, _characters.Length - 1);
        return _characters[safeCharacterID];
    }

    private void LoadCharacter(Character character, int id)
    {
        GameObject playerPrefab = character.GetSkinnedShip(_selectedSkinID);
        if (playerPrefab == null)
        {
            Debug.LogError($"{nameof(CharacterLoader)}: selected character has no player prefab.");
            return;
        }

        Player.Initialize(playerPrefab, character.Class);

        PlayerShipData playerShipData = FindAnyObjectByType<PlayerShipData>();
        if (playerShipData != null)
        {
            playerShipData.Initialize(_startHitPoints, 0, id);
        }

        // Legacy module auto-spawn is disabled while the project is being rebuilt.
        // for (int i = 0; i < character.HandingModules.Length; i++)
        // {
        //     ModuleCore.SpawnModule(character.HandingModules[i]);
        // }
    }

    private void MissionMenuLoadCharacter(Character character)
    {
        // Legacy mission-menu module loading is disabled.
        // for (int i = 0; i < character.HandingModules.Length; i++)
        // {
        //     character.HandingModules[i].MissionMenuLoad();
        // }
    }
}
