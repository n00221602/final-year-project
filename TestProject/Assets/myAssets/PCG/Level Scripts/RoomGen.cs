using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomGen : MonoBehaviour
{
    //PREFABS + LAYOUT
    public GameObject outerWall;
    public GameObject floor;
    public GameObject corner;
    public GameObject innerCorner;
    public GameObject door;
    public GameObject output;

    //Call the LayoutGen script
    public LayoutGen roomLayout;

    //PARENT OBJECTS
    [HideInInspector] public GameObject roomParent1;
    [HideInInspector] public GameObject roomParent2;
    [HideInInspector] public GameObject roomParent3;
    [HideInInspector] public GameObject[] roomParent;

    //EVENT
    [HideInInspector] public UnityEvent OnRoomGenComplete;

    //LAYOUTS
    [HideInInspector] public int[,] layout1;
    [HideInInspector] public int[,] layout2;
    [HideInInspector] public int[,] layout3;

    //Rows and cols for each layout (will be made dynamic later)
    [HideInInspector] public int rows1;
    [HideInInspector] public int cols1;
    [HideInInspector] public int rows2;
    [HideInInspector] public int cols2;
    [HideInInspector] public int rows3;
    [HideInInspector] public int cols3;

    //LISTS + ARRAYS
    [HideInInspector] public List<int[,]> layoutList;
    [HideInInspector] public int[] rowsArray;
    [HideInInspector] public int[] colsArray;

    //ROTATIONS
    [HideInInspector] public Quaternion cornerRotation;
    [HideInInspector] public Quaternion wallRotation;
    [HideInInspector] public Quaternion doorRotation;
    [HideInInspector] public Quaternion outputRotation;

    //PHYSICS MATERIAL
    //public PhysicsMaterial physicsMaterial;

    void Start()
    {
        RoomGeneration();
    }
    void RoomGeneration()
    {
        layout1 = roomLayout.layout1;
        rows1 = layout1.GetLength(0);
        cols1 = layout1.GetLength(1);

        layout2 = roomLayout.layout2;
        rows2 = layout2.GetLength(0);
        cols2 = layout2.GetLength(1);

        layout3 = roomLayout.layout3;
        rows3 = layout3.GetLength(0);
        cols3 = layout3.GetLength(1);

        layoutList = new List<int[,]>();
        layoutList.Add(layout1);
        layoutList.Add(layout2);
        layoutList.Add(layout3);

        rowsArray = new int[] { rows1, rows2, rows3 };
        colsArray = new int[] { cols1, cols2, cols3 };

        //Tile targets later used for checking neighbouring tiles
        int cornerTarget = 1;
        int wallTarget = 2;
        int floorTarget = 3;
        int doorTarget = 4;
        int outputTarget = 5;

        //Target bools used for checking neighbouring tiles.
        bool top;
        bool bottom;
        bool left;
        bool right;

        //Target bools for checking door post
        bool doorTop;
        bool doorBottom;
        bool doorLeft;
        bool doorRight;

        //Parent Object. The instantiated prefabs are placed into this parent.
        roomParent1 = new GameObject("Room");
        roomParent2 = new GameObject("Room2");
        roomParent3 = new GameObject("Room3");

        roomParent = new GameObject[] { roomParent1, roomParent2, roomParent3 };

        cornerRotation = Quaternion.Euler(0, 0, 0);
        wallRotation = Quaternion.Euler(0, 0, 0);
        doorRotation = Quaternion.Euler(0, 0, 0);

        //Creates layoutList based on the "layoutList" list index
        for (int i = 0; i < layoutList.Count; i++)
        {
            //Debug.Log("Creating layout " + (i));
            for (int y = 0; y < rowsArray[i]; y++)
            {
                for (int x = 0; x < colsArray[i]; x++)
                {
                    //Since scene is 3D, y axis is up. The z axis is equivilant to "height" for a 2D grid
                    Vector3 position = new Vector3(x, 0, -y);
                    switch (layoutList[i][y, x])
                    {
                        //Empty space. empty = 0
                        case 0:
                            break;

                        //Create corners. corners = 1 //UPDATE TO MATCH WITH *ANYTHING* THAT IS NOT A FLOOR OR EMPTY (CASE 1 OR 2, NOT 0 OR 3) do i have to do this?
                        case 1:

                            //Change bools to check for corners and walls, with added checks to make sure they are within the array index.
                            top = (y > 0) && (layoutList[i][y - 1, x] == cornerTarget || layoutList[i][y - 1, x] == wallTarget);
                            bottom = (y < rowsArray[i] - 1) && (layoutList[i][y + 1, x] == cornerTarget || layoutList[i][y + 1, x] == wallTarget);
                            left = (x > 0) && (layoutList[i][y, x - 1] == cornerTarget || layoutList[i][y, x - 1] == wallTarget);
                            right = (x < colsArray[i] - 1) && (layoutList[i][y, x + 1] == cornerTarget || layoutList[i][y, x + 1] == wallTarget);

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
                            top = (y > 0) && (layoutList[i][y - 1, x] == floorTarget);
                            bottom = (y < rowsArray[i] - 1) && (layoutList[i][y + 1, x] == floorTarget);
                            left = (x > 0) && (layoutList[i][y, x - 1] == floorTarget);
                            right = (x < colsArray[i] - 1) && (layoutList[i][y, x + 1] == floorTarget);

                            //If there are floors next to the corner, use innerCorner. Else use regular corner.
                            if (top || bottom || left || right)
                            {
                                Instantiate(innerCorner, position, cornerRotation, roomParent[i].transform);
                            }
                            else
                            {
                                Instantiate(corner, position, cornerRotation, roomParent[i].transform);
                            }
                            break;



                        //Create walls. walls = 2
                        case 2:
                            //tileTarget = 3; //Set target to floors
                            //Change bools to check for floors
                            top = (y > 0) && (layoutList[i][y - 1, x] == floorTarget);
                            bottom = (y < rowsArray[i] - 1) && (layoutList[i][y + 1, x] == floorTarget);
                            left = (x > 0) && (layoutList[i][y, x - 1] == floorTarget);
                            right = (x < colsArray[i] - 1) && (layoutList[i][y, x + 1] == floorTarget);

                            //Since doors are 1x3, dont place walls next to doors or outputs.
                            doorTop = (y > 0) && (layoutList[i][y - 1, x] == doorTarget || layoutList[i][y - 1, x] == outputTarget);
                            doorBottom = (y < rowsArray[i] - 1) && (layoutList[i][y + 1, x] == doorTarget || layoutList[i][y + 1, x] == outputTarget);
                            doorLeft = (x > 0) && (layoutList[i][y, x - 1] == doorTarget || layoutList[i][y, x - 1] == outputTarget);
                            doorRight = (x < colsArray[i] - 1) && (layoutList[i][y, x + 1] == doorTarget || layoutList[i][y, x + 1] == outputTarget);

                            if (doorTop || doorBottom || doorLeft || doorRight)
                            {
                                break;
                            }

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
                            Instantiate(outerWall, position, wallRotation, roomParent[i].transform);
                            break;



                        //Create floors. floors = 3
                        case 3:
                            Instantiate(floor, position, Quaternion.identity, roomParent[i].transform);
                            break;

                        //Create doors. doors = 4
                        case 4:
                            top = (y > 0) && (layoutList[i][y - 1, x] == floorTarget);
                            bottom = (y < rowsArray[i] - 1) && (layoutList[i][y + 1, x] == floorTarget);
                            left = (x > 0) && (layoutList[i][y, x - 1] == floorTarget);
                            right = (x < colsArray[i] - 1) && (layoutList[i][y, x + 1] == floorTarget);

                            //Outer walls are rotated accordingly depending on neighbouring floor tile positions.
                            if (top || bottom)
                            {
                                doorRotation = Quaternion.Euler(0, 90, 0);
                            }
                            else if (left || right)
                            {
                                doorRotation = Quaternion.Euler(0, 0, 0);
                            }
                            Instantiate(door, position, doorRotation, roomParent[i].transform);
                            break;

                        case 5:
                            top = (y > 0) && (layoutList[i][y - 1, x] == floorTarget);
                            bottom = (y < rowsArray[i] - 1) && (layoutList[i][y + 1, x] == floorTarget);
                            left = (x > 0) && (layoutList[i][y, x - 1] == floorTarget);
                            right = (x < colsArray[i] - 1) && (layoutList[i][y, x + 1] == floorTarget);
                            //Outer walls are rotated accordingly depending on neighbouring floor tile positions.
                            if (top)
                            {
                                outputRotation = Quaternion.Euler(0, 270, 0);
                            }
                            else if (bottom)
                            {
                                outputRotation = Quaternion.Euler(0, 90, 0);
                            }
                            else if (left)
                            {
                                outputRotation = Quaternion.Euler(0, 180, 0);
                            }
                            else if (right)
                            {
                                outputRotation = Quaternion.Euler(0, 0, 0);
                            }
                            Instantiate(output, position, outputRotation, roomParent[i].transform);
                            break;
                    }
                }
            }
            CombineRoomMeshes(roomParent[i]);
        }
        //Invokes event listener in FloorCreator.cs to run once room generation code is complete.
        OnRoomGenComplete.Invoke();
    }

    void CombineRoomMeshes(GameObject roomParent)
    {
        // Get all MeshFilters from direct children only (not nested)
        MeshFilter[] meshFilters = roomParent.GetComponentsInChildren<MeshFilter>();

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            Mesh mesh = meshFilters[i].sharedMesh;
            combine[i].mesh = mesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

        }

        // Create combined mesh
        Mesh combinedMesh = new();
        combinedMesh.CombineMeshes(combine, true, true);
        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();

        // Add MeshCollider to the roomParent
        MeshCollider meshCollider = roomParent.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = combinedMesh;
    }
}
