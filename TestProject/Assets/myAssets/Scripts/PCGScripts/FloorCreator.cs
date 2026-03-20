using UnityEngine;

public class FloorCreator : MonoBehaviour
{
    public LayoutGen roomLayout;
    public RoomGen roomGen;

    Vector3 entryPosition;
    Vector3 exitPosition;

    public Vector3[] outputPosArray;

    bool top;
    bool bottom;
    bool left;
    bool right;

    int floorTarget = 3;

    Quaternion currentDoorRotation = Quaternion.Euler(0, 0, 0);
    Quaternion roomRotation = Quaternion.Euler(0, 0, 0);

    void Awake()
    {
        //Once the room generation is complete, CreateFloorLayout is called to reposition the rooms accordingly.
        roomGen.OnRoomGenComplete.AddListener(CreateFloorLayout);
    }

    void CreateFloorLayout()
    {
        // Initialize the array with the correct size
        outputPosArray = new Vector3[roomGen.roomParent.Length];

        for (int i = 0; i < roomGen.roomParent.Length - 1; i++)
        {
            if (roomGen.roomParent[i] == null || roomGen.roomParent[i + 1] == null)
            {
                return;
            }

            //Find input position using position lookup - O(1) instead of O(n²)
            Vector2Int? doorPos = roomGen.FindTilePosition(i, 4); // 4 = DOOR
            if (!doorPos.HasValue)
                continue;

            entryPosition = new Vector3(doorPos.Value.x, 0, -doorPos.Value.y);
            currentDoorRotation = roomGen.roomParent[i].transform.rotation;

            //Find output location using position lookup - O(1) instead of O(n²)
            Vector2Int? outputPos = roomGen.FindTilePosition(i + 1, 5); // 5 = OUTPUT
            if (!outputPos.HasValue)
                continue;

            exitPosition = new Vector3(outputPos.Value.x, 0, -outputPos.Value.y);
            outputPosArray[i] = exitPosition;

            //Subtract the '4' and '5' positions to find the difference.
            Vector3 positionDifference = entryPosition - exitPosition;

            //ROTATION//

            //Set x and y values to the door position
            int entryX = doorPos.Value.x;
            int entryY = doorPos.Value.y;

            //Check for neighbouring floor tiles around the door using direct array access
            top = (entryY > 0) && (roomGen.layoutList[i][entryY - 1, entryX] == floorTarget);
            bottom = (entryY < roomGen.rowsArray[i] - 1) && (roomGen.layoutList[i][entryY + 1, entryX] == floorTarget);
            left = (entryX > 0) && (roomGen.layoutList[i][entryY, entryX - 1] == floorTarget);
            right = (entryX < roomGen.colsArray[i] - 1) && (roomGen.layoutList[i][entryY, entryX + 1] == floorTarget);

            //Determine room rotation based on door orientation
            if (top || bottom)
            {
                roomRotation = Quaternion.Euler(0, 180, 0);
            }
            else if (left || right)
            {
                roomRotation = Quaternion.Euler(0, 180, 0);
            }

            if (positionDifference != Vector3.zero)
            {
                //Move the next room by the position difference AND the current room's position.
                roomGen.roomParent[i + 1].transform.position = positionDifference + roomGen.roomParent[i].transform.position;
                //roomGen.roomParent[i + 1].transform.rotation = roomRotation;
            }
        }
    }
}