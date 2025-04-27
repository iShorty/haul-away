using UnityEngine;



public class VFXObj : MonoBehaviour
{
    [field: Header("===== VFX INFO ====="), SerializeField, RenameField(nameof(VFXInfo))]
    ///<Summary>The VFX info which this VFX instance belongs to. Return to object pooler using this VFXInfo</Summary>
    public VFXInfo VFXInfo { get; protected set; } = default;
    // Time marker when it was spawned/pulled from pool
    float timer = 0;

    public void Initialise() {
        timer = VFXInfo.Duration;
        VFXPool.RegisterVFX(this);
    }

    protected void OnDisable()
    {
        VFXPool.UnregisterVFX(this);
    }

    public void GameUpdate() {
        // Anything else goes here

        // When despawned, unregister it from VFX update loop and then return to pooler.
        if(!Despawn()) {
            VFXPool.UnregisterVFX(this);
            VFXPool.ReturnInstanceOf(this);
        }
    }
    
    bool Despawn() {
        // Not finished doing VFX things
        if(timer > 0) {
            timer -= Time.deltaTime;
            return true;
        }
        // Finished
        timer = 0;
        return false;
    }
}
