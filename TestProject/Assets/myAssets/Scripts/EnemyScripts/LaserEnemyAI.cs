using UnityEngine;
using UnityEngine.AI;

public class LaserEnemyAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public Animator animator;
    public HealthSystem healthSystem;

    public LineRenderer laserLineRenderer;
    private Vector3 laserOffset = new Vector3(0, 1f, 0);
    public enum State
    {
        Idle,
        Aiming,
        Shooting,
        Cooldown
    }
    public State currentState;
    float playerDistance;

    public float timer;

    private float dodgeWindow = 3.5f;
    private float aimTime = 4f;
    private Vector3 lastPlayerPosition;

    public GameObject laserGun;


    //Add Melee and Range type enemies later on.

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found");
        }

        player = GameObject.Find("Player").transform;
        healthSystem = GameObject.Find("Player UI").GetComponent<HealthSystem>();

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
            case State.Cooldown:
                LaserCooldown();
                break;
        }
    }

    void LaserIdle()
    {
        animator.SetBool("isIdle", true);
        animator.SetBool("isOnCooldown", false);
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
        animator.SetBool("isIdle", false);
        animator.SetBool("isAiming", true);

        Vector3 playerDirection = player.transform.position;
        playerDirection.y = transform.position.y;
        agent.transform.LookAt(playerDirection);

        laserLineRenderer.SetPosition(0, laserGun.transform.position);
        laserLineRenderer.SetPosition(1, player.position + new Vector3(0, laserGun.transform.position.y, 0));
        laserLineRenderer.enabled = true;
        laserLineRenderer.material.color = Color.red;

        Vector3 raycastStart = laserLineRenderer.GetPosition(0);
        Vector3 raycastEnd = laserLineRenderer.GetPosition(1);

        RaycastHit hit;
        if (Physics.Linecast(raycastStart, raycastEnd, out hit))
        {
            //If raycast hits an obstacle, set the laser end point to the hit point
            if (!hit.collider.CompareTag("Player"))
            {
                laserLineRenderer.SetPosition(1, hit.point);
            }


            timer += Time.deltaTime;
            //Debug.Log("Timer: " + timer);
            if (timer >= dodgeWindow)
            {
                if (lastPlayerPosition == Vector3.zero)
                {
                    lastPlayerPosition = player.position;
                }

                agent.transform.LookAt(lastPlayerPosition);
                laserLineRenderer.material.color = Color.orange;
                laserLineRenderer.SetPosition(1, lastPlayerPosition + new Vector3(0, laserGun.transform.position.y, 0));
                laserLineRenderer.enabled = false;

            }
        }
        if (timer >= aimTime)
        {
            timer = 0f;
            currentState = State.Shooting;
        }
    }

    void LaserShooting()
    {
        //Debug.Log("SHOOOOOOTING");
        laserLineRenderer.enabled = true;
        laserLineRenderer.material.color = Color.orange;
        animator.SetBool("isAiming", false);
        animator.SetBool("isShooting", true);

        Vector3 raycastStart = laserLineRenderer.GetPosition(0);
        Vector3 raycastEnd = laserLineRenderer.GetPosition(1);

        timer += Time.deltaTime;

        // Check for collision along the laser line
        RaycastHit hit;
        if (Physics.Linecast(raycastStart, raycastEnd, out hit))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                laserLineRenderer.SetPosition(1, hit.point);
            }
            // Check if player was hit
            if (hit.collider.CompareTag("Player"))
            {
                //Debug.Log("PLAYER DAMAGED");
                healthSystem.TakeDamage(0.1f);
                healthSystem.PlayerGracePeriod(agent);

            }
        }

        //Once time is up, deactivate laser and reset to idle
        if (timer >= 0.8f)
        {
            laserLineRenderer.enabled = false;
            currentState = State.Cooldown;
        }
    }

    void LaserCooldown()
    {
        animator.SetBool("isShooting", false);
        animator.SetBool("isOnCooldown", true);
        timer += Time.deltaTime;

        if (timer >= 3f)
        {
            timer = 0f;
            lastPlayerPosition = Vector3.zero;
            currentState = State.Idle;
        }
    }
}
