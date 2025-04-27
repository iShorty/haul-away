using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumBasedArrayExtension
{

    public static T[] SortToEnumOrder<T, EnumType>(T[] array, Func<string, int, Predicate<T>> function)
    {
        //List delcaration
        List<T> list = new List<T>();
        list.AddRange(array);


        //Enum local var
        var enumType = typeof(EnumType);
        string[] allEnumNames = Enum.GetNames(enumType);
        var enumValues = Enum.GetValues(enumType);

        //then set the array to correct enum size
        array = new T[allEnumNames.Length];

        foreach (int enumValueAsInt in enumValues)
        {
            //  Enum.GetName(enumType,enumValueAsInt);
            string currentValueName = allEnumNames[enumValueAsInt];

            //Find the listElement index which gets the matching T type which correlates with the enumValueAsInt or currentValueName
            int listElementIndex = list.FindIndex(function?.Invoke(currentValueName, enumValueAsInt));

            T element;

            //If corresponding enum value doesnt exists,
            if (listElementIndex < 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"The enum value {currentValueName} does not have its array element assigned at index {enumValueAsInt}!");
#endif
                element = default;
            }
            else
            {
                //Else if it exists,
                element = list[listElementIndex];
                list.RemoveAt(listElementIndex);
            }

            array[enumValueAsInt] = element;
        }

        return array;
    }


}
