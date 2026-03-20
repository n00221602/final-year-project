using System.Collections;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    //TO-DO
    //Handle inputs. Primary (right hand) and secondary (left hand) attacks. DONE
    //Handle combat animations. ALSO look into root motion.
    //For hitboxes, enable the hitbox depending on the animation frame.

    public Animator animator;
    public PlayerMovement playerMovement;
    public RoomEnter roomEnter;
    public HealthSystem healthSystem;

    public Transform player;
    public GameObject primaryHitbox;
    [HideInInspector] public bool hit = false;
    float playerDamage = 0.34f;

    float closestEnemyDistance;
    GameObject closestEnemy;

    GameObject[] enemyCount;
    float enemyDif;

    float elapsedTime;

    public bool isAttacking = false;


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

        FindClosestEnemy();
    }

    //HITBOX CHECKS//
    //private void OnTriggerEnter(Collider collider)
    //{
    //    if (collider.gameObject.CompareTag("Enemy"))
    //    {
    //        Debug.Log("HIT ENEMY");
    //    }
    //}


    //Received from HitboxReporter when the hitbox detects a collision with an enemy.



    //ANIMATIONS//
    void PrimaryAttack()
    {
        if (isAttacking) return;
        isAttacking = true;

        if (closestEnemyDistance < 5f && closestEnemy != null)
        {
            //Vector3 lookPosition = closestEnemy.transform.position;
            //lookPosition.y = player.transform.position.y;
            //player.transform.LookAt(lookPosition);
            //player.transform.position = Vector3.MoveTowards(player.transform.position, closestEnemy.transform.position, 10f * Time.deltaTime);
            //playerMovement.rb.AddForce(closestEnemy.transform.position * playerMovement.moveSpeed * 10f, ForceMode.Force);

            StartCoroutine(LockOn(closestEnemy.transform.position));

        }

        animator.SetTrigger("AttackP");

    }

    void SecondaryAttack()
    {
        if (isAttacking) return;
        isAttacking = true;

        if (closestEnemy != null)
        {
            StartCoroutine(FaceEnemy(closestEnemy.transform.position, 0.15f));
        }

        animator.SetTrigger("AttackS");
    }


    //COMBAT LOGIC//
    void FindClosestEnemy()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy");

        //Debug.Log("Enemy count: " + enemyCount.Length);
        closestEnemyDistance = Mathf.Infinity;
        closestEnemy = null;

        //Check distance for each enemy
        foreach (GameObject enemy in enemyCount)
        {
            enemyDif = Vector3.Distance(player.position, enemy.transform.position);

            if (enemyDif < closestEnemyDistance)
            {
                closestEnemyDistance = enemyDif;
                closestEnemy = enemy;
                //Debug.Log("Closest Enemy Distance: " + closestEnemyDistance);
            }
        }
    }

    private IEnumerator LockOn(Vector3 enemyPosition)
    {
        float lockOnTime = 0.15f;
        elapsedTime = 0f;

        Vector3 direction = (enemyPosition - player.position).normalized;
        direction.y = 0f;

        // Target rotation setup
        Quaternion startRot = player.rotation;
        Quaternion targetRot = Quaternion.LookRotation(direction);

        Vector3 destination = enemyPosition - (direction * 0.5f);
        Vector3 startingPos = playerMovement.rb.position; // Use Rigidbody position for accuracy

        while (elapsedTime < lockOnTime)
        {
            // 1. Smoothly Rotate
            player.rotation = Quaternion.Slerp(startRot, targetRot, elapsedTime / lockOnTime);

            // 2. Smoothly Move
            Vector3 nextPos = Vector3.Lerp(startingPos, destination, elapsedTime / lockOnTime);
            playerMovement.rb.MovePosition(nextPos);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap perfectly at the end to ensure it didn't fall short
        player.rotation = targetRot;
    }

    // This handles ONLY Rotation over time (for secondary attacks)
    private IEnumerator FaceEnemy(Vector3 enemyPosition, float turnDuration)
    {
        elapsedTime = 0f;

        Vector3 direction = (enemyPosition - player.position).normalized;
        direction.y = 0f;

        Quaternion startRot = player.rotation;
        Quaternion targetRot = Quaternion.LookRotation(direction);

        while (elapsedTime < turnDuration)
        {
            player.rotation = Quaternion.Slerp(startRot, targetRot, elapsedTime / turnDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.rotation = targetRot;
    }

    //ANIMATION EVENT FUNCTIONS//

    //Called at start of swing
    void PrimaryHitboxOn()
    {
        Debug.Log("SWING");
        primaryHitbox.SetActive(true);
    }

    public void OnHitboxHit(GameObject hitEnemy)
    {
        Debug.Log("Hitbox struck: " + hitEnemy.name);

        //Get enemy's health system and apply damage
        hit = true;
        hitEnemy.GetComponent<HealthSystem>().TakeDamage(playerDamage);
    }

    //Called at end of swing
    void PrimaryHitboxOff()
    {
        primaryHitbox.SetActive(false);
        isAttacking = false;
        hit = false;
    }

    void SecondaryHitboxOn()
    {
        Debug.Log("BANG");
    }

    void SecondaryHitboxOff()
    {
        isAttacking = false;
    }
}
