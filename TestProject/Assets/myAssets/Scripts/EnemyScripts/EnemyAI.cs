using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //public Animator animator;
    public Transform player;
    private NavMeshAgent agent;
    public Animator animator;
    public enum State
    {
        Idle,
        Moving,
        Attacking
    }

    public State state;
    float playerDistance;

    //Add Melee and Range type enemies later on.

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found");
        }

    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                EnemyIdle();
                break;
            case State.Moving:
                EnemyMoving();
                break;
            case State.Attacking:
                EnemyAttack();
                break;
        }
    }

    void EnemyIdle()
    {
        animator.SetBool("isMoving", false);
        agent.isStopped = true;
        playerDistance = Vector3.Distance(player.position, transform.position);
        if (playerDistance < 10f)
        {
            EnemyMoving();
        }

    }

    void EnemyMoving()
    {
        animator.SetBool("isMoving", true);
        agent.isStopped = false;
        agent.transform.LookAt(player);
        agent.SetDestination(player.position);

        if (playerDistance < 2f)
        {
            EnemyAttack();
        }

        if (playerDistance > 10f)
        {
            Debug.Log("Player out of range");
            EnemyIdle();
        }
    }

    void EnemyAttack()
    {
        animator.SetBool("isMoving", false);
        agent.isStopped = true;
        if (playerDistance > 2f)
        {
            EnemyMoving();
        }
    }
}
