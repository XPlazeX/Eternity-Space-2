using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public class SubclassSelectorDrawer : PropertyDrawer
{
    private static readonly Dictionary<Type, Type[]> CachedTypes = new();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
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

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        Type fieldType = GetManagedReferenceFieldType();
        if (fieldType == null)
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
            ShowTypeMenu(property, fieldType);
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

    private Type GetManagedReferenceFieldType()
    {
        if (fieldInfo == null)
            return null;

        return fieldInfo.FieldType;
    }

    private void ShowTypeMenu(SerializedProperty property, Type baseType)
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