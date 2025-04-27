namespace LinearEffectsEditor
{
    using UnityEngine;
    using UnityEditor;

    public static class EditorDebugExtension
    {
        #region Debug
        //Just a debug method
        public static void PrintAllProperties(this SerializedObject serializedObject)
        {
            SerializedProperty firstP = serializedObject.GetIterator();
            while (firstP.NextVisible(true))
            {
                Debug.Log(firstP.name);
            };
        }

        //Just a debug method
        public static void PrintAllProperties(this SerializedProperty serializedProperty)
        {
            while (serializedProperty.NextVisible(true))
            {
                Debug.Log(serializedProperty.name);
            };
            serializedProperty.Reset();
        }


        public static bool TryGetProperty(this SerializedProperty p, string propertyName, out SerializedProperty propertyFound)
        {
            propertyFound = p.FindPropertyRelative(propertyName);
            if (propertyFound != null)
            {
                return true;
            }

            Debug.LogWarning($"The property named: {propertyName} inside the Block class has been renamed to something else or it doesnt exist anymore! \n Property Path: {p.propertyPath}");
            return false;
        }
        #endregion

        public static bool DeleteArrayElement(this SerializedProperty arrayProperty, System.Predicate<SerializedProperty> predicate)
        {
            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                SerializedProperty property = arrayProperty.GetArrayElementAtIndex(i);

                if (predicate.Invoke(property))
                {
                    arrayProperty.DeleteArrayElementAtIndex(i);
                    return true;
                }

            }

            return false;
        }


    }

}