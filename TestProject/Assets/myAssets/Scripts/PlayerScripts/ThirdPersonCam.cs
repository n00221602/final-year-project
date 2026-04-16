using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;

    public float rotationSpeed;

    public CombatSystem combatSystem;
    //public HealthSystem healthSystem;
    //public RoomEnter roomEnter;

    //private GameObject healthBar;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //Rotate the player orientation to face the camera's forward direction.
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //The direction that the player is facing will only change is there is an input and that the player is not attacking.
        if (inputDir != Vector3.zero && !combatSystem.isAttacking)
        {
            //player.forward = inputDir.normalized;
            player.transform.rotation = Quaternion.LookRotation(inputDir);
        }

    }
}
