using System.Collections.Generic;
using UnityEngine;

public class RoomEnter : MonoBehaviour
{
    //TO DO - call layoutGen script to spawn based on layout. enemies only spawn on floors using a random function. If floor near a door or outlet then reroll.
    public LayoutGen layoutGen;
    public FloorCreator floorCreator;
    public RoomGen roomGen;
    public GameObject enemy;
    //Vector3 outputPos;

    [HideInInspector] public int[,] activeRoom;
    [HideInInspector] public int rows;
    [HideInInspector] public int cols;
    public GameObject activeRoomParent;

    List<Vector3> spawnPointList;
    public int enemyCount;
    int randomIndex;

    bool isTriggered = false;
    void Start()
    {

    }
    private void OnTriggerEnter(Collider collider)
    {
        //Check if player collides with the spawn point, if so spawn an enemy
        if (collider.gameObject.CompareTag("RoomTrigger"))
        {
            isTriggered = true;
            collider.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (isTriggered)
        {
            Debug.Log("COLLIDE");

            RoomActive();
            isTriggered = false;
        }
    }


    void RoomActive()
    {
        //Solutions - go through each layout and check if 5 is equal to outputPos.
        // Debug array contents

        for (int i = 0; i < roomGen.layoutList.Count; i++)
        {
            //If chosen layout has an output position that matches the current room's output position, spawn the next room and enemies.
            //if (roomGen.layoutList[i]
            //if (roomGen.layoutList[i][(int)-floorCreator.outputPosArray[i].z, (int)floorCreator.outputPosArray[i].x] == 5)
            //{
            //    Debug.Log("OUTPUT POSITION MATCHED");
            //    //Spawn next room and enemies.
            //    SpawnEnemy();
            //}

            for (int j = 0; j < floorCreator.outputPosArray.Length; j++)
            {
                int yIndex = (int)-floorCreator.outputPosArray[j].z;
                int xIndex = (int)floorCreator.outputPosArray[j].x;

                //Debug.Log($"Checking layout {i} for output position at ({yIndex}, {xIndex})");

                //Checks if the indexs of the current output position are within the current layout's bounds.
                if (yIndex >= 0 && yIndex < roomGen.layoutList[i].GetLength(0) && xIndex >= 0 && xIndex < roomGen.layoutList[i].GetLength(1))
                {
                    if (roomGen.layoutList[i][yIndex, xIndex] == 5)
                    {

                        activeRoom = roomGen.layoutList[i];
                        activeRoomParent = roomGen.roomParent[i];
                        //Debug.Log(activeRoom);
                        //Spawn next room and enemies.
                        SpawnEnemy();
                        return;
                    }
                }
            }
        }

        void SpawnEnemy()
        {
            Debug.Log("ENEMY SPAWNING");
            //Count all the 3s in the layout and add them into an array. Then randomly select one of the 3s and spawn an enemy there.
            rows = activeRoom.GetLength(0);
            cols = activeRoom.GetLength(1);

            spawnPointList = new List<Vector3>();

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

            //Pick random spots from the array and spawn enemies there. If a spot is near a door or outlet then reroll.
            for (int i = 0; i < 1; i++)
            {
                Debug.Log("CURRENT ITERATION: " + i);
                randomIndex = Random.Range(0, spawnPointList.Count);

                //Since spawnPoint is relative to the room parent, we need to add the room parent's position from the spawn point to get the world position.
                Vector3 spawnPoint = spawnPointList[randomIndex] + activeRoomParent.transform.position;
                Debug.Log("SPAWN POINT: " + spawnPointList[randomIndex]);
                Debug.Log("PARENT POS: " + activeRoomParent.transform.position);
                Debug.Log("New SPAWN POINT: " + (spawnPointList[randomIndex] - activeRoomParent.transform.position));
                Instantiate(enemy, spawnPoint, Quaternion.identity, activeRoomParent.transform);
            }

        }
    }
}
