using System.Collections.Generic;
using ScenarioSystem;
using UnityEngine;

public class ScenarioRunner : MonoBehaviour
{
    public event System.Action ScenarioAssetChanged;

    [SerializeField] private ScenarioAsset scenarioAsset;
    [SerializeField] private EncounterDirector encounterDirector;
    [SerializeField] private SledgeTransitor sledgeTransitor;
    [SerializeField] private float conditionsCheckFrequency = 6f;
    [SerializeField] private bool debugMessages = true;

    private ScenarioAsset RunningScenario {get; set;}
    private List<NodeData> RunningNodes {get; set;} = new();
    private Dictionary<string, ActiveNodeLogicRuntime> RunningNodeLogics {get; set;} = new();
    private ActiveTransitionRuntime RunningTransition {get; set;}

    private List<NodeData> _requestedToStartNodes = new();

    public bool Running {get; private set;} = false;
    public bool Paused {get; private set;} = false;

    private ScenarioContext _context;

    private int _lastNodeIndex = -1;
    private int _previousNodeIndex = -1;
    // private int _checkpointNodeIndex = -1;
    private float _lastNodeTimer;
    private float _scenarioTimer;
    private Dictionary<string, int> _nodeIndexes;
    private float _conditionsCheckTimer = 0f;

    public void SetScenarioAsset(ScenarioAsset asset)
    {
        if (Running)
        {
            Debug.LogError("Нельзя изменять сценарий, когда другой сценарий уже запущен.");
            return;
        }
        scenarioAsset = asset;
        ScenarioAssetChanged?.Invoke();

        _nodeIndexes = new Dictionary<string, int>();
        
        for (int i = 0; i < asset.nodes.Count; i++)
        {
            _nodeIndexes[asset.nodes[i].id] = i;
        }

        _scenarioTimer = 0f;
        if (Map.CurrentSector == null)
        {
            Debug.LogError("Невозможно установить сценарий: Map.CurrentSector пуст.");
        }
        _context = new ScenarioContext
        {
            EncounterDirector = encounterDirector,
            Sector = Map.CurrentSector,
            EnemyHq = FindAnyObjectByType<EnemyHQ>()
        };

        RebuildContext();
        DebugLog($"Установлен ассет сценария");
    }

    private void DebugLog(string msg)
    {
        if (!debugMessages) return;

        Debug.Log($"<color=orange>[SCENARIO RUNNER]: {msg}</color>.");
    }

    public void StartRunning()
    {
        if (scenarioAsset == null)
        {
            Debug.LogError("Невозможно запустить сценарий: сценарий отстутствует (null)");
            return;
        }
        RunningScenario = scenarioAsset;
        Running = true;
        DebugLog($"Сценарий запущен");
    }

    public void SetPaused(bool pause)
    {
        Paused = pause;
    }

    void Update()
    {
        if (!Running || Paused) return;

        Tick(ESTime.worldDeltaTime);
        _lastNodeTimer += ESTime.worldDeltaTime;
        _scenarioTimer += ESTime.worldDeltaTime;
    }

    private int ScenarioIndexOf(NodeData nodeData)
    {
        return _nodeIndexes[nodeData.id];
    }

    private void Tick(float dt)
    {
        // тики
        TickTransition(dt);
        TickNodeLogics(dt);

        RebuildContext();

        _conditionsCheckTimer -= dt;

        if (_conditionsCheckTimer <= 0f)
        {
            CheckConditions();
            _conditionsCheckTimer = 1f / conditionsCheckFrequency;
        }
    }

    private void CheckConditions()
    {
        // проверка условий начала
        List<NodeData> checkingNodes = new List<NodeData>(); // смотрим следующую ноду

        for (int i = 0; i < RunningScenario.nodes.Count; i++)
        {
            NodeData nodeData = RunningScenario.nodes[i];

            if (nodeData.disposable && _context.CompletedNodeSet.Contains(ScenarioIndexOf(nodeData)))
            {
                continue; // нода disposable и уже однажды была пройденна.
            }

            checkingNodes.Add(nodeData);
        }

        for (int i = 0; i < checkingNodes.Count; i++) // запускаем ноды, если условия старта удовлетворительны
        {
            NodeData nodeData = checkingNodes[i];

            if (nodeData.startConditions.Evaluate(_context))
            {
                RequestTransitionToNode(nodeData);
            }
        }

        // проверка условий конца
        for (int i = RunningNodes.Count - 1; i >= 0; i--)
        {
            NodeData nodeData = RunningNodes[i];
            if (nodeData.endConditions.Evaluate(_context))
            {
                ReleaseNode(RunningNodes[i]);
            }
        }
    }

    private void RequestTransitionToNode(NodeData nodeData)
    {
        if (nodeData.disposable && _context.CompletedNodeSet.Contains(ScenarioIndexOf(nodeData))) return;
        if (_requestedToStartNodes.Contains(nodeData)) return;
        if (RunningNodes.Contains(nodeData)) return;

        _requestedToStartNodes.Add(nodeData);

        if (RunningTransition == null) // если сейчас ничего не исполняется - сразу исполняем
        {
            StartTransition(nodeData.transitionToThis);
            return;
        }
    }

    private void RebuildContext()
    {
        if (_context == null) _context = new ScenarioContext();

        _context.ScenarioTime = _scenarioTimer;
        _context.NodeTime = _lastNodeTimer;

        _context.IsParsing = sledgeTransitor != null && sledgeTransitor.IsTransitioning;
        _context.CurrentPivotPosition = ArenaLocal.Pivot == null ? Vector3.zero : ArenaLocal.Pivot.position;
        _context.CurrentPlayerPosition = Player.PlayerTransform == null ? Vector3.zero : Player.PlayerTransform.position;

        // Debug.Log($"Обновление контекста: IsEncounterRunningValide = { _context.IsEncounterRunningValide} (Processing = {encounterDirector.Processing}, AnyEnemyAlive = {encounterDirector.AnyEnemyAlive})");
        _context.AnyEnemyAlive = encounterDirector.AnyEnemyAlive;

        _context.ActiveEncounterSnapshots = encounterDirector.GetActiveEncounterSnapshots();
        _context.SectorStateSnapshot = Map.CurrentSector.GetSectorStateSnapshot();
        _context.IsEncounterRunningValide = _context.ActiveEncounterSnapshots.Count > 0;//encounterDirector.Processing && encounterDirector.AnyEnemyAlive;
    }

    private void StartNewNode(NodeData nodeData)
    {
        for (int i = RunningNodes.Count - 1; i >= 0; i--)
        {
            if (RunningNodes[i].endIfOtherNodesStarted)
            {
                ReleaseNode(RunningNodes[i]);
            }
        }

        RunningNodes.Add(nodeData);

        ActiveNodeLogicRuntime nodeLogicRuntime = nodeData.logic.CreateRuntime();

        RunningNodeLogics[nodeData.id] = nodeLogicRuntime;

        _context.IsStartNode = _lastNodeIndex <= -1;

        _context.PreviousNodeIndex = _lastNodeIndex;

        _lastNodeIndex = ScenarioIndexOf(nodeData);
        _context.LastNodeIndex = _lastNodeIndex;

        _lastNodeTimer = 0f;
        _context.NodeTime = _lastNodeTimer;

        nodeLogicRuntime.Begin(_context);

        if (!string.IsNullOrEmpty(nodeData.radioMessagesLogicBlock.radioMessageIdOnStart))
        {
            RadioManager.RequestMessage(new RadioMessageRequest(
                nodeData.radioMessagesLogicBlock.radioMessageIdOnStart,
                nodeData.radioMessagesLogicBlock.radioChannelOnStart
            ));
        }

        DebugLog($"Запущена новая нода: {nodeData.id}");
    }

    private void TickNodeLogics(float dt)
    {
        var logics = new List<ActiveNodeLogicRuntime>(RunningNodeLogics.Values);
        for (int i = 0; i < logics.Count; i++)
        {
            logics[i].Tick(_context, dt);
        }
    }

    private void ReleaseNode(NodeData nodeData) // прекратить выполнение
    {
        RunningNodes.Remove(nodeData);

        RunningNodeLogics[nodeData.id].End(_context);

        RunningNodeLogics.Remove(nodeData.id);

        int nodeIndex = ScenarioIndexOf(nodeData);

        _context.CompletedNodeSet.Add(nodeIndex);

        if (!_context.CompletedNodeCounts.ContainsKey(nodeIndex))
            _context.CompletedNodeCounts[nodeIndex] = 0;
        _context.CompletedNodeCounts[nodeIndex]++;

        _context.ElapsedNodeIndexes.Add(nodeIndex);

        if (!string.IsNullOrEmpty(nodeData.radioMessagesLogicBlock.radioMessageIdOnEnd))
        {
            RadioManager.RequestMessage(new RadioMessageRequest(
                nodeData.radioMessagesLogicBlock.radioMessageIdOnEnd,
                nodeData.radioMessagesLogicBlock.radioChannelOnEnd
            ));
        }

        DebugLog($"Остановлена и освобождена нода: {nodeData.id}");
    }

    private void StartTransition(TransitionData transitionData)
    {
        if (RunningTransition != null)
        {
            Debug.LogError("Нельзя запускать новый переход, пока активен старый");
            return;
        }

        RunningTransition = transitionData.CreateRuntime();
        RunningTransition.Completed += OnTransitionCompleted; // будем ждать команды от рантайм-transition

        RunningTransition.Begin(_context);
        DebugLog($"Запущен переход типа {transitionData.GetType()}");
    }

    private void TickTransition(float dt)
    {
        if (RunningTransition == null) return;

        RunningTransition.Tick(_context, dt);
    }

    private void OnTransitionCompleted()
    {
        EndTransition();
    }

    private void EndTransition()
    {
        if (RunningTransition == null) return;

        RunningTransition.End(_context);
        RunningTransition.Completed -= OnTransitionCompleted;

        RunningTransition = null;

        StartNewNode(_requestedToStartNodes[0]);
        _requestedToStartNodes.RemoveAt(0);

        DebugLog($"Переход остановлен.");

        if (_requestedToStartNodes.Count > 0)
        {
            StartTransition(_requestedToStartNodes[0].transitionToThis);
        }
    }

    public void SetContextFlag(string name, bool flag)
    {
        if (flag)
        {
            _context.Flags.Add(name);
        } else if (_context.Flags.Contains(name))
        {
            _context.Flags.Remove(name);
        }
    }

    public void SetContextInt(string name, int v)
    {
        _context.Ints[name] = v;
    }

    public void SetContextFloat(string name, float v)
    {
        _context.Floats[name] = v;
    }
}
