using UnityEngine;

public class HandleTeleport : MonoBehaviour
{
    public RoomGen roomGen;

    void Start()
    {
        if (roomGen == null)
        {
            roomGen = GameObject.FindGameObjectWithTag("ScriptCube").GetComponent<RoomGen>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("PLAYER TELEPORT");
            //Run reroll.
            roomGen.OnFloorComplete.Invoke();

            //Add a delay. Such as loading screen.

            //Player gets teleported back to the starting room position.
            other.gameObject.transform.position = new Vector3(4, 0, -4);
        }
    }
}

