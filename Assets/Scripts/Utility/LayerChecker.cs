#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LayerChecker : MonoBehaviour
{
    [SerializeField]
    string _layerName = default;

    [SerializeField]
    KeyCode _checkKeycode = KeyCode.B;

    int _layerIndex = default;

    // Start is called before the first frame update
    void Start()
    {
        _layerIndex = LayerMask.NameToLayer(_layerName);


    }
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
                RecursiveCheckLayer(item);
            }

        }
    }


    void RecursiveCheckLayer(GameObject go)
    {
        //Check this go's layer
        CheckLayer(go);

        if (go.transform.childCount <= 0)
        {
            return;
        }

        //Call recursive check layer on every one of its children
        foreach (Transform child in go.transform)
        {
            RecursiveCheckLayer(child.gameObject);
        }
    }

    void CheckLayer(GameObject go)
    {
        if (go.layer == _layerIndex)
        {
            Debug.Log($"Gameobject {go.name} is still using the layer {_layerName}", go);
        }
    }

}

#endif