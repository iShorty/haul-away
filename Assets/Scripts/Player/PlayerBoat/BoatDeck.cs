using UnityEngine;



public class BoatDeck : MonoBehaviour
{
    public float _DeckSize = 5f;
    [SerializeField] VFXInfo dropDustCloudInfo = default;
    public event System.Action onEnterBoatDeck;


#if UNITY_EDITOR
    private void Awake()
    {
        Debug.Assert(gameObject.layer == Constants.For_Layer_and_Tags.LAYERINDEX_PLAYERDECK, $"Boat deck needs to be using the {Constants.For_Layer_and_Tags.LAYERNAME_PLAYERDECK} layer!", this);
    }
#endif

    #region Trigger Methods

    private void OnTriggerEnter(Collider other)
    {
        EvaluateTrigger(other);
    }

    private void EvaluateTrigger(Collider other)
    {
        // Debug.Log("deck hit " + other.attachedRigidbody.gameObject.name, other.gameObject);
        //Filter out station objects
        if (BoatManager.IsPartOfBoat(other)) return;

            
        VFXObj e = VFXPool.GetInstanceOf(dropDustCloudInfo.Prefab, other.transform.position);
        e.Initialise();
        
        //Projectiles, Players and BaseCargo are on the interactable layer and therefore subjected to this code
        //Differentiate between cargo from players & projectiles
        if (PlayerPickableManager.IsCargo(other))
        {

            // Check if its a mystery box to spawn the actual cargo, invoke event if its a cargo to add to boat inv or etc
            MysteryBox box = other.attachedRigidbody.GetComponent<MysteryBox>();
            box?.Box_OnEvaluateBoatDeckTrigger();
            onEnterBoatDeck?.Invoke();
            return;
        }

    }

    #endregion



}
