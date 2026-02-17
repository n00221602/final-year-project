using UnityEngine;

public class FloorCreator : MonoBehaviour
{

    public LayoutGen roomLayout;
    public RoomGen roomGen;

    //Reassignable variable used when changing the position of a room.
    GameObject roomParent;

    Vector3 entryPosition;
    Vector3 exitPosition;

    void Awake()
    {
        //Once the room generation is complete, CreateFloorLayout is called to reposition the rooms accordingly.
        roomGen.OnRoomGenComplete.AddListener(CreateFloorLayout);
    }
    void CreateFloorLayout()
    {

        if (roomGen.roomParent2 == null)
        {
            Debug.LogError("Room parent not assigned");
            return;
        }

        if (roomGen.roomParent2 != null)
        {
            Debug.LogError("Room parent found");
            roomParent = roomGen.roomParent2;

            //Find input position (door)
            for (int y = 0; y < roomGen.rows1; y++)
            {
                for (int x = 0; x < roomGen.cols1; x++)
                {
                    if (roomGen.layout1[y, x] == 4)
                    {
                        Debug.Log("door found at" + (x, y));
                        entryPosition = new Vector3(x, 0, -y);
                        break;
                    }
                }
            }

            //Find output location. Output is set to 5 in the layoutGen script.
            for (int y = 0; y < roomGen.rows2; y++)
            {
                for (int x = 0; x < roomGen.cols2; x++)
                {
                    if (roomGen.layout2[y, x] == 5)
                    {

                        //Move room2 so 5 matches with the door in room1
                        Debug.Log("exit found at" + (x, y));
                        exitPosition = new Vector3(x, 0, -y);
                        break;
                    }
                }
            }

            Vector3 positionDifference = entryPosition - exitPosition;
            Debug.Log("position difference is" + positionDifference);

            if (positionDifference != null)
            {
                roomParent.transform.position = positionDifference;
            }

        }
    }
}

//NOTE: this script needs to find the input from a previous room and match it with the ouput of the next room. Rooms can be decided in an array.
//Every room has an input and output EXCEPT for the starting room, which only has an input.
//input-room[0] -> output-room[1] | input-room[1] -> output-room[2] | input-room[2] -> output-room[3] etc.