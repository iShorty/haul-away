using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIExtensions
{

    public static void ToggleButtonNavigation(bool state, params Selectable[] selectables)
    {
        Navigation navMode = selectables[0].navigation;
        navMode.mode = state ? Navigation.Mode.Automatic : Navigation.Mode.None;

        foreach (var item in selectables)
        {
            item.navigation = navMode;
        }
    }

}
