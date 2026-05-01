using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{

    public Image health;
    [SerializeField] private UniversalRendererData rendererData;
    public Animator playerAnimator;

    public PlayerMovement playerMovement;
    public ThirdPersonCam thirdPersonCam;
    public CombatSystem combatSystem;
    public ScreenUI screenUI;
    public GameObject playerDeathLight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Assign all health bars to full.
        health.fillAmount = 1f;
    }

    void Update()
    {
        //Call the death function if the health bar is empty.
        if (health.fillAmount == 0f)
        {
            OnDeath();
        }
    }

    //Used by external scripts for inflicting health damage.
    public void TakeDamage(float damage)
    {
        health.fillAmount -= damage;
    }

    public void OnDeath()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            //Destroys the enemy object from the scene.
            Destroy(gameObject.transform.parent.gameObject);
        }

        if (gameObject.CompareTag("Player"))
        {
            //Disable player scripts and stop existing momentum.
            thirdPersonCam.enabled = false;
            playerMovement.enabled = false;
            combatSystem.enabled = false;

            Rigidbody rb = playerMovement.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            //Death animation and renderer feature are triggered.
            playerAnimator.SetTrigger("PlayerDeath");
            if (rendererData != null)
            {
                var playerMaskFeature = rendererData.rendererFeatures.Find(f => f.name == "PlayerDeathMask");
                if (playerMaskFeature != null)
                {
                    playerDeathLight.SetActive(true);
                    playerMaskFeature.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    screenUI.gameOverUI.SetActive(true);
                }
            }
        }
    }
}