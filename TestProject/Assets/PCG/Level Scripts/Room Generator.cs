using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RoomGen : MonoBehaviour
{
    public GameObject outerWall;
    public GameObject floor;
    public GameObject corner;
    public GameObject innerCorner;
    public GameObject door;
    public LayoutGen roomLayout;

    [HideInInspector]
    public GameObject roomParent;
    [HideInInspector]
    public GameObject roomParent2;

    public UnityEvent OnRoomGenComplete;

    void Start()
    {
        RoomGeneration();
    }
    void RoomGeneration()
    {
        if (roomLayout == null)
        {
            Debug.LogError("LayoutGen not assigned");
            return;
        }


        int[,] layout = roomLayout.layout1;
        int rows = layout.GetLength(0);
        int cols = layout.GetLength(1);

        int[,] layout2 = roomLayout.layout2;
        int rows2 = layout2.GetLength(0);
        int cols2 = layout2.GetLength(1);



        //Tile targets later used for checking neighbouring tiles
        int cornerTarget = 1;
        int wallTarget = 2;
        int floorTarget = 3;

        //Target bools used for checking neighbouring tiles.
        bool top;
        bool bottom;
        bool left;
        bool right;

        //Parent Object. The instantiated prefabs are placed into this parent.
        roomParent = new GameObject("Room");
        roomParent2 = new GameObject("Room2");

        //CREATING ROOMS MUST USE A FOR EACH LOOP LATER.

        Quaternion cornerRotation = Quaternion.Euler(0, 0, 0);
        Quaternion wallRotation = Quaternion.Euler(0, 0, 0);
        Quaternion doorRotation = Quaternion.Euler(0, 0, 0);

        //Create first layout
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                //Since scene is 3D, y axis is up. The z axis is equivilant to "height" for a 2D grid
                Vector3 position = new Vector3(x, 0, -y);
                switch (layout[y, x])
                {
                    //Empty space. empty = 0
                    case 0:
                        break;

                    //Create corners. corners = 1 //UPDATE TO MATCH WITH *ANYTHING* THAT IS NOT A FLOOR OR EMPTY (CASE 1 OR 2, NOT 0 OR 3) do i have to do this?
                    case 1:

                        //Change bools to check for corners and walls, with added checks to make sure they are within the array index.
                        top = (y > 0) && (layout[y - 1, x] == cornerTarget || layout[y - 1, x] == wallTarget);
                        bottom = (y < rows - 1) && (layout[y + 1, x] == cornerTarget || layout[y + 1, x] == wallTarget);
                        left = (x > 0) && (layout[y, x - 1] == cornerTarget || layout[y, x - 1] == wallTarget);
                        right = (x < cols - 1) && (layout[y, x + 1] == cornerTarget || layout[y, x + 1] == wallTarget);

                        //Corners are rotated accordingly to match surrounding walls.
                        if (bottom && right) //walls on bottom and right (top-left corner)
                        {
                            cornerRotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (bottom && left) //walls on bottom and left (top-right corner)
                        {
                            cornerRotation = Quaternion.Euler(0, 180, 0);
                        }
                        else if (top && right) //wall on top and right (bottom-left corner)
                        {
                            cornerRotation = Quaternion.Euler(0, 0, 0);
                        }
                        else if (top && left) //wall on top and left (bottom-right corner)
                        {
                            cornerRotation = Quaternion.Euler(0, 270, 0);
                        }

                        //Change bools to check for floors
                        top = (y > 0) && (layout[y - 1, x] == floorTarget);
                        bottom = (y < rows - 1) && (layout[y + 1, x] == floorTarget);
                        left = (x > 0) && (layout[y, x - 1] == floorTarget);
                        right = (x < cols - 1) && (layout[y, x + 1] == floorTarget);

                        //If there are floors next to the corner, use innerCorner. Else use regular corner.
                        if (top || bottom || left || right)
                        {
                            Instantiate(innerCorner, position, cornerRotation, roomParent.transform);
                            break;
                        }
                        else
                        {
                            Instantiate(corner, position, cornerRotation, roomParent.transform);
                            break;
                        }



                    //Create walls. walls = 2
                    case 2:
                        //tileTarget = 3; //Set target to floors
                        //Change bools to check for floors
                        top = (y > 0) && (layout[y - 1, x] == floorTarget);
                        bottom = (y < rows - 1) && (layout[y + 1, x] == floorTarget);
                        left = (x > 0) && (layout[y, x - 1] == floorTarget);
                        right = (x < cols - 1) && (layout[y, x + 1] == floorTarget);

                        //Outer walls are rotated accordingly depending on neighbouring floor tile positions.
                        if (top)
                        {
                            wallRotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (bottom)
                        {
                            wallRotation = Quaternion.Euler(0, 270, 0);
                        }
                        else if (left)
                        {
                            wallRotation = Quaternion.Euler(0, 0, 0);
                        }
                        else if (right)
                        {
                            wallRotation = Quaternion.Euler(0, 180, 0);
                        }
                        Instantiate(outerWall, position, wallRotation, roomParent.transform);
                        break;

                    //Create floors. floors = 3
                    case 3:
                        Instantiate(floor, position, Quaternion.identity, roomParent.transform);
                        break;

                    //Create doors. doors = 4
                    case 4:
                        top = (y > 0) && (layout[y - 1, x] == floorTarget);
                        bottom = (y < rows - 1) && (layout[y + 1, x] == floorTarget);
                        left = (x > 0) && (layout[y, x - 1] == floorTarget);
                        right = (x < cols - 1) && (layout[y, x + 1] == floorTarget);

                        //Outer walls are rotated accordingly depending on neighbouring floor tile positions.
                        if (top)
                        {
                            doorRotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (bottom)
                        {
                            doorRotation = Quaternion.Euler(0, 270, 0);
                        }
                        else if (left)
                        {
                            doorRotation = Quaternion.Euler(0, 0, 0);
                        }
                        else if (right)
                        {
                            doorRotation = Quaternion.Euler(0, 180, 0);
                        }
                        Instantiate(door, position, Quaternion.identity, roomParent.transform);
                        break;
                }
            }
        }

        //Create second layout
        for (int y = 0; y < rows2; y++)
        {
            for (int x = 0; x < cols2; x++)
            {
                //Since scene is 3D, y axis is up. The z axis is equivilant to "height" for a 2D grid
                Vector3 position = new Vector3(x, 0, -y);
                switch (layout2[y, x])
                {
                    //Empty space. empty = 0
                    case 0:
                        break;

                    //Create corners. corners = 1 //UPDATE TO MATCH WITH *ANYTHING* THAT IS NOT A FLOOR OR EMPTY (CASE 1 OR 2, NOT 0 OR 3) do i have to do this?
                    case 1:

                        //Change bools to check for corners and walls, with added checks to make sure they are within the array index.
                        top = (y > 0) && (layout2[y - 1, x] == cornerTarget || layout2[y - 1, x] == wallTarget);
                        bottom = (y < rows2 - 1) && (layout2[y + 1, x] == cornerTarget || layout2[y + 1, x] == wallTarget);
                        left = (x > 0) && (layout2[y, x - 1] == cornerTarget || layout2[y, x - 1] == wallTarget);
                        right = (x < cols2 - 1) && (layout2[y, x + 1] == cornerTarget || layout2[y, x + 1] == wallTarget);

                        //Corners are rotated accordingly to match surrounding walls.
                        if (bottom && right) //walls on bottom and right (top-left corner)
                        {
                            cornerRotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (bottom && left) //walls on bottom and left (top-right corner)
                        {
                            cornerRotation = Quaternion.Euler(0, 180, 0);
                        }
                        else if (top && right) //wall on top and right (bottom-left corner)
                        {
                            cornerRotation = Quaternion.Euler(0, 0, 0);
                        }
                        else if (top && left) //wall on top and left (bottom-right corner)
                        {
                            cornerRotation = Quaternion.Euler(0, 270, 0);
                        }

                        //Change bools to check for floors
                        top = (y > 0) && (layout2[y - 1, x] == floorTarget);
                        bottom = (y < rows2 - 1) && (layout2[y + 1, x] == floorTarget);
                        left = (x > 0) && (layout2[y, x - 1] == floorTarget);
                        right = (x < cols2 - 1) && (layout2[y, x + 1] == floorTarget);

                        //If there are floors next to the corner, use innerCorner. Else use regular corner.
                        if (top || bottom || left || right)
                        {
                            Instantiate(innerCorner, position, cornerRotation, roomParent2.transform);
                            break;
                        }
                        else
                        {
                            Instantiate(corner, position, cornerRotation, roomParent2.transform);
                            break;
                        }



                    //Create walls. walls = 2
                    case 2:
                        //tileTarget = 3; //Set target to floors
                        //Change bools to check for floors
                        top = (y > 0) && (layout2[y - 1, x] == floorTarget);
                        bottom = (y < rows2 - 1) && (layout2[y + 1, x] == floorTarget);
                        left = (x > 0) && (layout2[y, x - 1] == floorTarget);
                        right = (x < cols2 - 1) && (layout2[y, x + 1] == floorTarget);

                        //Outer walls are rotated accordingly depending on neighbouring floor tile positions.
                        if (top)
                        {
                            wallRotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (bottom)
                        {
                            wallRotation = Quaternion.Euler(0, 270, 0);
                        }
                        else if (left)
                        {
                            wallRotation = Quaternion.Euler(0, 0, 0);
                        }
                        else if (right)
                        {
                            wallRotation = Quaternion.Euler(0, 180, 0);
                        }
                        Instantiate(outerWall, position, wallRotation, roomParent2.transform);
                        break;

                    //Create floors. floors = 3
                    case 3:
                        Instantiate(floor, position, Quaternion.identity, roomParent2.transform);
                        break;

                    //Create doors. doors = 4
                    case 4:
                        top = (y > 0) && (layout2[y - 1, x] == floorTarget);
                        bottom = (y < rows2 - 1) && (layout2[y + 1, x] == floorTarget);
                        left = (x > 0) && (layout2[y, x - 1] == floorTarget);
                        right = (x < cols2 - 1) && (layout2[y, x + 1] == floorTarget);

                        //Outer walls are rotated accordingly depending on neighbouring floor tile positions.
                        if (top)
                        {
                            doorRotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (bottom)
                        {
                            doorRotation = Quaternion.Euler(0, 270, 0);
                        }
                        else if (left)
                        {
                            doorRotation = Quaternion.Euler(0, 0, 0);
                        }
                        else if (right)
                        {
                            doorRotation = Quaternion.Euler(0, 180, 0);
                        }
                        Instantiate(door, position, Quaternion.identity, roomParent2.transform);
                        break;
                }
            }
        }


        OnRoomGenComplete?.Invoke();
    }
}
