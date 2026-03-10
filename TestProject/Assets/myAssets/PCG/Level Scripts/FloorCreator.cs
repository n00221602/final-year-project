using UnityEngine;

public class FloorCreator : MonoBehaviour
{

    public LayoutGen roomLayout;
    public RoomGen roomGen;

    //Reassignable variable used when changing the position of a room.
    //GameObject roomParent;

    Vector3 entryPosition;
    Vector3 exitPosition;

    public Vector3[] outputPosArray;

    bool top;
    bool bottom;
    bool left;
    bool right;

    bool inputTop;
    bool inputBottom;
    bool inputLeft;
    bool inputRight;

    bool outputTop;
    bool outputBottom;
    bool outputLeft;
    bool outputRight;
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
                //Debug.Log("room parent is null");
                return;
            }

            //Find input position Input is set to 4 in LayoutGen.cs and to a door prefab in RoomGen.cs.
            for (int y = 0; y < roomGen.rowsArray[i]; y++)
            {
                for (int x = 0; x < roomGen.colsArray[i]; x++)
                {
                    if (roomGen.layoutList[i][y, x] == 4)
                    {
                        //Debug.Log("door found at" + (x, y));
                        entryPosition = new Vector3(x, 0, -y);

                        //Debug.Log("door rotation is" + roomGen.roomParent[i].transform.rotation);
                        currentDoorRotation = roomGen.roomParent[i].transform.rotation;
                        break;
                    }
                }
            }

            //Find output location. Output is set to 5 in LayoutGen.cs.
            for (int y = 0; y < roomGen.rowsArray[i + 1]; y++)
            {
                for (int x = 0; x < roomGen.colsArray[i + 1]; x++)
                {
                    if (roomGen.layoutList[i + 1][y, x] == 5)
                    {
                        //Debug.Log("exit found at" + (x, y));
                        exitPosition = new Vector3(x, 0, -y);
                        outputPosArray[i] = exitPosition;
                        break;
                    }
                }
            }

            //Subtract the '4' and '5' positions to find the difference.
            Vector3 positionDifference = entryPosition - exitPosition;
            //Debug.Log("position difference is" + positionDifference);



            //ROTATION//

            //Set x and y values to the '4'' position (the door from the input room).
            int entryX = (int)entryPosition.x;
            int entryY = (int)-entryPosition.z;

            //Check for neighbouring floor tiles around the door.
            top = (entryY > 0) && (roomGen.layoutList[i][entryY - 1, entryX] == floorTarget);
            bottom = (entryY < roomGen.rowsArray[i] - 1) && (roomGen.layoutList[i][entryY + 1, entryX] == floorTarget);
            left = (entryX > 0) && (roomGen.layoutList[i][entryY, entryX - 1] == floorTarget);
            right = (entryX < roomGen.colsArray[i] - 1) && (roomGen.layoutList[i][entryY, entryX + 1] == floorTarget);



            //Since output is can either be on the top, bottom, left or right of the room, we need additional checks for deciding rotation

            //POSSIBLE SOLUTIONS:
            //Make 4 layout arrays for rooms that have their door at the top,bottom,left or right.
            //Add if statements for each possible connection in this code?
            //Make each floor like a hallway, each room has an input and output on the left and right sides of the layout.

            if (top)
            {
                roomRotation = Quaternion.Euler(0, 180, 0);
            }
            else if (bottom)
            {
                roomRotation = Quaternion.Euler(0, 180, 0);
            }
            else if (left)
            {
                roomRotation = Quaternion.Euler(0, 180, 0);
            }
            else if (right)
            {
                roomRotation = Quaternion.Euler(0, 180, 0);
            }

            if (positionDifference != null)
            {
                //Move the next room by the position difference AND the '4' room's position. since this room is not always at (0,0,0).
                roomGen.roomParent[i + 1].transform.position = positionDifference + roomGen.roomParent[i].transform.position;
                //roomGen.roomParent[i + 1].transform.rotation = roomRotation;
            }
        }
        //// Debug array contents
        //for (int i = 0; i < outputPosArray.Length; i++)
        //{
        //    Debug.Log($"OUTPUT ARRAY[{i}]: {outputPosArray[i]}");
        //}
    }

}


//NOTE: this script needs to find the input from a previous room and match it with the ouput of the next room. Rooms can be decided in an array.
//Every room has an input and output EXCEPT for the starting room, which only has an input.
//input-room[0] -> output-room[1] | input-room[1] -> output-room[2] | input-room[2] -> output-room[3] etc.