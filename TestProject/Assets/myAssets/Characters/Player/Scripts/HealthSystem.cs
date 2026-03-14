using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{

    public Image health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health.fillAmount = 1f;
    }

    void Update()
    {
        if (health.fillAmount <= 0)
        {
            OnDeath();
        }
    }

    public void TakeDamage(float damage)
    {
        health.fillAmount -= damage;
    }

    public void OnDeath()
    {
        Debug.Log(gameObject.name + "DEAD");
        //TO-DO: Add death animation, sound effects, and respawn mechanics.
    }
}
