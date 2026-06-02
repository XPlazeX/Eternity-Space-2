using UnityEngine;
using ScenarioSystem;

public class EncounterNodeLogics : NodeLogicData
{
    [SerializeField] private bool waitSledgeEndTransition = true; // ждать ли окончания перехода на салазках, прежде чем запускать энкаунтер
    [SerializeField] private float waitTime = 5f;
    [SerializeField] private Encounter encounterDef; 

    public Encounter EncounterDef => encounterDef;
    public bool WaitSledgeEndTransition => waitSledgeEndTransition;
    public float WaitTime => waitTime;

    public override ActiveNodeLogicRuntime CreateRuntime()
    {
        EncounterNodeLogicRuntime runtime = new EncounterNodeLogicRuntime();
        runtime.data = this;

        return runtime;
    }
}

public class EncounterNodeLogicRuntime : ActiveNodeLogicRuntime
{
    private bool _encounterStarted = false;

    public override void Begin(ScenarioContext context)
    {
        if (_encounterStarted) return;

        if (((EncounterNodeLogics)data).WaitSledgeEndTransition && context.IsParsing)
        {
            // ждем окончания перехода на салазках, прежде чем запускать энкаунтер
            return;
        }

        if (!_encounterStarted && ((EncounterNodeLogics)data).WaitTime <= 0f)
        {
            TryStartEncounter(context);
        }
    }

    public override void End(ScenarioContext context)
    {
        return;
    }

    public override void Tick(ScenarioContext context, float dt)
    {
        if (_encounterStarted) return;

        if (((EncounterNodeLogics)data).WaitSledgeEndTransition && context.IsParsing)
        {
            // ждем окончания перехода на салазках, прежде чем запускать энкаунтер
            return;
        }

        if (!_encounterStarted && ((EncounterNodeLogics)data).WaitTime >= context.NodeTime)
        {
            TryStartEncounter(context);
        }
    }

    private void TryStartEncounter(ScenarioContext context)
    {
        if (_encounterStarted) return;

        if (((EncounterNodeLogics)data).EncounterDef != null)
        {
            _encounterStarted = true;
            context.EncounterDirector.StartEncounter(((EncounterNodeLogics)data).EncounterDef);
        }
    }
}

