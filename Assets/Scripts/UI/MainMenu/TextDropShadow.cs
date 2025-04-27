using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextDropShadow : MonoBehaviour
{
    private void Awake()
    {
        TextMeshProUGUI _text = GetComponentInChildren<TextMeshProUGUI>();

        _text.fontMaterial.SetFloat("_UnderlayOffsetX", 1);
        _text.fontMaterial.SetFloat("_UnderlayOffsetY", -1);
    }
}
