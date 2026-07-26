using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public class SubclassSelectorDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return SubclassSelectorGUI.GetHeight(property);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Type fieldType = GetManagedReferenceFieldType();
        SubclassSelectorGUI.Draw(position, property, label, fieldType);
    }

    private Type GetManagedReferenceFieldType()
    {
        return fieldInfo?.FieldType;
    }
}

[CustomPropertyDrawer(typeof(SubclassSelectorListedAttribute))]
public class SubclassSelectorListedDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (IsManagedReferenceElementProperty(property))
            return SubclassSelectorGUI.GetHeight(property);

        if (!IsSupportedListProperty(property))
            return EditorGUIUtility.singleLineHeight;

        float height = EditorGUIUtility.singleLineHeight;
        if (!property.isExpanded)
            return height;

        SerializedProperty sizeProperty = property.FindPropertyRelative("Array.size");
        height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(sizeProperty);

        for (int i = 0; i < property.arraySize; i++)
        {
            SerializedProperty element = property.GetArrayElementAtIndex(i);
            height += EditorGUIUtility.standardVerticalSpacing + SubclassSelectorGUI.GetHeight(element);
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (IsManagedReferenceElementProperty(property))
        {
            SubclassSelectorGUI.Draw(position, property, label, GetElementType());
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        if (!IsSupportedListProperty(property))
        {
            EditorGUI.LabelField(headerRect, label.text, "Use on SerializeReference lists or arrays");
            EditorGUI.EndProperty();
            return;
        }

        Type elementType = GetElementType();
        if (elementType == null)
        {
            EditorGUI.LabelField(headerRect, label.text, "Element type not found");
            EditorGUI.EndProperty();
            return;
        }

        property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, label, true);
        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        EditorGUI.indentLevel++;

        float y = headerRect.yMax + EditorGUIUtility.standardVerticalSpacing;
        SerializedProperty sizeProperty = property.FindPropertyRelative("Array.size");
        float sizeHeight = EditorGUI.GetPropertyHeight(sizeProperty);
        Rect sizeRect = new Rect(position.x, y, position.width, sizeHeight);
        EditorGUI.PropertyField(sizeRect, sizeProperty);
        y += sizeHeight + EditorGUIUtility.standardVerticalSpacing;

        for (int i = 0; i < property.arraySize; i++)
        {
            SerializedProperty element = property.GetArrayElementAtIndex(i);
            float elementHeight = SubclassSelectorGUI.GetHeight(element);
            Rect elementRect = new Rect(position.x, y, position.width, elementHeight);
            SubclassSelectorGUI.Draw(elementRect, element, new GUIContent($"Element {i}"), elementType);
            y += elementHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

    private bool IsSupportedListProperty(SerializedProperty property)
    {
        return fieldInfo != null &&
               property.isArray &&
               property.propertyType == SerializedPropertyType.Generic &&
               GetElementType() != null;
    }

    private bool IsManagedReferenceElementProperty(SerializedProperty property)
    {
        return fieldInfo != null &&
               property.propertyType == SerializedPropertyType.ManagedReference &&
               GetElementType() != null;
    }

    private Type GetElementType()
    {
        if (fieldInfo == null)
            return null;

        Type fieldType = fieldInfo.FieldType;
        if (fieldType.IsArray)
            return fieldType.GetElementType();

        if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            return fieldType.GetGenericArguments()[0];

        return null;
    }
}

internal static class SubclassSelectorGUI
{
    private static readonly Dictionary<Type, Type[]> CachedTypes = new();

    public static float GetHeight(SerializedProperty property)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (property.isExpanded && property.managedReferenceValue != null)
        {
            SerializedProperty copy = property.Copy();
            SerializedProperty end = copy.GetEndProperty();

            copy.NextVisible(true);
            while (!SerializedProperty.EqualContents(copy, end))
            {
                height += EditorGUI.GetPropertyHeight(copy, true) + EditorGUIUtility.standardVerticalSpacing;
                if (!copy.NextVisible(false))
                    break;
            }
        }

        return height;
    }

    public static void Draw(Rect position, SerializedProperty property, GUIContent label, Type baseType)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        if (baseType == null)
        {
            EditorGUI.LabelField(headerRect, label.text, "Field type not found");
            EditorGUI.EndProperty();
            return;
        }

        string currentTypeName = property.managedReferenceValue == null
            ? "Null"
            : property.managedReferenceValue.GetType().Name;

        Rect foldoutRect = new Rect(headerRect.x, headerRect.y, 16f, headerRect.height);
        Rect labelRect = new Rect(headerRect.x + 16f, headerRect.y, headerRect.width - 120f, headerRect.height);
        Rect buttonRect = new Rect(headerRect.xMax - 100f, headerRect.y, 100f, headerRect.height);

        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none);
        EditorGUI.LabelField(labelRect, label.text, currentTypeName);

        if (GUI.Button(buttonRect, "Set Type"))
        {
            ShowTypeMenu(property, baseType);
        }

        if (property.isExpanded && property.managedReferenceValue != null)
        {
            EditorGUI.indentLevel++;
            SerializedProperty copy = property.Copy();
            SerializedProperty end = copy.GetEndProperty();

            float y = headerRect.yMax + EditorGUIUtility.standardVerticalSpacing;

            copy.NextVisible(true);
            while (!SerializedProperty.EqualContents(copy, end))
            {
                float h = EditorGUI.GetPropertyHeight(copy, true);
                Rect r = new Rect(position.x, y, position.width, h);
                EditorGUI.PropertyField(r, copy, true);
                y += h + EditorGUIUtility.standardVerticalSpacing;

                if (!copy.NextVisible(false))
                    break;
            }
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    private static void ShowTypeMenu(SerializedProperty property, Type baseType)
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

        foreach (Type type in GetConcreteTypes(baseType))
        {
            Type concreteType = type;

            bool isCurrent = property.managedReferenceValue != null &&
                            property.managedReferenceValue.GetType() == concreteType;

            menu.AddItem(new GUIContent(concreteType.Name), isCurrent, () =>
            {
                SerializedObject so = new SerializedObject(targetObject);
                so.Update();

                SerializedProperty p = so.FindProperty(propertyPath);

                object instance = Activator.CreateInstance(concreteType);
                p.managedReferenceValue = instance;
                p.isExpanded = true;

                so.ApplyModifiedProperties();
            });
        }

        menu.ShowAsContext();
    }

    private static Type[] GetConcreteTypes(Type baseType)
    {
        if (CachedTypes.TryGetValue(baseType, out Type[] cached))
            return cached;

        Type[] result = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Array.Empty<Type>(); }
            })
            .Where(t =>
                baseType.IsAssignableFrom(t) &&
                !t.IsAbstract &&
                !t.IsInterface &&
                !typeof(UnityEngine.Object).IsAssignableFrom(t))
            .OrderBy(t => t.Name)
            .ToArray();

        CachedTypes[baseType] = result;
        return result;
    }
}
