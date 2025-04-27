using UnityEngine;

[CreateAssetMenu(fileName = "EnemyShipInfo", menuName = "Enemy/EnemyShipInfo", order = 0)]
public class EnemyShipInfo : EnemyInfo {
    [Header("Pirate specific stats")]
	public string thing = "stuff here";
    
}