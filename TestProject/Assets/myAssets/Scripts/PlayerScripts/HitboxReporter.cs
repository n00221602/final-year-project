using UnityEngine;

public class HitboxReporter : MonoBehaviour
{
    public CombatSystem combatSystem; // Drag Player into here
    [HideInInspector] public GameObject hitEnemy;

    private void OnTriggerEnter(Collider other)
    {
        //If the hitbox comes in contact with an enemy, run the OnHitboxHit function from CombatSystem.
        if (other.CompareTag("Enemy") && combatSystem.hit != true)
        {
            combatSystem.OnHitboxHit(other.gameObject);
        }
    }
}
