using UnityEngine;

public class HitboxReporter : MonoBehaviour
{
    public CombatSystem combatSystem;
    public EnemyAI enemyAI;

    [HideInInspector] public GameObject hitEnemy;
    [HideInInspector] public bool hit = false;

    private void OnTriggerEnter(Collider other)
    {
        //If the hitbox comes in contact with an enemy, run the OnHitboxHit function from CombatSystem.
        if (other.CompareTag("Enemy") && hit != true)
        {
            combatSystem.OnHitboxHit(other.gameObject);
        }

        // If the hitbox comes in contact with the player, run the OnPlayerHit function from EnemyAI.  
        if (other.CompareTag("Player") && hit != true)
        {
            Debug.Log("Player hit by enemy!");
            enemyAI.OnPlayerHit(other.gameObject);
        }
    }
}