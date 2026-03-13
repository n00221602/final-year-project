using UnityEngine;

public class HitboxReporter : MonoBehaviour
{
    public CombatSystem combatSystem; // Drag Player into here

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Now you KNOW it was the hand that hit them!
            Debug.Log("HITBOX DETECTED");
            combatSystem.OnHitboxHit(other.gameObject); // Call a method in CombatSystem to handle the hit
        }
    }
}
