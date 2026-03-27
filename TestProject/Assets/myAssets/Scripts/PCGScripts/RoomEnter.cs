using System.Collections.Generic;
using UnityEngine;

public class RoomEnter : MonoBehaviour
{
    //TO DO - call layoutGen script to spawn based on layout. enemies only spawn on floors using a random function. If floor near a door or outlet then reroll.
    public LayoutGen layoutGen;
    public FloorCreator floorCreator;
    public RoomGen roomGen;
    public GameObject enemy;
    [HideInInspector] public GameObject[] doors;

    [HideInInspector] public int[,] activeRoom;
    [HideInInspector] public int rows;
    [HideInInspector] public int cols;
    [HideInInspector] public GameObject activeRoomParent;
    [HideInInspector] public int activeRoomIndex;

    List<Vector3> spawnPointList;
    List<Vector3> usedSpawnPointList;
    public int enemyCount;
    int randomIndex;

    bool isTriggered = false;
    [HideInInspector] public bool spawnEnemies = false;

    Vector3 spawnPoint;
    Vector3 activeTrigger;
    void Awake()
    {
        roomGen.OnRoomGenComplete.AddListener(FindDoors);
    }

    void FindDoors()
    {
        doors = GameObject.FindGameObjectsWithTag("Door");
        Debug.Log("DOOR LENGTH: " + doors.Length);

    }

    private void OnTriggerEnter(Collider collider)
    {
        //If a player makes contact with the room trigger, set bool to true and disable trigger. A bool is used to avoid multiple calls from the trigger.
        if (collider.gameObject.CompareTag("RoomTrigger"))
        {
            activeTrigger = collider.gameObject.transform.localPosition;

            //Get all doors and close them. This is to prevent the player from leaving the room before the enemies are spawned.
            foreach (GameObject door in doors)
            {
                door.transform.position = new Vector3(door.transform.position.x, 3f, door.transform.position.z);
            }

            isTriggered = true;
            collider.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        //if the trigger bool is true, call the next function.
        if (isTriggered)
        {
            Debug.Log("COLLIDE");

            RoomActive();
            isTriggered = false;
        }

        //Equaled to 2 for now, since 2 existing test enemies are in the scene.
        if (spawnEnemies && GameObject.FindGameObjectsWithTag("Enemy").Length == 2)
        {
            spawnEnemies = false;
            RoomCleared();
        }
    }


    //This function loops through each layout and checks if the output position of the current room matches with any of the output positions in the layouts
    void RoomActive()
    {
        //This function loops through each layout and checks if the output position of the current room matches with any of the output positions in the layouts.
        for (int i = 0; i < roomGen.layoutList.Count; i++)
        {
            //Loop through each output position to check which one matches the current layout's output position. OutputPosArray containts the world space postions of the outputs (5) for each layout.
            for (int j = 0; j < floorCreator.outputPosArray.Length; j++)
            {
                //The output position is a vector3, so we need to convert them back to their original [y,x] format to cross reference the indexes.
                int yIndex = (int)-floorCreator.outputPosArray[j].z;
                int xIndex = (int)floorCreator.outputPosArray[j].x;
                Debug.Log(floorCreator.outputPosArray[j] + "<-OUTPUT POS ARRAY INDEX " + j);

                //Checks if the indexs of the current output position are within the current layout's bounds.
                if (yIndex >= 0 && yIndex < roomGen.layoutList[i].GetLength(0) && xIndex >= 0 && xIndex < roomGen.layoutList[i].GetLength(1))
                {
                    //If the current layout index matches the output position, then the active room has been found. This layout and its parent is set to the active room.
                    if (roomGen.layoutList[i][yIndex, xIndex] == 5 && floorCreator.outputPosArray[j] == activeTrigger)
                    {
                        Debug.Log("OUTPUT FOUND AT: " + floorCreator.outputPosArray[j]);
                        activeRoom = roomGen.layoutList[i];
                        activeRoomIndex = i;
                        activeRoomParent = roomGen.roomParent[i];
                        LoadEnemy();
                        return;
                    }
                }
            }
        }
    }

    //Spawn enemys after a short delay.
    void LoadEnemy()
    {
        float timer = 1f;
        Invoke(nameof(SpawnEnemy), timer);
    }

    void SpawnEnemy()
    {
        spawnEnemies = true;

        //Get all floor positions using position lookup instead of nested loops
        List<Vector2Int> floorPositions = roomGen.GetTilePositions(activeRoomIndex, 3); // 3 = FLOOR

        //Initialize the spawn point list and used spawn point list.
        spawnPointList = new List<Vector3>();
        usedSpawnPointList = new List<Vector3>();

        //Convert floor positions to world space
        foreach (Vector2Int floorPos in floorPositions)
        {
            Vector3 worldSpawnPoint = new Vector3(floorPos.x, 0, -floorPos.y) + activeRoomParent.transform.position;
            spawnPointList.Add(worldSpawnPoint);
        }

        Debug.Log("SPAWN POINT LENGTH:" + spawnPointList.Count);

        //Pick a random position from the list and spawn enemies there.
        for (int i = 0; i < enemyCount; i++)
        {
            Debug.Log("CURRENT ITERATION: " + i);
            randomIndex = Random.Range(0, spawnPointList.Count);

            spawnPoint = spawnPointList[randomIndex];

            //Once a spawn point is chosen, remove it from the spawn point list and add it to the used list.
            spawnPointList.RemoveAt(randomIndex);
            usedSpawnPointList.Add(spawnPoint);

            Instantiate(enemy, spawnPoint, Quaternion.identity, activeRoomParent.transform);
        }
    }


    void RoomCleared()
    {
        activeRoom = null;
        activeRoomParent = null;

        foreach (GameObject door in doors)
        {
            door.transform.position = new Vector3(door.transform.position.x, -3f, door.transform.position.z);
        }

        Debug.Log("ROOM CLEARED");
    }
}

