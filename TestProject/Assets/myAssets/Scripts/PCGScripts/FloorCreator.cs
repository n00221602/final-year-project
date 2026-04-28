using UnityEngine;

public class FloorCreator : MonoBehaviour
{
    public LayoutGen roomLayout;
    public RoomGen roomGen;

    Vector3 entryPosition;
    Vector3 exitPosition;

    public Vector3[] outputPosArray;

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

            Vector2Int? doorPos = roomGen.FindTilePosition(i, 4);
            if (!doorPos.HasValue)
                continue;

            entryPosition = new Vector3(doorPos.Value.x, 0, -doorPos.Value.y);

            Vector2Int? outputPos = roomGen.FindTilePosition(i + 1, 5);
            if (!outputPos.HasValue)
                continue;

            exitPosition = new Vector3(outputPos.Value.x, 0, -outputPos.Value.y);
            outputPosArray[i] = exitPosition;

            //Subtract the '4' and '5' positions to find the difference. The offset is added so that rooms aren't overlapping.
            Vector3 positionDifference = (entryPosition - exitPosition) + new Vector3(-1, 0, 0);

            if (positionDifference != Vector3.zero)
            {
                //Move the next room by the position difference AND the current room's position.
                roomGen.roomParent[i + 1].transform.position = positionDifference + roomGen.roomParent[i].transform.position;
            }
        }
    }
}