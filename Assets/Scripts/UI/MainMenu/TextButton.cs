using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using AudioManagement;

[RequireComponent(typeof(Animator))]
public class TextButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    static readonly Color SELECTED_COLOUR = new Color(0.98f, 0.908f, 0.421f);
    static readonly Color UNSELECTED_COLOUR = new Color(0.924f, 0.924f, 0.924f);

    Animator _animator = default;

    TextMeshProUGUI _text = default;

    public void OnDeselect(BaseEventData eventData)
    {
        _animator.SetTrigger("Deselect");
        _text.color = UNSELECTED_COLOUR;
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Debug.Log($"Selected {this.name}",this);
        _animator.SetTrigger("Select");
        _text.color = SELECTED_COLOUR;
        AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_ButtonHighlight, true);
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
#if UNITY_EDITOR
        Debug.Assert(_animator, $"There must be a textmeshpro ui as a child of {name}!", this);
#endif

        _text = GetComponentInChildren<TextMeshProUGUI>();

        _text.fontMaterial.SetFloat("_UnderlayOffsetX",1);
        _text.fontMaterial.SetFloat("_UnderlayOffsetY",-1);
    }


}
