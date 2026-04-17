using System.Collections.Generic;
using ScenarioSystem;
using UnityEngine;

public class ScenarioRunner : MonoBehaviour
{
    public event System.Action ScenarioAssetChanged;

    [SerializeField] private ScenarioAsset scenarioAsset;
    [SerializeField] private EncounterDirector encounterDirector;

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
    private int _checkpointNodeIndex = -1;
    private float _lastNodeTimer;
    private float _scenarioTimer;
    private Dictionary<string, int> _nodeIndexes;

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
        _context = new ScenarioContext();
        RebuildContext();
    }

    public void StartRunning()
    {
        if (scenarioAsset == null)
        {
            Debug.LogError("Невозможно запустить сценарий: сценарий отстутствует (null)");
            return;
        }
        Running = true;
    }

    public void SetPaused(bool pause)
    {
        Paused = pause;
    }

    void Update()
    {
        if (!Running || Paused) return;

        Tick(Time.deltaTime);
        _lastNodeTimer += Time.deltaTime;
        _scenarioTimer += Time.deltaTime;
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

        // проверка условий начала
        List<int> checkingNodes = new List<int>() {_lastNodeIndex + 1}; // смотрим следующую ноду

        if (_lastNodeIndex + 1 >= RunningScenario.nodes.Count) // если это последняя нода - следующую не смотрим
        {
            checkingNodes.Clear();
        }

        for (int i = 0; i < RunningNodes.Count; i++) // если ноды хотят, чтобы мы проверяли другие ноды - собираем их индексы
        {
            int[] toCheck = RunningNodes[i].nextNodeIndexes;

            for (int j = 0; j < toCheck.Length; j++)
            {
                if (!checkingNodes.Contains(toCheck[j]))
                {
                    checkingNodes.Add(toCheck[j]);
                }
            }
        }

        for (int i = 0; i < checkingNodes.Count; i++) // запускаем ноды, если условия старта удовлетворительны
        {
            NodeData nodeData = RunningScenario.nodes[checkingNodes[i]];

            if (nodeData.disposable && _context.CompletedNodeSet.Contains(checkingNodes[i])) continue;

            if (nodeData.startCondition.Evaluate(_context))
            {
                RequestTransitionToNode(nodeData);
            }
        }

        // проверка условий конца
        for (int i = RunningNodes.Count - 1; i >= 0; i--)
        {
            NodeData nodeData = RunningNodes[i];
            if (nodeData.endCondition.Evaluate(_context))
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

        _context.AnyEnemyAlive = encounterDirector.AnyEnemyAlive;
        //_context.IsParsing = когда доделаем Transitions
        // _context.CurrentPivotPosition =
        // _context.NoDamageTaken = 

        _context.ActiveEncounterSnapshots = encounterDirector.GetActiveEncounterSnapshots();
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

        if (nodeData.logic.isCheckpoint)
        {
            _context.CheckpointNodeIndex = _lastNodeIndex;
        }

        _context.IsEndingNode = nodeData.logic.endRunOnCompleted;
        
        // _context.NodeStartPivotPosition = Vector3.zero;
        _lastNodeTimer = 0f;
        _context.NodeTime = _lastNodeTimer;

        nodeLogicRuntime.Begin(_context);
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
