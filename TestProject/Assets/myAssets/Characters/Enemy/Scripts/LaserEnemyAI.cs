using UnityEngine;
using UnityEngine.AI;

public class LaserEnemyAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public Animator animator;
    public HealthSystem playerHealth;

    public LineRenderer laserLineRenderer;
    private Vector3 laserOffset = new Vector3(0, 1f, 0);
    public enum State
    {
        Idle,
        Aiming,
        Shooting
    }
    public State currentState;
    float playerDistance;

    public float timer;

    private float dodgeWindow = 3.5f;
    private float aimTime = 4f;
    private Vector3 lastPlayerPosition;


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
        switch (currentState)
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
            currentState = State.Aiming;
        }

    }

    void LaserAiming()
    {
        animator.SetBool("isAiming", true);
        agent.transform.LookAt(player);
        laserLineRenderer.SetPosition(0, transform.position + laserOffset);
        laserLineRenderer.SetPosition(1, player.position + laserOffset);
        laserLineRenderer.enabled = true;

        timer += Time.deltaTime;
        Debug.Log("Timer: " + timer);
        if (timer >= dodgeWindow)
        {
            if (lastPlayerPosition == Vector3.zero) // Only capture once
            {
                lastPlayerPosition = player.position;
            }

            agent.transform.LookAt(lastPlayerPosition);
            laserLineRenderer.SetPosition(1, lastPlayerPosition + laserOffset);

        }
        if (timer >= aimTime)
        {
            timer = 0f;
            currentState = State.Shooting;
        }
    }

    void LaserShooting()
    {
        Debug.Log("SHOOOOOOTING");
        laserLineRenderer.material.color = Color.orange;
        animator.SetBool("isAiming", false);
        animator.SetBool("isShooting", true);

        Vector3 raycastStart = laserLineRenderer.GetPosition(0);
        Vector3 raycastEnd = laserLineRenderer.GetPosition(1);

        // Check for collision along the laser line
        RaycastHit hit;
        if (Physics.Linecast(raycastStart, raycastEnd, out hit))
        {
            // Check if player was hit
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("PLAYER DAMAGED");
                playerHealth.TakeDamage(0.1f); // Adjust damage value as needed

            }
        }
    }
}
