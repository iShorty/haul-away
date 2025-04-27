using System.Collections.Generic;
using System;
using System.Reflection;

    public static class ReflectionExtensions
    {
        /// <summary>
        /// Perform a deep copy of the class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>A deep copy of obj.</returns>
        /// <exception cref="System.ArgumentNullException">Object cannot be null</exception>
        public static T DeepCopy<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("Object cannot be null");
            }
            return (T)DoCopy(obj);
        }


        //Basically, check the type of the object and then return their respective values/references as new instances based on their respective Types
        /// <summary>
        /// Does the copy.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Unknown type</exception>
        static object DoCopy(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            Type objType = obj.GetType();


            // Value type returned as just a value
            if (objType.IsValueType || objType == typeof(string))
            {
                return obj;
            }

            // Array
            else if (objType.IsArray)
            {
                //If the object we are cloning is an array, we need to know the type of the element of the Array as well!
                Type elementType = objType.GetElementType();

                Array array = obj as Array;

                Array newInstance = Array.CreateInstance(elementType, array.Length);

                for (int i = 0; i < array.Length; i++)
                {
                    //We call DoCopy for each element here because 
                    //1) We can just do a recursive call and save on writing another block of code to check the object type
                    //2) Hell if the elementType is an Array, this recursive call will be super neat clone all the nested Array types!
                    newInstance.SetValue(DoCopy(array.GetValue(i)), i);
                }

                return Convert.ChangeType(newInstance, objType);
            }

            // Unity Object which means it will returned as a reference 
            else if (typeof(UnityEngine.Object).IsAssignableFrom(objType))
            {
                return obj;
            }

            // Class -> Recursion
            else if (objType.IsClass)
            {
                //Create an instance of the Class Type 
                var copy = Activator.CreateInstance(obj.GetType());

                //Get all fields from the class
                var fields = objType.GetAllFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (FieldInfo field in fields)
                {
                    //Do a mini check here to see if obj is empty
                    var fieldValue = field.GetValue(obj);

                    if (fieldValue != null)
                    {
                        //if it aint empty, copy the values over to "copy" instance by recursively calling DoCopy
                        field.SetValue(copy, DoCopy(fieldValue));
                    }
                }

                return copy;
            }

            // Fallback
            else
            {
                throw new ArgumentException("Unknown type");
            }
        }

        /// <summary>
        /// Gets all fields from an object and its hierarchy inheritance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>All fields of the type.</returns>
        static List<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
        {
            // Early exit if Object type
            if (type == typeof(System.Object))
            {
                return new List<FieldInfo>();
            }

            // Recursive call
            List<FieldInfo> fields = type.BaseType.GetAllFields(flags);

            //This is the SystemReflection method that ACTUALLY does the getting of fields
            //Get Fields which are of flags definition (defined when you call GetAllFields) and DeclaredOnly 
            //Definition for DeclaredOnly enum: Specifies that only members declared at the level of the supplied type's hierarchy should be considered. Inherited members are not considered.
            fields.AddRange(type.GetFields(flags | BindingFlags.DeclaredOnly));
            return fields;
        }

    }