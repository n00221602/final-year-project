using System.Collections.Generic;
using UnityEngine;

public class RoomEnter : MonoBehaviour
{
    //TO DO - call layoutGen script to spawn based on layout. enemies only spawn on floors using a random function. If floor near a door or outlet then reroll.
    public LayoutGen layoutGen;
    public FloorCreator floorCreator;
    public RoomGen roomGen;
    public GameObject enemy;
    public GameObject[] doors;

    [HideInInspector] public int[,] activeRoom;
    [HideInInspector] public int rows;
    [HideInInspector] public int cols;
    public GameObject activeRoomParent;

    List<Vector3> spawnPointList;
    List<Vector3> usedSpawnPointList;
    public int enemyCount;
    int randomIndex;

    bool isTriggered = false;
    bool spawnEnemies = false;

    bool top;
    bool bottom;
    bool left;
    bool right;
    bool enemyClose;

    //int doorTarget = 4;
    //int outputTarget = 5;

    Vector3 spawnPoint;
    Vector3 noDif = new Vector3(0, 0, 0);
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
            Debug.Log("ACTIVE TRIGGER: " + activeTrigger);
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
        //Solutions - go through each layout and check if 5 is equal to outputPos.

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
                        activeRoomParent = roomGen.roomParent[i];
                        LoadEnemy();
                        return;
                    }
                }
            }
        }
    }

    void LoadEnemy()
    {
        float timer = 1f;
        Invoke(nameof(SpawnEnemy), timer);
    }

    void SpawnEnemy()
    {
        spawnEnemies = true;
        //Count all the 3s in the layout and add them into an array. Then randomly select one of the 3s and spawn an enemy there.
        rows = activeRoom.GetLength(0);
        cols = activeRoom.GetLength(1);

        //Initialize the spawn point list and used spawn point list.
        spawnPointList = new List<Vector3>();
        usedSpawnPointList = new List<Vector3>();

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (activeRoom[y, x] == 3)
                {
                    spawnPointList.Add(new Vector3(x, 0, -y));
                }
            }
        }
        //Debug.Log("SPAWN POINT LENGTH:" + spawnPointList.Count);

        //Pick a random postition from the list and spawn enemies there. If a spot is near a door or outlet then reroll.
        for (int i = 0; i < enemyCount; i++)
        {
            Debug.Log("CURRENT ITERATION: " + i);
            randomIndex = Random.Range(0, spawnPointList.Count);

            //Since spawnPoint is relative to the room parent, we need to add the room parent's position from the spawn point to get the world position.
            spawnPoint = spawnPointList[randomIndex] + activeRoomParent.transform.position;




            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    enemyClose = enemy.transform.position - spawnPoint == noDif;


                    if (x == spawnPoint.x || y == -spawnPoint.z || enemy)
                    {
                        //If the spawn point is near a door or outlet, or if an enemy is already there, then reroll by breaking out of the loop and starting the next iteration of the for loop.
                        randomIndex = Random.Range(0, spawnPointList.Count);
                        spawnPoint = spawnPointList[randomIndex] + activeRoomParent.transform.position;
                    }
                    else
                    {
                        //Once a spawn point is chosen, remove it from the spawn point list and add it to the used list.
                        spawnPointList.RemoveAt(randomIndex);
                        usedSpawnPointList.Add(spawnPoint);
                        break;
                    }
                }
            }

            Instantiate(enemy, spawnPoint, Quaternion.identity, activeRoomParent.transform);

            //Debug.Log("SPAWN POINT: " + spawnPointList[randomIndex]);
            //Debug.Log("PARENT POS: " + activeRoomParent.transform.position);
            //Debug.Log("New SPAWN POINT: " + (spawnPointList[randomIndex] - activeRoomParent.transform.position));

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

