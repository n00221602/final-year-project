using UnityEngine;

public class RoomEnter : MonoBehaviour
{
    //TO DO - call layoutGen script to spawn based on layout. enemies only spawn on floors using a random function. If floor near a door or outlet then reroll.
    public LayoutGen layoutGen;
    public FloorCreator floorCreator;
    public RoomGen roomGen;
    Vector3 outputPos;
    void Start()
    {

    }
    private void OnTriggerEnter(Collider collider)
    {
        //Check if player collides with the spawn point, if so spawn an enemy
        if (collider.gameObject.CompareTag("RoomTrigger"))
        {

            //outputPos = collider.transform.position;
            //Debug.Log(outputPos);
            RoomActive();
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

                // Bounds check for the 2D array
                if (yIndex >= 0 && yIndex < roomGen.layoutList[j].GetLength(0) && xIndex >= 0 && xIndex < roomGen.layoutList[j].GetLength(1))
                {
                    if (roomGen.layoutList[j][yIndex, xIndex] == 5)
                    {
                        Debug.Log($"OUTPUT POSITION MATCHED at index {i}");
                        //Spawn next room and enemies.
                        SpawnEnemy();
                    }
                }
            }
        }
    }

    void SpawnEnemy()
    {
        Debug.Log("ENEMY SPAWNING");

    }
}
