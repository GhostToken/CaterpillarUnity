using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnumAsFlags : PropertyAttribute
{
    public EnumAsFlags() 
    { 
    }
}

public class AnyFlagRequirement : PropertyAttribute
{
    public readonly string FieldToCheck;
    public readonly int Mask;

    public AnyFlagRequirement(string fieldToCheck, int mask)
    {
        FieldToCheck = fieldToCheck;
        Mask = mask;
    }
}

public class AllFlagRequirement : PropertyAttribute
{
    public readonly string FieldToCheck;
    public readonly int Mask;

    public AllFlagRequirement(string fieldToCheck, int mask)
    {
        FieldToCheck = fieldToCheck;
        Mask = mask;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(EnumAsFlags))]
public class EnumAsFlagsPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        _property.intValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
    }
}

[CustomPropertyDrawer(typeof(AnyFlagRequirement))]
public class AnyFlagRequirementPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        if (CheckCondition(attribute as AnyFlagRequirement, _property))
        {
            EditorGUI.PropertyField(_position, _property, _label, true);
        }
    }

    private bool CheckCondition(AnyFlagRequirement _flagAttribute, SerializedProperty _property)
    {
        SerializedProperty property = _property.serializedObject.GetIterator();
        while (property.Next(true))
        {
            if (property.name == _flagAttribute.FieldToCheck)
            {
                return (property.intValue & _flagAttribute.Mask) > 0;
            }
        }
        Debug.LogWarning("Property not found : " + _flagAttribute.FieldToCheck);
        return true;
    }

    public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
    {
        if(CheckCondition(attribute as AnyFlagRequirement, _property) == true)
        {
            return EditorGUI.GetPropertyHeight(_property, _label);
        }
        return 0.0f;
    }
}

[CustomPropertyDrawer(typeof(AllFlagRequirement))]
public class AllFlagRequirementPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        if (CheckCondition(attribute as AnyFlagRequirement, _property))
        {
            EditorGUI.PropertyField(_position, _property, _label, true);
        }
    }

    private bool CheckCondition(AnyFlagRequirement _flagAttribute, SerializedProperty _property)
    {
        SerializedProperty property = _property.serializedObject.GetIterator();
        while (property.Next(true))
        {
            if (property.name == _flagAttribute.FieldToCheck)
            {
                return (property.intValue & _flagAttribute.Mask) == _flagAttribute.Mask;
            }
        }
        Debug.LogWarning("Property not found : " + _flagAttribute.FieldToCheck);
        return true;
    }

    public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
    {
        if (CheckCondition(attribute as AnyFlagRequirement, _property) == true)
        {
            return EditorGUI.GetPropertyHeight(_property, _label);
        }
        return 0.0f;
    }
}

#endif