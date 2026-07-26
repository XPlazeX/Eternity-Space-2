using System.Collections.Generic;
using UnityEngine;

/// ПАЙПЛАЙН
/// Scenario Runner - исполнитель сценариев. Держит и формирует контекст. 
/// Получает ScenarioAsset и на его основе читает и создает runtime ноды и переходы.
/// 1. Начинает с начальной ноды, формирует и передаёт ей контекст.
/// 2. Каждый тик смотрит end Condition текущей ноды (или параллельно начало следующих).
/// 3. При выполнении условия - нода считается пройденной.
/// 4. Каждый тик смотрит start Condition следующей ноды (или параллельно). 
/// - Для последовательности, start Condition должен быть InstantCondition
/// - Если особое условие, то никакие ноды не выполняются, ожидается выполнение условий (каждый тик проверка), например - нужно взаимодействие
/// 5. Создается рантайм Transition, каждый тик исполняется, вплоть до коллбека Completed
/// 6. Запускается следующая нода

namespace ScenarioSystem 
{
    [System.Serializable]
    public class NodeData
    {
        public string id;
        public string debugName = "debug_node";

        [SerializeReference][SubclassSelector] public NodeLogicData logic;
        [Space()]
        [Tooltip("Условие для начала ноды, т.е. Что должно произойти в предыдущих нодах, чтобы началась эта.")]
        [SerializeReference] public ConditionBlock startConditions;
        [Space()]
        [Tooltip("Условие конца ноды, т.е. Что должно произойти в текущих нодах, чтобы остановилась эта.")]
        [SerializeReference] public ConditionBlock endConditions;
        [Space()]
        [SerializeReference][SubclassSelector] public TransitionData transitionToThis;
        [Space()]
        public RadioMessagesLogicBlock radioMessagesLogicBlock;

        [Space()]
        // [Tooltip("Если пусто - смотреть только следующую ноду, иначе смотреть условия указанных нод (для развилок)")]
        // public int[] nextNodeIndexes; 
        [Tooltip("Если включено - нода может запуститься только 1 раз.")]
        public bool disposable;
        [Tooltip("Автоматически завершать ноду, если началась другая (для развилок)")]
        public bool endIfOtherNodesStarted;
    }

    [System.Serializable]
    public abstract class NodeLogicData
    {
        /// EmptyNode - ничего дополнительно происходить не будет (возможно, все уже имеется на поле)
        /// ObjectSpawnNode - спаунит что-то в обход EnemySpawner
        /// EnemySpawnNode - спаунит благодаря EnemySpawner, в т.ч. боссов (можно сделать наследование: wave, boss, timed и т.п.)
        /// DialogueNode - сценарный диалог, после него можно instant transition и т.п.
        /// ScriptedNode - для сложного поведения, катсцены там - если пригодится
        // [Tooltip("Закончить уровень по завершению этой ноды?")]
        // public bool endRunOnCompleted; 
        // [Tooltip("При проигрыше, начинать с этой ноды?")]
        // public bool isCheckpoint; 
        // [Tooltip("Скорость перемещения арены (и салазок). ВНИМАНИЕ: ничем не ограничено!")] // скролл-шутер, ага?
        // public Vector2 scrollingPivotSpeed = new Vector2(0f, 0f);
        // [Tooltip("Скорость перемещения арены (и салазок) за игроком. ВНИМАНИЕ: ничем не ограничено!")] // для ощущения и эксплоринга на уровне(некоторых)
        // public float pivotToPlayerFollowSpeed = 0f;

        public abstract ActiveNodeLogicRuntime CreateRuntime();
    }

    [System.Serializable]
    public abstract class TransitionData
    {
        /// InstantTransition - запускает мгновенно
        /// FusedTransition - просто ждет и запускает
        /// SledgeFloatingTransition - салазки плавно летят к позиции
        /// SledgeParsingTransition - нужно состыковаться с салазками, потом к позиции

        public abstract ActiveTransitionRuntime CreateRuntime();
    }

    [System.Serializable]
    public class ConditionBlock
    {
        [SerializeReference][SubclassSelectorListed] private Condition[] conditions = new Condition[0];

        public bool Evaluate(ScenarioContext ctx)
        {
            if (conditions == null)
                return true;

            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i] != null && !conditions[i].Evaluate(ctx)) return false;
            }

            return true;
        }
    }

    [System.Serializable]
    public abstract class Condition
    {
        /// InstantCondition - всегда возвращает true (сразу запускает)
        /// IndexesCondition - смотрит какие ноды исполнялись, в т.ч. может быть использован в роли "Если эта выполнена, то эту запускаем"
        /// TimeCondition - смотрит на время
        /// EnemiesCondition - все что связано с врагами
        /// AwaitTriggerCondition - ждет(смотрит) Scenario Trigger
        /// AwaitValuesCondition - ждет значения в Flags, Ints и Floats
        /// MultiCondition - ...
        ///  
        public abstract bool Evaluate(ScenarioContext ctx);
    }

    public static class ScenarioTrigger
    {
        public static event System.Action<string> Triggered;
        public static void Trigger(string t)
        {
            Debug.Log($"<color=orange>[SCENARIO TRIGGER]: {t}</color>");
            Triggered?.Invoke(t);
        }
    }

    // === RUNTIME ===

    public class ScenarioContext
    {
        public float ScenarioTime; // общее время
        public float NodeTime; // с момента запуска ноды
        public int LastNodeIndex; // индекс ноды в сценарии
        public int PreviousNodeIndex = -1; // индекс предыдущей ноды в сценарии, для старта -1
        // public int CheckpointNodeIndex = -1; // индекс последней чекпоинт-ноды, по умолчанию -1

        public HashSet<int> CompletedNodeSet = new(); // уникально пройденные ноды // быстрый доступ
        public Dictionary<int, int> CompletedNodeCounts = new(); // сколько раз какие ноды были пройдены
        public List<int> ElapsedNodeIndexes = new(); // порядок прохождения нод, в т.ч. повторения

        public bool IsStartNode; // с этой ли ноды начался уровень
        // public bool IsEndingNode; // можно ли в этой ноде закончить уровень
        public bool IsEncounterRunningValide; // запущен ли сейчас энкаунтер (для условий, которые зависят от этого)
        public bool AnyEnemyAlive; // если есть враги
        public bool IsParsing; // перемещается ли игрок на салазках

        public int EnemiesAlive; // общее количество
        public int BossesAlive; // особые враги
        public float EnemiesWeight; // общее количество
        public int DefeatedEnemiesCount; // вообще все враги (включая боссов)
        public int DefeatedBosses; // особые враги

        public Vector3 NodeStartPivotPosition; // позиция центра арены в момент запуска ноды
        public Vector3 CurrentPivotPosition; // позиция центра арены сейчас
        public Vector3 CurrentPlayerPosition; // позиция центра арены сейчас

        public bool NoDamageTaken = true; // собсна

        public Dictionary<string, EncounterSnapshot> ActiveEncounterSnapshots = new(); // для проверок Condition, доступ по runtime-ключу Encounter
        public SectorStateSnapshot SectorStateSnapshot;

        // === СИСТЕМЫ
        public EncounterDirector EncounterDirector;
        public SectorRoot Sector;
        public EnemyHQ EnemyHq;
        // public SledgeController SledController;
        // public DialogueController DialogueController;
        // PlayerShipData - статик, доступ к здоровью, урону игроку
        // Player - статик, доступ к трансформу

        public HashSet<string> Flags = new();
        public Dictionary<string, int> Ints = new();
        public Dictionary<string, float> Floats = new();
    }

    public abstract class ActiveNodeLogicRuntime
    {
        public NodeLogicData data;

        public abstract void Begin(ScenarioContext context);
        public abstract void Tick(ScenarioContext context, float dt);
        public abstract void End(ScenarioContext context);
    }

    public abstract class ActiveTransitionRuntime
    {
        public abstract event System.Action Completed; // коллбэк завершенного перехода, можно прекратить выполнение
        public TransitionData data;

        public abstract void Begin(ScenarioContext context);
        public abstract void Tick(ScenarioContext context, float dt);
        public abstract void End(ScenarioContext context);

        public abstract float GetProgress01(ScenarioContext ctx); /// для того чтобы понимать состояние перехода
    }

    [System.Serializable]
    public class RadioMessagesLogicBlock
    {
        [Tooltip("Оставить пустым, если сообщение не требуется.")]
        public string radioMessageIdOnStart = "";
        public RadioChannel radioChannelOnStart = RadioChannel.Story;
        [Tooltip("Оставить пустым, если сообщение не требуется.")]
        public string radioMessageIdOnEnd = "";
        public RadioChannel radioChannelOnEnd = RadioChannel.Story;
    }
}
