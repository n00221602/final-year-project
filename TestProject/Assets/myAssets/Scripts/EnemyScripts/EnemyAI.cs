using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //public Animator animator;
    //public Transform player;

    //TODO - change bool checks to run once between switching states instead of all at once for each function.
    //Fix timing for the enemy charge.

    public GameObject player;
    private NavMeshAgent agent;
    public Animator animator;
    public GameObject hitArea;
    public HitboxReporter hitboxReporter;

    public Collider hitAreaCollider;

    public HealthSystem playerHealth;

    float jumpDamage = 0.34f;

    AnimatorStateInfo stateInfo;
    private Vector3 chargingStartPosition;

    public enum State
    {
        Idle,
        CloseRange,
        Attacking,
        LongRange,
        Charging,
        Landing
    }

    private State currentState;
    float playerDistance;

    float closeRangeDistance = 5f;
    float longRangeDistance = 10f;
    float attackDistance = 1f;
    float elapsedTime;

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

        Collider hitareaHitbox = hitArea.GetComponent<Collider>();
        hitareaHitbox.enabled = false;

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
            case State.CloseRange:
                EnemyCloseRange();
                break;
            case State.Attacking:
                EnemyAttack();
                break;
            case State.LongRange:
                EnemyLongRangeAim();
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
        animator.SetBool("isCloseRange", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isLongRangeAim", false);
        animator.SetBool("isCharging", false);
        animator.SetBool("isLanding", false);

        Debug.Log("IDLE");
        //animator.SetBool("isCloseRange", false);
        agent.isStopped = true;
        agent.ResetPath();

        //Change state based on player distance.
        //Close Range
        if (playerDistance < closeRangeDistance)
        {
            currentState = State.CloseRange;
        }
        //If player is between close and long range distance, change to LongRange state
        else if (playerDistance > closeRangeDistance && playerDistance < longRangeDistance)
        {
            currentState = State.LongRange;
        }
    }


    //This state checks if the player is within its close range only it's in the enemy view.
    void EnemyCloseRange()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isCloseRange", true);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isLongRangeAim", false);
        animator.SetBool("isCharging", false);
        animator.SetBool("isLanding", false);
        agent.isStopped = false;
        agent.transform.LookAt(player.transform);
        agent.SetDestination(player.transform.position);
        Debug.Log("CLOSE RANGE");

        if (playerDistance < attackDistance)
        {
            currentState = State.Attacking;
        }

        if (playerDistance > closeRangeDistance && playerDistance < longRangeDistance)
        {
            currentState = State.LongRange;
        }
    }

    void EnemyAttack()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isCloseRange", false);
        animator.SetBool("isAttacking", true);
        animator.SetBool("isLongRangeAim", false);
        animator.SetBool("isCharging", false);
        animator.SetBool("isLanding", false);

        agent.isStopped = true;
        agent.ResetPath();
        Debug.Log("ATTACKING");

        //In this state, the enemy should run its attack animation and then a cooldown once animation is complete. once cooldown is up,
        //change state depending on distance. Attack again if player still within range.


        //3 if statements: for idle, close and long range.

        if (playerDistance < closeRangeDistance && playerDistance > attackDistance)
        {
            currentState = State.CloseRange;
        }

        if (playerDistance > closeRangeDistance && playerDistance < longRangeDistance)
        {
            currentState = State.LongRange;
        }

        //if (playerDistance > longRangeDistance)
        //{
        //    currentState = State.Idle;
        //}

    }


    //This state checks if the player is within its long range only it's in the enemy view.
    void EnemyLongRangeAim()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isCloseRange", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isLongRangeAim", true);
        animator.SetBool("isCharging", false);
        animator.SetBool("isLanding", false);
        agent.ResetPath();
        Debug.Log("AIMING");

        elapsedTime += Time.deltaTime;
        Vector3 playerDirection = player.transform.position;
        playerDirection.y = transform.position.y;
        agent.transform.LookAt(playerDirection);

        hitArea.SetActive(true);
        hitArea.transform.position = new Vector3(player.transform.position.x, hitArea.transform.position.y, player.transform.position.z);

        if (elapsedTime > 3f)
        {
            lastPlayerPosition = player.transform.position;
            agent.transform.LookAt(lastPlayerPosition);
            hitArea.transform.position = new Vector3(lastPlayerPosition.x, hitArea.transform.position.y, lastPlayerPosition.z);
        }

        if (elapsedTime > 4f)
        {
            elapsedTime = 0f;
            currentState = State.Charging;
        }

    }
    void EnemyCharge()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isCloseRange", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isLongRangeAim", false);
        animator.SetBool("isCharging", true);
        animator.SetBool("isLanding", false);
        Debug.Log("CHARGING");

        //float chargeDuration = 1.5f;
        //float animationSpeed = 1f / chargeDuration;
        //animator.speed = animationSpeed;

        //start position is only set once.
        if (chargingStartPosition == Vector3.zero)
        {
            chargingStartPosition = transform.position;

        }
        //elapsedTime += Time.deltaTime;
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationProgress = stateInfo.normalizedTime;

        Vector3 moveToTarget = Vector3.Lerp(chargingStartPosition, lastPlayerPosition, animationProgress);
        transform.position = moveToTarget;

        //if (stateInfo.normalizedTime >= 1f)
        //{
        //    elapsedTime = 0f;
        //    //animationSpeed = 1f;
        //    currentState = State.Landing;
        //    chargingStartPosition = Vector3.zero;
        //}
    }

    void EnemyLand()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isCloseRange", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isLongRangeAim", false);
        animator.SetBool("isCharging", false);
        animator.SetBool("isLanding", true);
        Debug.Log("LANDING");

        hitAreaCollider.enabled = true;

        //if (stateInfo.normalizedTime == 1f)
        //{
        //    hitArea.SetActive(false) when land animation is over.
        //}


        if (playerDistance < attackDistance)
        {
            hitAreaCollider.enabled = false;
            //hitArea.SetActive(false);
            currentState = State.Attacking;
        }

        if (playerDistance < closeRangeDistance && playerDistance > attackDistance)
        {
            hitAreaCollider.enabled = false;
            //hitArea.SetActive(false);
            currentState = State.CloseRange;
        }

        if (playerDistance > closeRangeDistance && playerDistance < longRangeDistance)
        {
            hitAreaCollider.enabled = false;
            //hitArea.SetActive(false);
            currentState = State.LongRange;
        }

        //if (playerDistance > longRangeDistance)
        //{
        //    hitAreaCollider.enabled = false;
        //    //hitArea.SetActive(false);
        //    currentState = State.Idle;
        //}
    }


    //HITBOX AND ANIMATION LOGIC//

    //Hitbox logic called through HitboxReporter
    public void OnPlayerHit(GameObject player)
    {
        //Get enemy's health system and apply damage
        hitboxReporter.hit = true;
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

