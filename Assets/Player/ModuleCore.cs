using System.Collections.Generic;
using UnityEngine;
using ModuleWork;

public class ModuleCore : MonoBehaviour
{
    public delegate void InitializationHandler();
    public static event InitializationHandler moduleCoreInitialized;
    
    [SerializeField] private bool _testMode;
    [SerializeField] private AttackPattern _testWeaponPattern;
    [SerializeField] private Device _testDevice;
    [SerializeField] private Module[] _testLoadingModules;
    //[SerializeField] private Ultratech _defaultUltratech;

    private static CharacterModules _characterModules;
    //private List<PassiveModule> LoadedModules = new List<PassiveModule>();
    private AttackPattern _defaultWeaponPattern;
    private Device _defaultDevice;

    public bool StartInitialization()
    {
        _characterModules = GetComponent<CharacterModules>();

        Initialize(GameSessionInfoHandler.GetSessionSave());

        return true;
    }

    public void Initialize(GameSessionSave save)
    {
        GetComponent<CharacterBulletDatabase>().Initialize();
        GetComponent<ParryingHandler>().Initialize();

        if (_testMode)
            TestLoadModules();
        else
            LoadAllSaveModules(save);

        PlayerShipData.PlayerDeath += NeverReloadMainWeapon;
        Player.StartPlayerReturn += ReloadMainWeapon;

        moduleCoreInitialized?.Invoke();
    }

    private void LoadAllSaveModules(GameSessionSave save)
    {
        string weaponModel = save.WeaponModel;
        Mission activeMission = GameObject.FindWithTag("BetweenScenes").GetComponent<MissionsDatabase>()._activeMissionSample;

        if (!char.IsDigit(save.WeaponModel[save.WeaponModel.Length - 1]))
            weaponModel = save.WeaponModel.ToString() + "-" + (save.WeaponLevel + GlobalSaveHandler.GetSave().WeaponStartLevel).ToString();

        if (!string.IsNullOrEmpty(activeMission.CustomWeaponModel))
            weaponModel = activeMission.CustomWeaponModel;

        _defaultWeaponPattern = SpawnGear(GearType.Weapon, weaponModel) as AttackPattern;
        
        List<LevelEvent> levelEvents = ModulasSaveHandler.GetSave().GetAllLevelEvents();

        print("HHHHHHHHHH-ЗАГРУЗКА МОДУЛЕЙ ИЗ СОХРАНЕНИЯ-HHHHHHHHHH");
        for (int i = 0; i < levelEvents.Count; i++)
        {
            SpawnModule(levelEvents[i].moduleOperandID);
            print($"Загружен модуль из сохранения ID={levelEvents[i].moduleOperandID}");
        }
        print("HHHHHHHHHH-КОНЕЦ ЗАГРУЗКИ МОДУЛЕЙ ИЗ СОХРАНЕНИЯ-HHHHH");
    }

    private void TestLoadModules()
    {
        _defaultWeaponPattern = (AttackPattern)SpawnGear(_testWeaponPattern);

        for (int i = 0; i < _testLoadingModules.Length; i++)
        {
            SpawnModule(_testLoadingModules[i]);
            print($"Загружен тестовый модуль NUM={i}");
        }
    }

    public Gear SpawnGear(GearType type, string gearName)
    {
        Gear gear = Instantiate(_characterModules.GetGear(type, gearName), transform.position, Quaternion.identity);
        gear.transform.SetParent(transform);

        Gear[] _gearComponents = gear.GetComponents<Gear>();
        for (int i = 0; i < _gearComponents.Length; i++)
        {
            //_gearComponents[i].Initialize(0);
            _gearComponents[i].Load();
        }

        return gear;
    }

    private Gear SpawnGear(Gear g)
    {
        Gear gear = Instantiate(g, transform.position, Quaternion.identity);
        gear.transform.SetParent(transform);

        Gear[] _gearComponents = gear.GetComponents<Gear>();
        for (int i = 0; i < _gearComponents.Length; i++)
        {
            //_gearComponents[i].Initialize(0);
            _gearComponents[i].Load();
        }

        return gear;
    }

    public static Module SpawnModule(int id, bool autoLoad = true)
    {
        Module m = Instantiate(_characterModules.GetModule(id));
        if (autoLoad)
            m.Load();
        return m;
    }

    public static void SpawnModule(Module module, bool autoLoad = true)
    {
        Module m = Instantiate(module) as Module;
        m.transform.SetParent(SceneStatics.CharacterCore.transform);

        if (autoLoad)
            m.Load();

        print($"spawn module : {m.gameObject.name}");
    }

    public void ReloadMainWeapon() => _defaultWeaponPattern.Load();

    private void NeverReloadMainWeapon() => Player.StartPlayerReturn -= ReloadMainWeapon;

}



