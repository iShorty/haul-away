#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TagChecker : MonoBehaviour
{
    [SerializeField]
    string _tagName = default;

    [SerializeField]
    KeyCode _checkKeycode = KeyCode.B;

    private void Update()
    {
        if (Input.GetKeyDown(_checkKeycode))
        {
            List<GameObject> allRootGameObjects = new List<GameObject>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                allRootGameObjects.AddRange(scene.GetRootGameObjects());
            }

            foreach (var item in allRootGameObjects)
            {
                RecursiveCheckTag(item);
            }

        }
    }


    void RecursiveCheckTag(GameObject go)
    {
        //Check this go's layer
        CheckTag(go);

        if (go.transform.childCount <= 0)
        {
            return;
        }

        //Call recursive check layer on every one of its children
        foreach (Transform child in go.transform)
        {
            RecursiveCheckTag(child.gameObject);
        }
    }

    void CheckTag(GameObject go)
    {
        if (go.CompareTag(_tagName))
        {
            Debug.Log($"Gameobject {go.name} is still using the tag {_tagName}", go);
        }
    }

}

#endif