using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ScenarioSystem;

[CustomEditor(typeof(ScenarioAsset))]
public class ScenarioAssetEditor : Editor
{
    private bool _showRawData;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (GUILayout.Button("Open Scenario Editor", GUILayout.Height(32f)))
        {
            ScenarioAssetEditorWindow.Open((ScenarioAsset)target);
        }

        EditorGUILayout.Space();

        SerializedProperty nodes = serializedObject.FindProperty("nodes");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startNodeIndex"));
        EditorGUILayout.LabelField("Nodes", nodes == null ? "0" : nodes.arraySize.ToString());

        EditorGUILayout.Space();
        _showRawData = EditorGUILayout.Foldout(_showRawData, "Raw Serialized Data", true);
        if (_showRawData)
        {
            EditorGUILayout.PropertyField(nodes, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

public class ScenarioAssetEditorWindow : EditorWindow
{
    private const float SidebarWidth = 280f;
    private const float MinDetailsWidth = 360f;
    private const float SpawnCardIconSize = 42f;

    private static readonly Dictionary<Type, Type[]> ManagedReferenceTypes = new Dictionary<Type, Type[]>();
    private ScenarioAsset _asset;
    private SerializedObject _serializedAsset;
    private SerializedProperty _nodesProperty;
    private SerializedProperty _startNodeIndexProperty;
    private Vector2 _nodeListScroll;
    private Vector2 _detailsScroll;
    private int _selectedNodeIndex;

    public static void Open(ScenarioAsset asset)
    {
        ScenarioAssetEditorWindow window = GetWindow<ScenarioAssetEditorWindow>("Scenario Editor");
        window.minSize = new Vector2(820f, 520f);
        window.SetAsset(asset);
        window.Show();
        window.Focus();
    }

    [MenuItem("Tools/Eternity Space/Scenario Editor")]
    private static void OpenFromMenu()
    {
        Open(Selection.activeObject as ScenarioAsset);
    }

    private void OnSelectionChange()
    {
        if (_asset == null && Selection.activeObject is ScenarioAsset selectedAsset)
        {
            SetAsset(selectedAsset);
            Repaint();
        }
    }

    private void OnGUI()
    {
        DrawToolbar();

        if (_asset == null)
        {
            DrawNoAssetView();
            return;
        }

        EnsureSerializedAsset();
        _serializedAsset.Update();

        ClampSelectedNodeIndex();

        Rect contentRect = new Rect(0f, EditorGUIUtility.singleLineHeight + 10f, position.width, position.height - EditorGUIUtility.singleLineHeight - 10f);
        Rect sidebarRect = new Rect(contentRect.x, contentRect.y, SidebarWidth, contentRect.height);
        Rect detailsRect = new Rect(sidebarRect.xMax + 1f, contentRect.y, Mathf.Max(MinDetailsWidth, contentRect.width - SidebarWidth - 1f), contentRect.height);

        DrawNodeList(sidebarRect);
        DrawNodeDetails(detailsRect);

        _serializedAsset.ApplyModifiedProperties();
    }

    private void SetAsset(ScenarioAsset asset)
    {
        _asset = asset;
        _serializedAsset = asset == null ? null : new SerializedObject(asset);
        _nodesProperty = _serializedAsset?.FindProperty("nodes");
        _startNodeIndexProperty = _serializedAsset?.FindProperty("startNodeIndex");
        _selectedNodeIndex = Mathf.Max(0, _selectedNodeIndex);
    }

    private void EnsureSerializedAsset()
    {
        if (_serializedAsset != null && _serializedAsset.targetObject == _asset)
            return;

        SetAsset(_asset);
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        EditorGUI.BeginChangeCheck();
        ScenarioAsset selectedAsset = (ScenarioAsset)EditorGUILayout.ObjectField(_asset, typeof(ScenarioAsset), false, GUILayout.MinWidth(240f));
        if (EditorGUI.EndChangeCheck())
        {
            SetAsset(selectedAsset);
        }

        GUILayout.FlexibleSpace();

        using (new EditorGUI.DisabledScope(_asset == null))
        {
            if (GUILayout.Button("Ping", EditorStyles.toolbarButton, GUILayout.Width(56f)))
            {
                EditorGUIUtility.PingObject(_asset);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawNoAssetView()
    {
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.HelpBox("Select a ScenarioAsset or assign it in the toolbar.", MessageType.Info);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
    }

    private void DrawNodeList(Rect rect)
    {
        GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);

        GUILayout.BeginArea(new Rect(rect.x + 8f, rect.y + 8f, rect.width - 16f, rect.height - 16f));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Nodes", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("+", GUILayout.Width(28f)))
        {
            AddNode();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(4f);
        EditorGUILayout.PropertyField(_startNodeIndexProperty);
        EditorGUILayout.Space(6f);

        _nodeListScroll = EditorGUILayout.BeginScrollView(_nodeListScroll);

        if (_nodesProperty.arraySize == 0)
        {
            EditorGUILayout.HelpBox("No nodes yet.", MessageType.None);
        }

        for (int i = 0; i < _nodesProperty.arraySize; i++)
        {
            DrawNodeListItem(i);
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void DrawNodeListItem(int index)
    {
        SerializedProperty node = _nodesProperty.GetArrayElementAtIndex(index);
        SerializedProperty id = node.FindPropertyRelative("id");
        SerializedProperty debugName = node.FindPropertyRelative("debugName");

        bool isSelected = index == _selectedNodeIndex;
        bool isStart = _startNodeIndexProperty.intValue == index;

        GUIStyle style = new GUIStyle(EditorStyles.miniButton)
        {
            alignment = TextAnchor.MiddleLeft,
            fixedHeight = 34f
        };

        string title = string.IsNullOrEmpty(debugName.stringValue) ? "Unnamed Node" : debugName.stringValue;
        string prefix = isStart ? "* " : string.Empty;
        string label = $"{prefix}{index:00}  {title}";

        Color previousColor = GUI.backgroundColor;
        if (isSelected)
            GUI.backgroundColor = new Color(0.45f, 0.62f, 0.85f);
        else if (isStart)
            GUI.backgroundColor = new Color(0.55f, 0.75f, 0.55f);

        if (GUILayout.Button(label, style))
        {
            _selectedNodeIndex = index;
            GUI.FocusControl(null);
        }

        GUI.backgroundColor = previousColor;

        if (isSelected)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("ID", string.IsNullOrEmpty(id.stringValue) ? "-" : id.stringValue, EditorStyles.miniLabel);

            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(index == 0))
            {
                if (GUILayout.Button("Up", EditorStyles.miniButtonLeft))
                    MoveNode(index, index - 1);
            }

            using (new EditorGUI.DisabledScope(index >= _nodesProperty.arraySize - 1))
            {
                if (GUILayout.Button("Down", EditorStyles.miniButtonMid))
                    MoveNode(index, index + 1);
            }

            if (GUILayout.Button("Start", EditorStyles.miniButtonMid))
                _startNodeIndexProperty.intValue = index;

            if (GUILayout.Button("Remove", EditorStyles.miniButtonRight))
                RemoveNode(index);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(4f);
    }

    private void DrawNodeDetails(Rect rect)
    {
        GUILayout.BeginArea(new Rect(rect.x + 10f, rect.y + 8f, rect.width - 20f, rect.height - 16f));

        if (_nodesProperty.arraySize == 0)
        {
            EditorGUILayout.HelpBox("Add a node to start editing.", MessageType.Info);
            GUILayout.EndArea();
            return;
        }

        SerializedProperty node = _nodesProperty.GetArrayElementAtIndex(_selectedNodeIndex);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Node {_selectedNodeIndex:00}", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();

        using (new EditorGUI.DisabledScope(_selectedNodeIndex == 0))
        {
            if (GUILayout.Button("Previous", GUILayout.Width(78f)))
                _selectedNodeIndex--;
        }

        using (new EditorGUI.DisabledScope(_selectedNodeIndex >= _nodesProperty.arraySize - 1))
        {
            if (GUILayout.Button("Next", GUILayout.Width(78f)))
                _selectedNodeIndex++;
        }

        EditorGUILayout.EndHorizontal();

        _detailsScroll = EditorGUILayout.BeginScrollView(_detailsScroll);

        DrawSection("Identity", () =>
        {
            EditorGUILayout.PropertyField(node.FindPropertyRelative("id"));
            EditorGUILayout.PropertyField(node.FindPropertyRelative("debugName"));
        });

        DrawSection("Logic", () =>
        {
            DrawLogicProperty(node.FindPropertyRelative("logic"));
        });

        DrawSection("Start Conditions", () =>
        {
            DrawConditionBlockProperty(node.FindPropertyRelative("startConditions"), "Start Conditions");
        });

        DrawSection("End Conditions", () =>
        {
            DrawConditionBlockProperty(node.FindPropertyRelative("endConditions"), "End Conditions");
        });

        DrawSection("Transition To This Node", () =>
        {
            EditorGUILayout.PropertyField(node.FindPropertyRelative("transitionToThis"), true);
        });

        DrawRadioMessagesLogicBlock(node.FindPropertyRelative("radioMessagesLogicBlock"));

        DrawSection("Branches And Multi-Nodes", () =>
        {
            DrawPropertyIfExists(node.FindPropertyRelative("disposable"));
            DrawPropertyIfExists(node.FindPropertyRelative("endIfOtherNodesStarted"));
        });

        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void DrawSection(string title, System.Action drawContent)
    {
        EditorGUILayout.Space(6f);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.Space(2f);
        drawContent?.Invoke();
        EditorGUILayout.EndVertical();
    }

    private void DrawRadioMessagesLogicBlock(SerializedProperty radioMessagesProperty)
    {
        DrawSection("Radio Messages Logic Block", () =>
        {
            if (radioMessagesProperty == null)
            {
                EditorGUILayout.HelpBox("radioMessagesLogicBlock property was not found.", MessageType.Warning);
                return;
            }

            radioMessagesProperty.isExpanded = EditorGUILayout.Foldout(radioMessagesProperty.isExpanded, "RadioMessageLogicBlock", true);
            if (!radioMessagesProperty.isExpanded)
                return;

            EditorGUI.indentLevel++;
            EditorGUILayout.Space(2f);

            DrawSubsection("On Start");
            EditorGUILayout.PropertyField(radioMessagesProperty.FindPropertyRelative("radioMessageIdOnStart"));
            EditorGUILayout.PropertyField(radioMessagesProperty.FindPropertyRelative("radioChannelOnStart"));

            DrawSubsection("On End");
            EditorGUILayout.PropertyField(radioMessagesProperty.FindPropertyRelative("radioMessageIdOnEnd"));
            EditorGUILayout.PropertyField(radioMessagesProperty.FindPropertyRelative("radioChannelOnEnd"));

            EditorGUI.indentLevel--;
        });
    }

    private void DrawConditionBlockProperty(SerializedProperty conditionBlockProperty, string label)
    {
        if (conditionBlockProperty == null)
        {
            EditorGUILayout.HelpBox($"{label} property was not found.", MessageType.Warning);
            return;
        }

        EditorGUILayout.BeginHorizontal();
        conditionBlockProperty.isExpanded = EditorGUILayout.Foldout(conditionBlockProperty.isExpanded, label, true);

        string state = conditionBlockProperty.managedReferenceValue == null ? "Null" : "ConditionBlock";
        EditorGUILayout.LabelField(state, EditorStyles.miniLabel);

        if (conditionBlockProperty.managedReferenceValue == null)
        {
            if (GUILayout.Button("Create", GUILayout.Width(72f)))
            {
                conditionBlockProperty.managedReferenceValue = new ConditionBlock();
                conditionBlockProperty.isExpanded = true;
            }
        }
        else if (GUILayout.Button("Clear", GUILayout.Width(72f)))
        {
            conditionBlockProperty.managedReferenceValue = null;
        }

        EditorGUILayout.EndHorizontal();

        if (!conditionBlockProperty.isExpanded || conditionBlockProperty.managedReferenceValue == null)
            return;

        EditorGUI.indentLevel++;
        SerializedProperty conditionsProperty = conditionBlockProperty.FindPropertyRelative("conditions");
        if (conditionsProperty == null)
        {
            EditorGUILayout.HelpBox("conditions array was not found.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.PropertyField(conditionsProperty, true);
        }

        EditorGUI.indentLevel--;
    }

    private void DrawLogicProperty(SerializedProperty logicProperty)
    {
        if (logicProperty.managedReferenceValue is EncounterNodeLogics)
        {
            DrawEncounterNodeLogic(logicProperty);
            return;
        }

        EditorGUILayout.PropertyField(logicProperty, true);
    }

    private void DrawEncounterNodeLogic(SerializedProperty logicProperty)
    {
        DrawManagedReferenceHeader(logicProperty, new GUIContent("logic"), typeof(NodeLogicData));

        if (!logicProperty.isExpanded || logicProperty.managedReferenceValue == null)
            return;

        EditorGUILayout.Space(4f);

        Rect backgroundRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUI.DrawRect(backgroundRect, EditorGUIUtility.isProSkin
            ? new Color(0.09f, 0.11f, 0.12f, 1f)
            : new Color(0.62f, 0.67f, 0.68f, 1f));

        EditorGUILayout.LabelField("Encounter Node Logic", EditorStyles.boldLabel);
        DrawPropertyIfExists(logicProperty.FindPropertyRelative("waitSledgeEndTransition"));
        DrawPropertyIfExists(logicProperty.FindPropertyRelative("waitTime"));

        SerializedProperty encounterProperty = logicProperty.FindPropertyRelative("encounterDef");
        DrawEncounterProperty(encounterProperty);

        EditorGUILayout.EndVertical();
    }

    private void DrawEncounterProperty(SerializedProperty encounterProperty)
    {
        if (encounterProperty == null)
        {
            EditorGUILayout.HelpBox("encounterDef property was not found.", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space(8f);

        Rect backgroundRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUI.DrawRect(backgroundRect, EditorGUIUtility.isProSkin
            ? new Color(0.06f, 0.075f, 0.085f, 1f)
            : new Color(0.50f, 0.56f, 0.58f, 1f));

        encounterProperty.isExpanded = EditorGUILayout.Foldout(encounterProperty.isExpanded, "Encounter", true);
        if (encounterProperty.isExpanded)
        {
            EditorGUI.indentLevel++;

            DrawEncounterGeneral(encounterProperty);
            DrawEncounterPlanned(encounterProperty);
            DrawEncounterBonus(encounterProperty);
            DrawEncounterFinalCards(encounterProperty);

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawEncounterGeneral(SerializedProperty encounterProperty)
    {
        DrawSubsection("General");
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("runtimeId"));
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("rareCardProbability"));
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("uncommonCardProbability"));
    }

    private void DrawEncounterPlanned(SerializedProperty encounterProperty)
    {
        DrawSubsection("Planned");
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("plannedWaitTime"));
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("plannedBudget"));
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("plannedCostWeights"), true);
        DrawSpawnCardsPool(encounterProperty.FindPropertyRelative("spawnCardsPool"), "Spawn Cards Pool");
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("plannedMaxWeight"));
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("plannedThresholdWeight"));
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("plannedPlayCooldown"));
    }

    private void DrawEncounterBonus(SerializedProperty encounterProperty)
    {
        DrawSubsection("Bonus");
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("bonusWaitTime"));
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("bonusCostWeights"), true);
        DrawSpawnCardsPool(encounterProperty.FindPropertyRelative("bonusCardsPool"), "Bonus Cards Pool");
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("bonusMaxWeight"));
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("bonusThresholdWeight"));
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("bonusPlayCooldown"));
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("bonusStopWeightThreshold"));
    }

    private void DrawEncounterFinalCards(SerializedProperty encounterProperty)
    {
        DrawSubsection("Final Cards");
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("playFinalCards"));
        DrawSpawnCardsPool(encounterProperty.FindPropertyRelative("sortedFinalCardsPool"), "Sorted Final Cards Pool");
        DrawPropertyIfExists(encounterProperty.FindPropertyRelative("finalPlayCooldown"));
    }

    private void DrawSubsection(string title)
    {
        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
    }

    private void DrawSpawnCardsPool(SerializedProperty poolProperty, string label)
    {
        if (poolProperty == null)
        {
            EditorGUILayout.HelpBox($"{label} property was not found.", MessageType.Warning);
            return;
        }

        SerializedProperty cardsProperty = poolProperty.FindPropertyRelative("spawnCards");
        if (cardsProperty == null)
        {
            EditorGUILayout.PropertyField(poolProperty, new GUIContent(label), true);
            return;
        }

        EditorGUILayout.Space(4f);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        cardsProperty.isExpanded = EditorGUILayout.Foldout(cardsProperty.isExpanded, $"{label} ({cardsProperty.arraySize})", true);

        if (cardsProperty.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Card", GUILayout.Width(92f)))
            {
                cardsProperty.InsertArrayElementAtIndex(cardsProperty.arraySize);
            }

            using (new EditorGUI.DisabledScope(cardsProperty.arraySize == 0))
            {
                if (GUILayout.Button("Clear", GUILayout.Width(62f)) &&
                    EditorUtility.DisplayDialog("Clear Spawn Cards", $"Clear {label}?", "Clear", "Cancel"))
                {
                    cardsProperty.ClearArray();
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(4f);

            for (int i = 0; i < cardsProperty.arraySize; i++)
            {
                DrawSpawnCardListItem(cardsProperty, i);
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawSpawnCardListItem(SerializedProperty cardsProperty, int index)
    {
        SerializedProperty cardProperty = cardsProperty.GetArrayElementAtIndex(index);
        SpawnCard card = cardProperty.objectReferenceValue as SpawnCard;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(SpawnCardIconSize + 8f));

        DrawSpawnCardIcon(card, GUILayoutUtility.GetRect(SpawnCardIconSize, SpawnCardIconSize, GUILayout.Width(SpawnCardIconSize), GUILayout.Height(SpawnCardIconSize)));

        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(cardProperty, new GUIContent($"Card {index:00}"));

        if (card != null)
        {
            EditorGUILayout.LabelField(GetSpawnCardSummary(card), EditorStyles.miniLabel);
        }

        EditorGUILayout.BeginHorizontal();
        using (new EditorGUI.DisabledScope(index == 0))
        {
            if (GUILayout.Button("Up", EditorStyles.miniButtonLeft, GUILayout.Width(48f)))
                cardsProperty.MoveArrayElement(index, index - 1);
        }

        using (new EditorGUI.DisabledScope(index >= cardsProperty.arraySize - 1))
        {
            if (GUILayout.Button("Down", EditorStyles.miniButtonMid, GUILayout.Width(58f)))
                cardsProperty.MoveArrayElement(index, index + 1);
        }

        if (GUILayout.Button("Remove", EditorStyles.miniButtonRight, GUILayout.Width(70f)))
        {
            DeleteObjectReferenceArrayElement(cardsProperty, index);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawSpawnCardIcon(SpawnCard card, Rect rect)
    {
        Rect paddedRect = new Rect(rect.x + 2f, rect.y + 2f, rect.width - 4f, rect.height - 4f);
        EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin
            ? new Color(0.16f, 0.18f, 0.19f, 1f)
            : new Color(0.38f, 0.43f, 0.45f, 1f));

        Sprite icon = card == null ? null : card.inspectorIcon;
        if (icon == null)
        {
            GUI.Label(paddedRect, "-", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        DrawSpritePreview(paddedRect, icon);
    }

    private void DrawSpritePreview(Rect rect, Sprite sprite)
    {
        if (sprite.texture == null)
            return;

        Rect textureRect = sprite.textureRect;
        Rect uv = new Rect(
            textureRect.x / sprite.texture.width,
            textureRect.y / sprite.texture.height,
            textureRect.width / sprite.texture.width,
            textureRect.height / sprite.texture.height);

        GUI.DrawTextureWithTexCoords(rect, sprite.texture, uv, true);
    }

    private string GetSpawnCardSummary(SpawnCard card)
    {
        return $"Cost {card.cost} | {card.rarity} | Requests {card.spawnRequests.Count}";
    }

    private void DeleteObjectReferenceArrayElement(SerializedProperty arrayProperty, int index)
    {
        int previousSize = arrayProperty.arraySize;
        arrayProperty.DeleteArrayElementAtIndex(index);

        if (arrayProperty.arraySize == previousSize && index < arrayProperty.arraySize)
            arrayProperty.DeleteArrayElementAtIndex(index);
    }

    private void DrawPropertyIfExists(SerializedProperty property, bool includeChildren = false)
    {
        if (property != null)
            EditorGUILayout.PropertyField(property, includeChildren);
    }

    private void DrawManagedReferenceHeader(SerializedProperty property, GUIContent label, Type baseType)
    {
        EditorGUILayout.BeginHorizontal();
        property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label, true);

        string currentTypeName = property.managedReferenceValue == null
            ? "Null"
            : property.managedReferenceValue.GetType().Name;

        EditorGUILayout.LabelField(currentTypeName, EditorStyles.miniLabel);

        if (GUILayout.Button("Set Type", GUILayout.Width(100f)))
        {
            ShowManagedReferenceTypeMenu(property, baseType);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ShowManagedReferenceTypeMenu(SerializedProperty property, Type baseType)
    {
        GenericMenu menu = new GenericMenu();
        UnityEngine.Object targetObject = property.serializedObject.targetObject;
        string propertyPath = property.propertyPath;

        menu.AddItem(new GUIContent("Null"), property.managedReferenceValue == null, () =>
        {
            SerializedObject so = new SerializedObject(targetObject);
            so.Update();
            SerializedProperty p = so.FindProperty(propertyPath);
            p.managedReferenceValue = null;
            so.ApplyModifiedProperties();
        });

        foreach (Type type in GetManagedReferenceTypes(baseType))
        {
            Type concreteType = type;
            bool isCurrent = property.managedReferenceValue != null &&
                             property.managedReferenceValue.GetType() == concreteType;

            menu.AddItem(new GUIContent(concreteType.Name), isCurrent, () =>
            {
                SerializedObject so = new SerializedObject(targetObject);
                so.Update();
                SerializedProperty p = so.FindProperty(propertyPath);
                p.managedReferenceValue = Activator.CreateInstance(concreteType);
                p.isExpanded = true;
                so.ApplyModifiedProperties();
            });
        }

        menu.ShowAsContext();
    }

    private static Type[] GetManagedReferenceTypes(Type baseType)
    {
        if (ManagedReferenceTypes.TryGetValue(baseType, out Type[] cachedTypes))
            return cachedTypes;

        Type[] types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly =>
            {
                try { return assembly.GetTypes(); }
                catch { return Array.Empty<Type>(); }
            })
            .Where(type =>
                baseType.IsAssignableFrom(type) &&
                !type.IsAbstract &&
                !type.IsInterface &&
                !typeof(UnityEngine.Object).IsAssignableFrom(type))
            .OrderBy(type => type.Name)
            .ToArray();

        ManagedReferenceTypes[baseType] = types;
        return types;
    }

    private void AddNode()
    {
        int newIndex = _nodesProperty.arraySize;
        _nodesProperty.InsertArrayElementAtIndex(newIndex);

        SerializedProperty node = _nodesProperty.GetArrayElementAtIndex(newIndex);
        node.FindPropertyRelative("id").stringValue = System.Guid.NewGuid().ToString("N");
        node.FindPropertyRelative("debugName").stringValue = $"Node {newIndex:00}";
        node.FindPropertyRelative("logic").managedReferenceValue = null;
        SetManagedReferenceIfExists(node.FindPropertyRelative("startConditions"), new ConditionBlock());
        SetManagedReferenceIfExists(node.FindPropertyRelative("endConditions"), new ConditionBlock());
        node.FindPropertyRelative("transitionToThis").managedReferenceValue = null;
        SetBoolIfExists(node.FindPropertyRelative("disposable"), false);
        SetBoolIfExists(node.FindPropertyRelative("endIfOtherNodesStarted"), false);

        _selectedNodeIndex = newIndex;
        GUI.FocusControl(null);
    }

    private void SetManagedReferenceIfExists(SerializedProperty property, object value)
    {
        if (property != null)
            property.managedReferenceValue = value;
    }

    private void SetBoolIfExists(SerializedProperty property, bool value)
    {
        if (property != null)
            property.boolValue = value;
    }

    private void RemoveNode(int index)
    {
        if (!EditorUtility.DisplayDialog("Remove Scenario Node", $"Remove node {index:00}?", "Remove", "Cancel"))
            return;

        _nodesProperty.DeleteArrayElementAtIndex(index);
        _selectedNodeIndex = Mathf.Clamp(_selectedNodeIndex, 0, Mathf.Max(0, _nodesProperty.arraySize - 1));

        if (_startNodeIndexProperty.intValue >= _nodesProperty.arraySize)
            _startNodeIndexProperty.intValue = Mathf.Max(0, _nodesProperty.arraySize - 1);
    }

    private void MoveNode(int from, int to)
    {
        _nodesProperty.MoveArrayElement(from, to);
        _selectedNodeIndex = to;

        if (_startNodeIndexProperty.intValue == from)
        {
            _startNodeIndexProperty.intValue = to;
        }
        else if (_startNodeIndexProperty.intValue == to)
        {
            _startNodeIndexProperty.intValue = from;
        }
    }

    private void ClampSelectedNodeIndex()
    {
        if (_nodesProperty == null)
            return;

        _selectedNodeIndex = Mathf.Clamp(_selectedNodeIndex, 0, Mathf.Max(0, _nodesProperty.arraySize - 1));

        if (_startNodeIndexProperty != null && _nodesProperty.arraySize > 0)
            _startNodeIndexProperty.intValue = Mathf.Clamp(_startNodeIndexProperty.intValue, 0, _nodesProperty.arraySize - 1);
    }
}
