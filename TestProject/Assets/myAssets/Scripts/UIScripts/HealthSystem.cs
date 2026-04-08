using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{

    public Image health;
    [SerializeField] private UniversalRendererData rendererData;
    public Animator playerAnimator;

    public PlayerMovement playerMovement;
    public ThirdPersonCam thirdPersonCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health.fillAmount = 0.1f;
    }

    void Update()
    {
        if (health.fillAmount == 0f)
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

        //TO-DO: Add death animation, sound effects, and respawn mechanics.
        if (gameObject.CompareTag("Enemy"))
        {
            Debug.Log(gameObject.name + " DEAD");

            Destroy(gameObject);
        }

        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("Player DEAD");

            //Disable player movement script and stop existing momentum.
            thirdPersonCam.enabled = false;
            playerMovement.enabled = false;
            Rigidbody rb = playerMovement.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            //Death animation and renderer feature are triggered
            playerAnimator.SetTrigger("PlayerDeath");
            if (rendererData != null)
            {
                var playerMaskFeature = rendererData.rendererFeatures.Find(f => f.name == "PlayerDeathMask");
                if (playerMaskFeature != null)
                {
                    playerMaskFeature.SetActive(true);
                }
            }

            //Get all active agents and set them to inactive.

            //Game over UI

            //set player mask back to false when player retrys.
        }
    }

    public void PlayerGracePeriod(NavMeshAgent enemy)
    {
        //The player is invulnerable for a short period of time once hit, but only by the enemy that hit them.
        Debug.Log("GRACE PERIOD");

    }
}