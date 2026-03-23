using UnityEngine;

public class ScreenUI : MonoBehaviour
{
    private GameObject playerCam;
    private GameObject[] existingEnemies;

    public RoomEnter roomEnter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCam = GameObject.FindGameObjectWithTag("PlayerCam");
    }

    // Update is called once per frame
    void Update()
    {
        //if (roomEnter.spawnEnemies)
        //{
        //    GetEnemyUI();
        //}
        GetEnemyUI();
    }

    void GetEnemyUI()
    {
        existingEnemies = GameObject.FindGameObjectsWithTag("EnemyUI");
        foreach (GameObject enemyUI in existingEnemies)
        {
            enemyUI.transform.LookAt(playerCam.transform);
        }
    }
}
