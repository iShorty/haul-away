using UnityEngine;
#if UNITY_EDITOR
public partial class PlayerManager
{
    void EditorChecks()
    {
        // Debug.Assert(_players.Count > 0, $"The playermanager cant find any players!", this);
        Debug.Assert(_respawnIndicators.Length > 0, $"The playermanager doesnt have any respawn indicators cached!", this);
    }
}


#endif