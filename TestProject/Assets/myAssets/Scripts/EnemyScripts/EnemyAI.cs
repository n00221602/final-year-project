using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //public Animator animator;
    //public Transform player;

    //TODO - change bool checks to run once between switching states instead of all at once for each function.
    //Fix timing for the enemy charge.

    private Transform player;
    public NavMeshAgent agent;
    public Animator animator;
    public GameObject hitArea;
    public HitboxReporter hitboxReporter;

    public Collider hitAreaHitbox;

    public HealthSystem playerHealth;

    float jumpDamage = 0.25f;

    AnimatorStateInfo stateInfo;
    private Vector3 chargingStartPosition;

    public enum State
    {
        Idle,
        Aiming,
        Charging,
        Landing
    }

    private State currentState;
    float playerDistance;

    //float closeRangeDistance = 5f;
    float aimingDistance = 10f;
    float attackDistance = 1f;
    float elapsedTime;
    bool hasCharged = false;

    Vector3 lastPlayerPosition;

    //Add Melee and Range type enemies later on.

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found");
        }

        hitArea.SetActive(false);

        //Collider hitareaHitbox = hitArea.GetComponent<Collider>();
        hitAreaHitbox.enabled = false;

        player = GameObject.Find("Player").transform;
        playerHealth = GameObject.Find("Player UI").GetComponent<HealthSystem>();

    }

    void Update()
    {
        //Constantly check difference from the player.
        playerDistance = Vector3.Distance(player.transform.position, transform.position);

        switch (currentState)
        {
            case State.Idle:
                EnemyIdle();
                break;
            case State.Aiming:
                EnemyAiming();
                break;
            case State.Charging:
                EnemyCharge();
                break;
            case State.Landing:
                EnemyLand();
                break;
        }
    }


    //The enemy idle state
    void EnemyIdle()
    {
        animator.SetBool("isIdle", true);

        //animator.SetBool("isCloseRange", false);
        agent.isStopped = true;
        agent.ResetPath();

        //Change state based on player distance.
        //Close Range
        //if (playerDistance < closeRangeDistance)
        //{
        //    animator.SetBool("isIdle", false);
        //    currentState = State.CloseRange;
        //}

        //If player is between close and long range distance, change to Aiming state
        if (playerDistance < aimingDistance)
        {
            animator.SetBool("isIdle", false);
            currentState = State.Aiming;
        }
    }

    //This state checks if the player is within its close range only it's in the enemy view.
    //void EnemyCloseRange()
    //{
    //    animator.SetBool("isCloseRange", true);
    //    agent.isStopped = false;
    //    agent.transform.LookAt(player.transform);
    //    agent.SetDestination(player.transform.position);
    //    Debug.Log("CLOSE RANGE");

    //    if (playerDistance < attackDistance)
    //    {
    //        animator.SetBool("isCloseRange", false);
    //        currentState = State.Attacking;
    //    }

    //    if (playerDistance > closeRangeDistance && playerDistance < aimingDistance)
    //    {
    //        animator.SetBool("isCloseRange", false);
    //        currentState = State.Aiming;
    //    }
    //}

    void EnemyAttack()
    {
        animator.SetBool("isAttacking", true);

        agent.isStopped = true;
        agent.ResetPath();
        Debug.Log("ATTACKING");

        //In this state, the enemy should run its attack animation and then a cooldown once animation is complete. once cooldown is up,
        //change state depending on distance. Attack again if player still within range.


        //3 if statements: for idle, close and long range.

        //if (playerDistance < closeRangeDistance && playerDistance > attackDistance)
        //{
        //    animator.SetBool("isAttacking", false);
        //    currentState = State.CloseRange;
        //}

        if (playerDistance < aimingDistance)
        {
            animator.SetBool("isAttacking", false);
            currentState = State.Aiming;
        }

        //if (playerDistance > aimingDistance)
        //{
        //    currentState = State.Idle;
        //}

    }

    //This state checks if the player is within its long range only it's in the enemy view.
    void EnemyAiming()
    {
        animator.SetBool("isAiming", true);

        elapsedTime += Time.deltaTime;
        Vector3 playerDirection = player.transform.position;
        playerDirection.y = transform.position.y;
        agent.transform.LookAt(playerDirection);

        hitArea.SetActive(true);
        hitArea.transform.position = new Vector3(player.transform.position.x, hitArea.transform.position.y, player.transform.position.z);

        if (elapsedTime > 2f)
        {
            lastPlayerPosition = player.transform.position;
            agent.transform.LookAt(lastPlayerPosition);
            hitArea.transform.position = new Vector3(lastPlayerPosition.x, hitArea.transform.position.y, lastPlayerPosition.z);
        }

        if (elapsedTime > 2.5f)
        {
            animator.SetBool("isAiming", false);
            elapsedTime = 0f;
            currentState = State.Charging;
        }

    }

    void EnemyCharge()
    {
        animator.SetBool("isCharging", true);

        float chargeDuration = 1.5f;
        float animationSpeed = 1f / chargeDuration;
        animator.speed = animationSpeed;

        // Initialize start position on first frame
        if (elapsedTime == 0f)
        {
            chargingStartPosition = transform.position;
        }

        elapsedTime += Time.deltaTime;
        float progress = elapsedTime / chargeDuration;

        Vector3 moveToTarget = Vector3.Lerp(chargingStartPosition, lastPlayerPosition, progress);
        transform.position = moveToTarget;

        if (elapsedTime >= chargeDuration)
        {
            animator.SetBool("isCharging", false);
            elapsedTime = 0f;
            animator.speed = 1f;
            currentState = State.Landing;
        }
    }

    void EnemyLand()
    {
        animator.SetBool("isLanding", true);
        agent.isStopped = true;
        hitAreaHitbox.enabled = true;

        //if (hitboxReporter.hit)
        //{
        //    hitboxReporter.hit = false;
        //}

        //ANIMATION IS 1.867 SECONDS LONG
        float animationLength = 1.867f;
        elapsedTime += Time.deltaTime;

        ////normalizedTime always outputs 1 on first frame
        if (elapsedTime >= 0.5f)
        {
            hitAreaHitbox.enabled = false;
            hitArea.SetActive(false);
        }



        //Only change states once animation is fully done.
        if (elapsedTime >= animationLength)
        {
            elapsedTime = 0f;
            hitboxReporter.hit = false;
            //if (playerDistance < attackDistance)
            //{
            //    animator.SetBool("isLanding", false);
            //    currentState = State.Attacking;
            //}

            //if (playerDistance < closeRangeDistance && playerDistance > attackDistance)
            //{
            //    animator.SetBool("isLanding", false);
            //    currentState = State.CloseRange;
            //}

            if (playerDistance < aimingDistance)
            {
                animator.SetBool("isLanding", false);
                currentState = State.Aiming;
            }
        }
    }


    //HITBOX AND ANIMATION LOGIC//

    //Hitbox logic called through HitboxReporter
    public void OnPlayerHit(GameObject player)
    {
        //Get enemy's health system and apply damage
        hitboxReporter.hit = true;
        hitAreaHitbox.enabled = false;
        playerHealth.GetComponent<HealthSystem>().TakeDamage(jumpDamage);
    }

    //Called by animation event at start of swing
    //    void PrimaryHitboxOn()
    //    {
    //        primaryHitbox.SetActive(true);
    //    }

    //    //Called animation event at end of swing
    //    void PrimaryHitboxOff()
    //    {
    //        primaryHitbox.SetActive(false);
    //        isAttacking = false;
    //        playerMovement.enabled = true;
    //        hitboxReporter.hit = false;
    //    }

}

