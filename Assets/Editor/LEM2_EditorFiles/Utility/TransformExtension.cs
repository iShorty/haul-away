using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class TransformExtension
{
    ///<Summary>Gets the full path of a Transform which includes the scene name</Summary>
    public static string GetFullPath(this Transform t)
    {
        string path = t.name;

        while (t.parent != null)
        {
            path = path.Insert(0, $"{t.parent.name}/");
            t = t.parent;
        }

        //Append scene name
        path = path.Insert(0, $"{t.root.gameObject.scene.name}/");
        return path;
    }

    ///<Summary>Checks if a scene name matches is found in the path</Summary>
    static bool CheckSceneNameIsPresent(string sceneName, string path, out string pathWithoutSceneName)
    {
        pathWithoutSceneName = string.Empty;

        //Checking if there is a scene name in the path
        int slashFound = path.IndexOf("/");
        if (slashFound == -1)
        {
#if UNITY_EDITOR
            Debug.Log($"There is no scene path in {path}");
#endif
            return false;
        }

        //===================== CURRENTNAME = SCENE NAME ===========================
        string sceneNameFound = path.Substring(0, slashFound);

        //Check if the scene name in path matches the Scene.name
        if (sceneNameFound != sceneName)
        {
#if UNITY_EDITOR
            Debug.Log($"The scene in {path} is not the same scene as {sceneName}");
#endif
            return false;
        }

        //Else the scene name matches up to the path's scene name
        pathWithoutSceneName = path.Remove(0, slashFound + 1);
        return true;
    }

    ///<Summary>Finds a transform given the full path of the Transform including the scene name</Summary>
    public static bool GetTransform(this Scene scene, string fullPath, out Transform transform)
    {
        //Check if the scene's name matches up to the path's scene name
        if (!CheckSceneNameIsPresent(scene.name, fullPath, out string pathWithoutSceneName))
        {
            transform = null;
            return false;
        }

        //=================== CURRENTNAME = ROOT GAMEOBJECT ==========================
        int slashFound = pathWithoutSceneName.IndexOf("/");
        string rootGameObjectName, restOfThePath;
        if (slashFound == -1)
        {
            rootGameObjectName = pathWithoutSceneName;
            restOfThePath = string.Empty;
        }
        else
        {
            rootGameObjectName = pathWithoutSceneName.Substring(0, slashFound);
            //Remove the rootgameobjectname's string
            restOfThePath = pathWithoutSceneName.Remove(0, slashFound + 1);
        }

        // Debug.Log($"The scene name <{scene.name}> is present in the fullPath <{fullPath}>! RootGameObjectName found is <{rootGameObjectName}> in the path without scene name: <{pathWithoutSceneName}> ");

        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (var rootGameObject in rootObjects)
        {
            // Debug.Log($"Current rootobject with the name <{rootGameObject.name}> is being compared with the inputed root object string: {rootGameObjectName}", rootGameObject);

            if (rootGameObject.name != rootGameObjectName)
            {
                continue;
            }

            // Debug.Log($"Using the rest of the path: <{restOfThePath}>, i am finding the transform with the flowchart component", rootGameObject);

            //If the rest of the path isnt empty, then the root object holds the transform we are looking for
            if (string.IsNullOrEmpty(restOfThePath))
            {
                transform = rootGameObject.transform;
                return true;
            }

            //Else we must find the transform with the rest of the path
            transform = rootGameObject.transform.Find(restOfThePath);

            //Since Find() might return null,
            return transform == null ? false : true;
        }

        //Else if none of the root object's names matched,
        transform = null;
        return false;
    }



}
