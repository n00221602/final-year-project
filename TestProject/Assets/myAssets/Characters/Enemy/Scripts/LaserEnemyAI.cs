using UnityEngine;
using UnityEngine.AI;

public class LaserEnemyAI : MonoBehaviour
{
    //public Animator animator;
    public Transform player;
    private NavMeshAgent agent;
    public Animator animator;

    public LineRenderer laserLineRenderer;
    private Vector3 laserOffset = new Vector3(0, 1f, 0);
    public enum State
    {
        Idle,
        Aiming,
        Shooting
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
                LaserIdle();
                break;
            case State.Aiming:
                LaserAiming();
                break;
            case State.Shooting:
                LaserShooting();
                break;
        }
    }

    void LaserIdle()
    {
        animator.SetBool("isAiming", false);
        agent.isStopped = true;
        playerDistance = Vector3.Distance(player.position, transform.position);
        laserLineRenderer.enabled = false;
        if (playerDistance < 10f)
        {
            LaserAiming();
        }

    }

    void LaserAiming()
    {
        animator.SetBool("isAiming", true);
        agent.isStopped = true;
        agent.transform.LookAt(player);
        laserLineRenderer.SetPosition(0, transform.position + laserOffset);
        laserLineRenderer.SetPosition(1, player.position + laserOffset);
        laserLineRenderer.enabled = true;

        if (playerDistance < 2f)
        {
            LaserShooting();
        }
    }

    void LaserShooting()
    {
        laserLineRenderer.enabled = false;
        animator.SetBool("isAiming", false);
        animator.SetBool("isShooting", true);
        agent.isStopped = true;
    }
}
