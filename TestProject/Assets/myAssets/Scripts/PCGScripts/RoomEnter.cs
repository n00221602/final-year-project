using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomEnter : MonoBehaviour
{
    //TO DO - call layoutGen script to spawn based on layout. enemies only spawn on floors using a random function. If floor near a door or outlet then reroll.
    public LayoutGen layoutGen;
    public FloorCreator floorCreator;
    public RoomGen roomGen;
    private GameObject enemy;
    public GameObject[] enemyArray;
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
    bool jumperSpawned = false;

    bool isTriggered = false;
    [HideInInspector] public bool spawnEnemies = false;

    Vector3 spawnPoint;
    Vector3 activeTrigger;
    [HideInInspector] public List<int> usedRoomIndices;

    void Awake()
    {
        roomGen.OnRoomGenComplete.AddListener(FindDoors);
    }

    void FindDoors()
    {
        GameObject[] allDoors = GameObject.FindGameObjectsWithTag("Door");

        // Filter out any null/destroyed doors
        List<GameObject> validDoors = new List<GameObject>();
        foreach (GameObject door in allDoors)
        {
            if (door != null)
            {
                validDoors.Add(door);
            }
        }

        doors = validDoors.ToArray();
        Debug.Log("DOORS FOUND:" + doors.Length);
    }


    private void OnTriggerEnter(Collider collider)
    {
        //If a player makes contact with the room trigger, set bool to true and disable trigger. A bool is used to avoid multiple calls from the trigger.
        if (collider.gameObject.CompareTag("RoomTrigger"))
        {
            activeTrigger = collider.gameObject.transform.localPosition;

            //Get all doors and close them. This is to prevent the player from leaving the room before the enemies are spawned.
            if (doors != null && doors.Length > 0)
            {
                foreach (GameObject door in doors)
                {
                    if (door != null)  // Check if door still exists
                    {
                        door.transform.position = new Vector3(door.transform.position.x, 3f, door.transform.position.z);
                    }
                }
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
        if (spawnEnemies && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
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
            if (usedRoomIndices.Contains(i))
                continue;
            //Loop through each output position to check which one matches the current layout's output position. OutputPosArray containts the world space postions of the outputs (5) for each layout.
            for (int j = 0; j < floorCreator.outputPosArray.Length; j++)
            {
                //The output position is a vector3, so we need to convert them back to their original [y,x] format to cross reference the indexes.
                int yIndex = (int)-floorCreator.outputPosArray[j].z;
                int xIndex = (int)floorCreator.outputPosArray[j].x;

                //Checks if the indexs of the current output position are within the current layout's bounds.
                if (yIndex >= 0 && yIndex < roomGen.layoutList[i].GetLength(0) && xIndex >= 0 && xIndex < roomGen.layoutList[i].GetLength(1))
                {
                    //If the current layout index matches the trigger's position, then the active room has been found.
                    if (roomGen.layoutList[i][yIndex, xIndex] == 5 && floorCreator.outputPosArray[j] == activeTrigger)
                    {
                        activeRoom = roomGen.layoutList[i];
                        activeRoomIndex = i;
                        activeRoomParent = roomGen.roomParent[i];
                        usedRoomIndices.Add(i);
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
        float timer = 1.5f;
        Invoke(nameof(SpawnEnemy), timer);
    }

    void SpawnEnemy()
    {
        List<Vector2Int> teleporterPosition = roomGen.GetTilePositions(activeRoomIndex, 7);

        if (teleporterPosition.Count > 0)
        {
            return;
        }
        else
        {
            spawnEnemies = true;
        }
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

        //Pick a random position from the list and spawn enemies there.
        for (int i = 0; i < enemyCount; i++)
        {
            randomIndex = Random.Range(0, spawnPointList.Count);
            float value = Random.value;

            if (value > 0.66f && !jumperSpawned)
            {
                enemy = enemyArray[1];
                jumperSpawned = true;
            }
            else
            {
                enemy = enemyArray[0];
            }

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
        jumperSpawned = false;

        if (doors != null && doors.Length > 0)
        {
            foreach (GameObject door in doors)
            {
                if (door != null)
                {
                    door.transform.position = new Vector3(door.transform.position.x, -3f, door.transform.position.z);
                }
            }
        }

        Debug.Log("ROOM CLEARED");
    }
}

