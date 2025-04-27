
namespace LinearEffectsEditor
{
    using UnityEngine;
    using UnityEditor;
    using LinearEffects;

    //Responsible for drawing and updating the current being inspected effect
    public partial class BlockInspector : ImprovedEditor
    {
        #region Observed Effect Fields
        SerializedProperty _currObservedProperty = default;
        #endregion

        const float OBSERVED_EFFECTBG_BORDER = 50f,
           OBSERVED_EFFECT_YOFFSET = 20f
           ;

        #region Observed Effect

        void BottomHalf_OnGUI_ObservedEffect(float inspectorWidth)
        {
            if (_currObservedProperty != null && _prevClickedIndex == CurrentClickedListIndex)
            {
                //Current effect is still the same
                BottomHalf_DrawObservedEffect(inspectorWidth);
                return;
            }

            if (!BottomHalf_TryGetNewObservedEffect())
            {
                return;
            }


            BottomHalf_DrawObservedEffect(inspectorWidth);
        }

        void BottomHalf_DrawObservedEffect(float inspectorWidth)
        {
            //======== DRAWING EFFECT ==========
            float height = EditorGUI.GetPropertyHeight(_currObservedProperty);

            // ========== DRAWING BG BOX =============
            // Color prevColor = GUIExtensions.Start_GUI_ColourChange(OBSERVED_EFFECT_BOXCOLOUR);
            GUILayout.Box(string.Empty, GUILayout.Height(height + OBSERVED_EFFECTBG_BORDER), GUILayout.MaxWidth(inspectorWidth));
            // GUIExtensions.End_GUI_ColourChange(prevColor);

            //========== DRAWING EFFECT =============
            Rect prevRect = GUILayoutUtility.GetLastRect();
            prevRect.y += OBSERVED_EFFECT_YOFFSET;
            EditorGUI.PropertyField(prevRect, _currObservedProperty, true);

            //========= SAVE EFFECT'S CHANGES ===========
            if (_currObservedProperty.serializedObject.ApplyModifiedProperties())
            {
                _currObservedProperty.serializedObject.Update();
            }
        }

        bool BottomHalf_TryGetNewObservedEffect()
        {
            //Get currently selected order element
            if (!TopHalf_GetOrderArrayElement(CurrentClickedListIndex, out SerializedProperty orderElement))
            {
                return false;
            }

            //===== GETTING OBSERVED EFFECT =====
            //convert holder to serializedobject
            BaseEffectExecutor holder = (BaseEffectExecutor)orderElement.FindPropertyRelative(Block.EffectOrder.PROPERTYNAME_REFHOLDER).objectReferenceValue;

            //Since we are destroying unused effect executor whenever we remove a effectorder from the executor, we need to check if holder is null
            if (holder == null)
            {
                return false;
            }

            SerializedObject holderObject = new SerializedObject(holder);

            //Get the effectDatas array as serializedProperty
            SerializedProperty effectDataArray = holderObject.FindProperty(BaseEffectExecutor.EDITOR_PROPERTYNAME_EFFECTDATAS);

            //Get dataelementindex from orderElement in block
            int dataElementIndex = orderElement.FindPropertyRelative(Block.EffectOrder.PROPERTYNAME_DATAELEMENTINDEX).intValue;

            //Somehow the effect order instance still exists when i delete them so i can still apparently get the dataelementindex but the effectDataArray already has deleted all the array elements and hence causes an error when i try to GetArrayElementAtIndex()
            if (dataElementIndex >= effectDataArray.arraySize)
                return false;


            //Get current selected effect through the use of the EffectData array and dataelementindex
            _currObservedProperty = effectDataArray.GetArrayElementAtIndex(dataElementIndex);
            return true;
        }

        #endregion

    }
}
