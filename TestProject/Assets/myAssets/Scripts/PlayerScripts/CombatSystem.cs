using System.Collections;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public Animator animator;
    public PlayerMovement playerMovement;
    public RoomEnter roomEnter;
    public HealthSystem healthSystem;
    public HitboxReporter hitboxReporter;

    public Transform player;
    public GameObject primaryHitbox;

    //hit bool is used to prevent constant hits with the same attack. It is checked within HitboxReporter.cs
    [HideInInspector] public bool hit = false;

    float playerDamage = 0.34f;

    float closestEnemyDistance;
    GameObject closestEnemy;

    GameObject[] enemyCount;
    float enemyDif;

    //float elapsedTime;

    [HideInInspector] public bool isAttacking = false;



    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        roomEnter = GetComponent<RoomEnter>();

        //Left click for primary attack, right click for secondary attack.
        Input.GetKeyDown(KeyCode.Mouse0);
        Input.GetKeyUp(KeyCode.Mouse1);
        primaryHitbox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PrimaryAttack();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SecondaryAttack();
        }

        if (isAttacking)
        {
            playerMovement.rb.linearVelocity = Vector3.zero;
            playerMovement.horizontalInput = 0f;
            playerMovement.verticalInput = 0f;

        }
        else
        {
            FindClosestEnemy();
        }
    }

    //ATTACKS//
    void PrimaryAttack()
    {
        if (isAttacking) return;
        isAttacking = true;
        playerMovement.enabled = false;

        if (closestEnemyDistance < 5f && closestEnemy != null)
        {
            StartCoroutine(LockOn(closestEnemy.transform.position, true));

        }

        animator.SetTrigger("AttackP");

    }

    void SecondaryAttack()
    {
        if (isAttacking) return;
        isAttacking = true;

        if (closestEnemy != null)
        {
            StartCoroutine(LockOn(closestEnemy.transform.position, false));
        }

        animator.SetTrigger("AttackS");
    }


    //COMBAT LOGIC//

    void FindClosestEnemy()
    {
        //Get all enemies in the scene
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy");

        closestEnemyDistance = Mathf.Infinity;
        closestEnemy = null;

        //Check distance for each enemy until the smallest distance is found.
        foreach (GameObject enemy in enemyCount)
        {
            enemyDif = Vector3.Distance(player.position, enemy.transform.position);

            if (enemyDif < closestEnemyDistance)
            {
                closestEnemyDistance = enemyDif;
                closestEnemy = enemy;
            }
        }
    }

    private IEnumerator LockOn(Vector3 enemyPosition, bool isPrimaryAttack)
    {
        //Establish the duration of the lock-on and the timer.
        float lockOnTime = 0.15f;
        float elapsedTime = 0f;

        //Calculate the direction from the player to the enemy. Reset Y to 0 to ensure the player doesn't rotate vertically.
        Vector3 direction = (enemyPosition - player.position).normalized;
        direction.y = 0f;

        //The set destination is slightly in front of the enemy to prevent the player from moving inside the enemy's hitbox.
        Vector3 startPos = playerMovement.rb.position;
        Vector3 destination = enemyPosition - (direction * 0.5f);

        while (elapsedTime < lockOnTime)
        {
            player.rotation = Quaternion.LookRotation(direction);

            //Move towards enemy if primary attack bool is activated
            if (isPrimaryAttack)
            {
                Vector3 nextPos = Vector3.Lerp(startPos, destination, elapsedTime / lockOnTime);
                playerMovement.rb.MovePosition(nextPos);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    //ANIMATION EVENT FUNCTIONS//

    //Called by animation event at start of swing
    void PrimaryHitboxOn()
    {
        primaryHitbox.SetActive(true);
    }

    //Called animation event at end of swing
    void PrimaryHitboxOff()
    {
        primaryHitbox.SetActive(false);
        isAttacking = false;
        playerMovement.enabled = true;
        hitboxReporter.hit = false;
    }


    public void OnHitboxHit(GameObject hitEnemy)
    {
        Debug.Log("Hitbox struck: " + hitEnemy.name);

        //Get enemy's health system and apply damage
        hitboxReporter.hit = true;
        hitEnemy.GetComponent<HealthSystem>().TakeDamage(playerDamage);
    }
}
