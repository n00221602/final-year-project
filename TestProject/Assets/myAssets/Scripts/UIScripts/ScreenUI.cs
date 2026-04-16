using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class ScreenUI : MonoBehaviour
{
    private GameObject playerCam;
    private GameObject[] existingEnemies;
    public GameObject gameOverUI;

    [SerializeField] private UniversalRendererData rendererData;

    //public RoomEnter roomEnter;
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

    public void PlayerRetry()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayerQuit()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
}
