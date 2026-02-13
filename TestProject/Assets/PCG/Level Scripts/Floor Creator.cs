using UnityEngine;

public class FloorCreator : MonoBehaviour
{

    public LayoutGen roomLayout;
    public RoomGen roomGen;

    void Awake()
    {
        //Once the room generation is complete, CreateFloorLayout is called to reposition the rooms accordingly.
        roomGen.OnRoomGenComplete.AddListener(CreateFloorLayout);
    }

    void CreateFloorLayout()
    {
        int[,] layout1 = roomLayout.layout1;
        int[,] layout2 = roomLayout.layout2;
        int rows1 = layout1.GetLength(0);
        int cols1 = layout1.GetLength(1);
        int rows2 = layout2.GetLength(0);
        int cols2 = layout2.GetLength(1);

        GameObject roomParent;

        if (roomGen.roomParent2 == null)
        {
            Debug.LogError("Room parent not assigned");
            return;
        }

        if (roomGen.roomParent2 != null)
        {
            Debug.LogError("Room parent found");
            roomParent = roomGen.roomParent2;



            Vector3 entryPosition = new Vector3(0, 0, 0);
            Vector3 exitPosition = new Vector3(0, 0, 0);

            //Find 4 location (entry)
            for (int y = 0; y < rows1; y++)
            {
                for (int x = 0; x < cols1; x++)
                {
                    if (layout1[y, x] == 4)
                    {
                        Debug.Log("door found at" + (x, y));
                        entryPosition = new Vector3(x, 0, -y);
                        break;
                    }
                }
            }

            //Find 5 location (exit)
            for (int y = 0; y < rows2; y++)
            {
                for (int x = 0; x < cols2; x++)
                {
                    if (layout2[y, x] == 5)
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
