#if UNITY_EDITOR
namespace LinearEffects
{
    using UnityEditor;
    using UnityEngine;

    public interface ISavableData
    {
        void SaveToSerializedProperty(SerializedProperty property);
        void LoadFromSerializedProperty(SerializedProperty property);
    }

    #region    //============================== SERIALIZED PROPERTY EXTENSIONS ================================
    public static class BlockSerializedPropertyExtension
    {
        //Used wehn the window editor adds a new nodeblock 
        public static SerializedProperty AddToSerializedPropertyArray<T>(this SerializedProperty arrayProperty, T instance) where T : ISavableData
        {
            if (!arrayProperty.isArray)
            {
                Debug.Log($"Serialized Property {arrayProperty.name} is not an array!");
                return null;
            }

            arrayProperty.serializedObject.Update();
            arrayProperty.arraySize++;
            SerializedProperty lastElement = arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1);
            instance.SaveToSerializedProperty(lastElement);
            arrayProperty.serializedObject.ApplyModifiedProperties();
            return lastElement;
        }
    }
    #endregion

}
#endif