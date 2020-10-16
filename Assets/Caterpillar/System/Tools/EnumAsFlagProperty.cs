using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#region EnumAsFlags

public class EnumAsFlags : PropertyAttribute
{
    public EnumAsFlags() 
    { 
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
#endif

#endregion

#region FlagRequirement

public class FlagRequirement : PropertyAttribute
{
    public readonly string FieldToCheck;
    public readonly int Mask;

    public FlagRequirement(string fieldToCheck, int mask)
    {
        FieldToCheck = fieldToCheck;
        Mask = mask;
    }
}

public class AnyFlagRequirement : FlagRequirement
{
    public AnyFlagRequirement(string fieldToCheck, int mask)
        : base(fieldToCheck, mask)
    {
    }
}

public class AllFlagRequirement : FlagRequirement
{
    public AllFlagRequirement(string fieldToCheck, int mask)
        : base(fieldToCheck, mask)
    {
    }
}

#if UNITY_EDITOR

public class FlagRequirementPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        string parentPropertyName = _property.propertyPath.Substring(0, _property.propertyPath.LastIndexOf('.'));
        if (CheckCondition(attribute as AnyFlagRequirement, _property, parentPropertyName))
        {
            EditorGUI.PropertyField(_position, _property, _label, true);
        }
    }

    private bool CheckCondition(AnyFlagRequirement _flagAttribute, SerializedProperty _property, string ParentPropertyName)
    {
        SerializedProperty property = _property.serializedObject.GetIterator();
        while (property.Next(true))
        {
            if (property.propertyPath.Contains(ParentPropertyName) == true)
            {
                if (property.name.Contains(_flagAttribute.FieldToCheck))
                {
                    return CheckMask(property.intValue, _flagAttribute.Mask); 
                }
            }
        }
        Debug.LogWarning("Property not found : " + _flagAttribute.FieldToCheck);
        return true;
    }

    public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
    {
        string parentPropertyName = _property.propertyPath.Substring(0, _property.propertyPath.LastIndexOf('.'));
        if (CheckCondition(attribute as AnyFlagRequirement, _property, parentPropertyName) == true)
        {
            return EditorGUI.GetPropertyHeight(_property, _label);
        }
        return 0.0f;
    }

    virtual protected bool CheckMask(int Value, int Mask)
    {
        return false;
    }
}

[CustomPropertyDrawer(typeof(AnyFlagRequirement))]
public class AnyFlagRequirementPropertyDrawer : FlagRequirementPropertyDrawer
{
    override protected bool CheckMask(int Value, int Mask) 
    {
        return (Value & Mask) > 0;
    }
}

[CustomPropertyDrawer(typeof(AllFlagRequirement))]
public class AllFlagRequirementPropertyDrawer : FlagRequirementPropertyDrawer
{
    override protected bool CheckMask(int Value, int Mask)
    {
        return (Value & Mask) == Mask;
    }
}

#endif

#endregion