using UnityEngine;
using System;
using UnityEditor;


[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class ConditionalReadOnlyAttribute : PropertyAttribute
{
    public string m_ConditionalFieldPropPath = default;
    public bool m_ConditionToMeet = default;
    public bool m_IsFullPropertyPath = default;

    //Set isFullPropertyPath to be false if the conditionalfield is not on the same propertypath level as the field u use this attribute on
    public ConditionalReadOnlyAttribute(string propertyPath, bool isFullPropertyPath = true)
    {
        m_ConditionalFieldPropPath = propertyPath;
        m_ConditionToMeet = true;
        m_IsFullPropertyPath = isFullPropertyPath;
    }

    public ConditionalReadOnlyAttribute(string propertyPath, bool conditionToMeet, bool isFullPropertyPath = true)
    {
        m_ConditionalFieldPropPath = propertyPath;
        m_ConditionToMeet = conditionToMeet;
        m_IsFullPropertyPath = isFullPropertyPath;
    }

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ConditionalReadOnlyAttribute))]
public class ConditionalReadOnlyPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalReadOnlyAttribute condReadATT = (ConditionalReadOnlyAttribute)attribute;

        //Check if the conditionalfield path is fullproperty path. If it isnt get full propertypath
        string fullPropPathToConditionalField = condReadATT.m_IsFullPropertyPath ? condReadATT.m_ConditionalFieldPropPath : GetFullPropertyPath(property, condReadATT);

        bool conditionalFieldState = CheckConditionalFieldState(property, fullPropPathToConditionalField);

        bool wasEnabled = GUI.enabled;
        GUI.enabled = conditionalFieldState == condReadATT.m_ConditionToMeet;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = wasEnabled;
    }

    string GetFullPropertyPath(SerializedProperty property, ConditionalReadOnlyAttribute condReadATT)
    {
        //Get propertypath of the field u wanna hide
        string propertyPath = property.propertyPath;

        //Assuming that the masterswitch/conditional field is in the same propertypath level (foreseeing structs and classes)
        string conditionalFieldPropPath = propertyPath.Replace(property.name, condReadATT.m_ConditionalFieldPropPath);

        return conditionalFieldPropPath;
    }

    bool CheckConditionalFieldState(SerializedProperty property, string fullPropPathToConditionalField)
    {
        //Find the property 
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(fullPropPathToConditionalField);

        bool enabled;
        if (sourcePropertyValue != null)
            enabled = sourcePropertyValue.boolValue;
        else
        {
            Debug.LogWarning("There is no conditional field at propertypath: " + fullPropPathToConditionalField);
            enabled = true;
        }

        return enabled;
    }




}
#endif



}
