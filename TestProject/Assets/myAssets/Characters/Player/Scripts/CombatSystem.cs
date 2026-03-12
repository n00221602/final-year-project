using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    //TO-DO
    //Handle inputs. Primary (right hand) and secondary (left hand) attacks.
    //Handle combat animations. 
    //For hitboxes, enable the hitbox depending on the animation frame.

    public Animator animator;
    public PlayerMovement player;
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerMovement>();

        //Left click for primary attack, right click for secondary attack.
        Input.GetKeyDown(KeyCode.Mouse0);
        Input.GetKeyUp(KeyCode.Mouse1);
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
    }

    void PrimaryAttack()
    {
        Debug.Log("Primary Attack");
        animator.SetTrigger("AttackP");
    }

    void SecondaryAttack()
    {
        Debug.Log("Secondary Attack");
        animator.SetTrigger("AttackS");
    }
}
