using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    public static class TransformExtensions
    {
        public static T GetComponentOfChild<T>(this Transform t, int index)
        {
            return t.GetChild(index).GetComponent<T>();
        }

        public static string GetGameObjectPath(this Transform transform)
        {
            string scenePath = transform.name;

            while (transform.parent != null)
            {
                transform = transform.parent;
                scenePath = transform.name + "/" + scenePath;
            }
            return scenePath;
        }
    }

