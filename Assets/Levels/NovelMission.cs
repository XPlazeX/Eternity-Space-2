using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovelMission : Mission
{
    private const string novel_filename = "NovelChoices";

    [Header("Novel Mission")]
    [SerializeField] private float _waitTime;
    [SerializeField] private NovelNode[] _nodes;
    [SerializeField] private NovelGate _gateSample;
    [SerializeField] private float _gateRadius;

    private DialogueOpener _dialogOpener;
    private int _activeNode = 0;
    private List<NovelGate> ActiveGates = new List<NovelGate>();
    private bool _choiceMaded = false;
    private Spawner _spawner;

    public override void StartPlay()
    {
        base.StartPlay();

        _dialogOpener = GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>();
        _spawner = SceneStatics.SceneCore.GetComponent<Spawner>();

        _spawner.HideLabel();
        _spawner.PrintProgressUI($"???");

        StartCoroutine(NovelCycle());
    }

    private IEnumerator NovelCycle()
    {
        yield return new WaitForSeconds(_waitTime);

        for (int i = 0; i < _nodes.Length; i++)
        {
            _activeNode = i;
            _dialogOpener.TriggerDialogue(_nodes[i].dialogueName);

            yield return new WaitForSeconds(2f);

            GetNodeChoice(_activeNode);

            _choiceMaded = false;

            while (!_choiceMaded)
            {
                yield return null;
            }

            yield return new WaitForSeconds(_nodes[i].afterTime);
        }

        TriggerVictory();
    }

    public void GetNodeChoice(int nodeID)
    {
        NovelNode activeNode = _nodes[nodeID];

        for (int i = 0; i < activeNode.novelChocesRowCols.Length; i++)
        {
            NovelGate gate = Instantiate(_gateSample, CameraController.GetRandomFieldPosition(_gateRadius + 1f, Player.PlayerTransform.position, _gateRadius + 1f), Quaternion.identity);

            gate.SetCallbackID(ActiveGates.Count, this);
            ActiveGates.Add(gate);
            
            gate.SetText(SceneLocalizator.GetLocalizedString(novel_filename, activeNode.novelChocesRowCols[i].x, activeNode.novelChocesRowCols[i].y));
        }
    }

    public void GateChoiced(int id)
    {
        for (int i = 0; i < ActiveGates.Count; i++)
        {
            Destroy(ActiveGates[i].gameObject);
        }

        ActiveGates.Clear();

        _dialogOpener.TriggerDialogue(_nodes[_activeNode].triggeringDialogues[id]);
        _choiceMaded = true;
    }

    // public void OnMissionVictored()
    // {
    //     if (_autoExitToLobby)
    //         VictoryHandler.VictorySession();
    // }

    [System.Serializable]
    private struct NovelNode
    {
        public string dialogueName;
        public Vector2Int[] novelChocesRowCols;
        public string[] triggeringDialogues;
        public float afterTime;
    }
}
