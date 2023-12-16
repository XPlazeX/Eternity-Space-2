using System.Collections.Generic;
using UnityEngine;
using ModuleWork;

public class MissionEventsDistributor : MonoBehaviour
{
    const float module_service_offset = 10f;
    // Этот класс предоставляет загрузку и выбор событий на уровне, создавая объекты ModuleService
    // Все данные сохраняются в ModulasSave с помощью ModulasSaveHandler
    // Загрузку модулей осуществляет Module Core, модули беруться из  CHaracterModules который должен быть на CharacterCore
    // Временные модули убираются через GameSessionInfoHandler
    // Спрайты паков устанавливаются через PackRenderer!!!
    [SerializeField] private LevelEvent[] _levelEvents;
    [SerializeField] private List<int> DefaultExclusions = new List<int>();
    [SerializeField] private ModuleService[] moduleServices;
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
                PlaceEventRect(moduleServices[1], le, 0);
            else
                PlaceEventRect(moduleServices[0], le, 0);

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
                PlaceEventRect(moduleServices[1], le, i, customY);

            else
                PlaceEventRect(moduleServices[0], le, i, customY);
        }

        if (modulasSave.HasPack)
        {
            SceneStatics.SceneCore.GetComponent<MenuPackRenderer>().RenderPack(modulasSave.PackID);
        }
    }

    public void GetNewEventChoice(int maxEvents)
    {
        print($"### Начало выбора событий");
        List<int> Exclusions = new List<int>(DefaultExclusions);
        List<int> SelectedEvents = new List<int>();
        List<int> SelectionPool = new List<int>();

        ModulasSave modulasSave = ModulasSaveHandler.GetSave();

        Exclusions.AddRange(modulasSave.GetUniqueModulasID()); // исключаем уже имеющиеся уникальные модули

        for (int i = 0; i < _levelEvents.Length; i++) // исключаем неоткрытые модули и заполняем пул доступными ID
        {
            if (!_levelEvents[i].unlocked || !Bank.EnoughtCash(BankSystem.Currency.Aurite, _levelEvents[i].auritePrice) || Exclusions.Contains(i)
                || (_levelEvents[i].oneLeveled && GameSessionInfoHandler.LevelProgress == 0))
            {
                print($"Исключено событие: {i}");
                continue;
            }

            SelectionPool.Add(i);
        }
        print($"размер пула: {SelectionPool.Count}");
        if (SelectionPool.Count == 0)
        {
            modulasSave.InitEventChoice(SelectedEvents.ToArray());
            ModulasSaveHandler.RewriteSave(modulasSave);
            print($"### Перезаписан пустой пул");
            return;
        }
        while (SelectedEvents.Count < maxEvents) // выбираем ивенты
        {
            int selector = Random.Range(0, SelectionPool.Count); // выбираем из пула доступный ID

            SelectedEvents.Add(SelectionPool[selector]);
            print($"Выбрано событие: {SelectionPool[selector]}");

            if (_levelEvents[SelectionPool[selector]].stackType != ModuleStackType.Stacking) // убираем в случае чего ID события из пула
                SelectionPool.RemoveAt(selector);
        }

        modulasSave.InitEventChoice(SelectedEvents.ToArray());

        ModulasSaveHandler.RewriteSave(modulasSave);

        print($"### Перезапись выбора событий");
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
}
