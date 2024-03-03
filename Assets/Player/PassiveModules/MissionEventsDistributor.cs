using System.Collections.Generic;
using UnityEngine;
using ModuleWork;

public class MissionEventsDistributor : MonoBehaviour
{
    const float module_service_offset = 10f;
    const string service_type_collection_key = "ModuleServiceTypes";
    // Этот класс предоставляет загрузку и выбор событий на уровне, создавая объекты ModuleService
    // Все данные сохраняются в ModulasSave с помощью ModulasSaveHandler
    // Загрузку модулей осуществляет Module Core, модули беруться из  CHaracterModules который должен быть на CharacterCore
    // Временные модули убираются через GameSessionInfoHandler
    // Спрайты паков устанавливаются через PackRenderer!!!
    [SerializeField] private LevelEvent[] _levelEvents;
    [SerializeField] private List<int> DefaultExclusions = new List<int>();
    [SerializeField] private ModuleServiceOffer[] _commonModuleServices;
    [SerializeField] private ModuleServiceOffer[] _packModuleServices;
    [Space()]
    [SerializeField] private ModuleService _defaultCommonModuleService;
    [SerializeField] private ModuleService _defaultPackModuleService;
    [Space()]
    [SerializeField] private Transform _serviceParentTransform;
    [SerializeField] private Vector2 _startEventRectPosition;
    [SerializeField] private float _boostedEventYPosition;
    [Space()]
    [SerializeField] private bool _log;
    [Space()]
    [Header("Testing")]
    [SerializeField] private bool _testMode;
    [SerializeField] private int _testEventID = 1;

    private int _maxEvents = 1;
    private float _verticalStep = 0f;

    public void Start()
    {
        ContagionHandler.Initialize();

        if (_log)
        {
            ShowSaveModules();
        }
        if (_testMode)
        {
            LevelEvent le = _levelEvents[_testEventID];

            ModulasSave modulasSave = ModulasSaveHandler.GetSave();
            modulasSave.InitEventChoice(new int[1] {_testEventID});
            ModulasSaveHandler.RewriteSave(modulasSave);

            if (le.stackType == ModuleStackType.Pack)
                PlaceEventRect(_defaultPackModuleService, le, 0);
            else
                PlaceEventRect(_defaultCommonModuleService, le, 0);

            return;
        }
        LoadEvents((GameSessionInfoHandler.GetSessionSave().Boosted && GameSessionInfoHandler.GetSessionSave().CurrentLevel == 0));
    }

    public void LoadEvents(bool boosted)
    {
        if (!boosted && GameSessionInfoHandler.GetSessionSave().CurrentLevel == 0)
            return;

        ModulasSave modulasSave = ModulasSaveHandler.GetSave();

        if (boosted && !modulasSave.EventsLoaded)
        {
            GetNewEventChoice(_maxEvents + 1);
        }
        else if (!modulasSave.EventsLoaded && GameSessionInfoHandler.GetSessionSave().CurrentLevel != 0)
        {
            GetNewEventChoice(_maxEvents);
        }
        
        for (int i = 0; i < modulasSave.ActiveEventDatas.Length; i++)
        {
            LevelEvent le = _levelEvents[modulasSave.ActiveEventDatas[i].eventID];

            float customY = 0f;
            if (boosted && i == 0)
                customY = _boostedEventYPosition;

            if (le.stackType == ModuleStackType.Pack)
                PlaceEventRect(GetModuleServiceFromSave(i, true), le, i, customY);

            else
                PlaceEventRect(GetModuleServiceFromSave(i, false), le, i, customY);
        }

        if (modulasSave.HasPack)
        {
            SceneStatics.SceneCore.GetComponent<MenuPackRenderer>().RenderPack(modulasSave.PackID);
        }
    }

    public LevelEvent GetLevelEvent(int id)
    {
        return _levelEvents[id];
    }

    public void GetNewEventChoice(int maxEvents)
    {
        print($"### Начало выбора событий");
        GameSessionInfoHandler.RemoveDataCollection(service_type_collection_key);

        List<int> SelectedEvents = new List<int>();
        List<int> SelectionPool = new List<int>();

        ModulasSave modulasSave = ModulasSaveHandler.GetSave();

        SelectionPool = GetAvaiableEvents();

        print($"размер пула: {SelectionPool.Count}");
        if (SelectionPool.Count == 0)
        {
            modulasSave.InitEventChoice(SelectedEvents.ToArray());
            ModulasSaveHandler.RewriteSave(modulasSave);
            print($"### Перезаписан пустой пул");
            return;
        }

        SelectedEvents = SelectEventsFromPool(SelectionPool, maxEvents);

        modulasSave.InitEventChoice(SelectedEvents.ToArray());

        ModulasSaveHandler.RewriteSave(modulasSave);

        GameSessionInfoHandler.AddDataCollection(service_type_collection_key, GetServiceTypeListRandomized(SelectedEvents));

        print($"### Перезапись выбора событий");
    }

    public List<int> GetAvaiableEvents(bool auriteAvailable = true)
    {
        List<int> Exclusions = new List<int>(DefaultExclusions);
        List<int> SelectionPool = new List<int>();

        Exclusions.AddRange(ModulasSaveHandler.GetSave().GetUniqueModulasID()); // исключаем уже имеющиеся уникальные модули

        for (int i = 0; i < _levelEvents.Length; i++) // исключаем неоткрытые модули и заполняем пул доступными ID
        {
            if (!_levelEvents[i].unlocked 
            || (auriteAvailable ? !Bank.EnoughtCash(BankSystem.Currency.Aurite, _levelEvents[i].auritePrice) : false)
             || Exclusions.Contains(i)
                || (_levelEvents[i].oneLeveled && GameSessionInfoHandler.LevelProgress == 0))
            {
                print($"Исключено событие: {i}");
                continue;
            }

            SelectionPool.Add(i);
        }

        return SelectionPool;
    }

    public List<int> SelectEventsFromPool(List<int> eventPool, int targetCount)
    {
        List<int> SelectedEvents = new List<int>();
        List<int> SelectionPool = new List<int>(eventPool);

        if (SelectionPool.Count == 0)
        {
            Debug.Log("ВНИМАНИЕ! пустой пул событий");
        }

        while (SelectedEvents.Count < targetCount) // выбираем ивенты
        {
            int selector = Random.Range(0, SelectionPool.Count); // выбираем из пула доступный ID

            SelectedEvents.Add(SelectionPool[selector]);
            print($"Выбрано событие: {SelectionPool[selector]}");

            if (_levelEvents[SelectionPool[selector]].stackType != ModuleStackType.Stacking) // убираем в случае чего ID события из пула
                SelectionPool.RemoveAt(selector);
        }

        return SelectedEvents;
    }

    public void ShowSaveModules()
    {
        ModulasSave modulasSave = ModulasSaveHandler.GetSave();

        print("||||||||| постоянные модули |||||||||");
        for (int i = 0; i < modulasSave.PassiveEvents.Count; i++)
        {
            print(modulasSave.PassiveEvents[i].moduleOperandID);
        }
        print("--------- временные модули ----------");
        for (int i = 0; i < modulasSave.LevelEvents.Count; i++)
        {
            print(modulasSave.LevelEvents[i].moduleOperandID);
        }
        print("||||||||||| конец модулей |||||||||||");
    }

    private ModuleService GetModuleServiceFromSave(int eventID, bool isPack)
    {
        if (!GameSessionInfoHandler.ExistDataCollection(service_type_collection_key))
        {
            Debug.Log("Коллекция типа Модуль сервиса не существует. Используются модуль сервисы по умолчанию.");
            return isPack ? _defaultPackModuleService : _defaultCommonModuleService;
        }

        int result = GameSessionInfoHandler.GetDataCollection(service_type_collection_key)[eventID];

        if (result == -1)
        {
            return isPack ? _defaultPackModuleService : _defaultCommonModuleService;
        }

        return isPack ? _packModuleServices[result].moduleServiceSample : _commonModuleServices[result].moduleServiceSample;
    }

    private List<int> GetServiceTypeListRandomized(List<int> selectedEventIDs)
    {
        List<int> result = new List<int>();

        for (int i = 0; i < selectedEventIDs.Count; i++)
        {
            if (_levelEvents[selectedEventIDs[i]].stackType == ModuleStackType.Pack)
            {
                result.Add(GetModuleServiceIDRandomized(true, -1));
            } else
            {
                result.Add(GetModuleServiceIDRandomized(false, -1));
            }
        }

        return result;
    }

    private int GetModuleServiceIDRandomized(bool isPack, int defaultValue)
    {
        ModuleServiceOffer[] servicePool;

        if (isPack)
            servicePool = _packModuleServices;
        else
            servicePool = _commonModuleServices;

        for (int i = 0; i < servicePool.Length; i++)
        {
            if (Random.value < servicePool[i].chance)
            {
                return i;
            }
        }

        return defaultValue;
    }

    public void PlaceEventRect(ModuleService moduleService, LevelEvent levelEvent, int choiceID, float _customYposition = 0f)
    {
        ModuleService sampleEvent = Instantiate(moduleService);

        RectTransform serviceRect = sampleEvent.GetComponent<RectTransform>();
        serviceRect.transform.SetParent(_serviceParentTransform);
        serviceRect.transform.localScale = Vector3.one;

        if (_customYposition == 0f)
        {
            serviceRect.anchoredPosition = _startEventRectPosition - new Vector2(0f, _verticalStep);
            _verticalStep += moduleService.Height + module_service_offset;
        }
        else
            serviceRect.anchoredPosition = new Vector2(_startEventRectPosition.x, _customYposition);

        serviceRect.sizeDelta = new Vector2(1f, moduleService.Height);

        sampleEvent.Load(levelEvent, choiceID);
    }

    [System.Serializable]
    public struct ModuleServiceOffer
    {
        public ModuleService moduleServiceSample;
        [Range(0, 1f)]public float chance;
    }
}
