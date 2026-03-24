using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class RoomGen : MonoBehaviour
{
    //PREFABS + LAYOUT
    public GameObject outerWall;
    public GameObject floor;
    public GameObject corner;
    public GameObject innerCorner;
    public GameObject door;
    public GameObject output;
    public GameObject innerWall;
    public GameObject innerWallCorner;

    //Call the LayoutGen script
    public LayoutGen roomLayout;

    //PARENT OBJECTS
    [HideInInspector] public GameObject roomParent1;
    [HideInInspector] public GameObject roomParent2;
    [HideInInspector] public GameObject roomParent3;
    [HideInInspector] public GameObject roomParent4;
    [HideInInspector] public GameObject roomParent5;
    [HideInInspector] public GameObject[] roomParent;

    //EVENT
    [HideInInspector] public UnityEvent OnRoomGenComplete;

    //LAYOUTS
    [HideInInspector] public int[,] layout1;
    [HideInInspector] public int[,] layout2;
    [HideInInspector] public int[,] layout3;
    [HideInInspector] public int[,] layout4;
    [HideInInspector] public int[,] layout5;

    //Rows and cols for each layout (will be made dynamic later)
    [HideInInspector] public int rows1;
    [HideInInspector] public int rows2;
    [HideInInspector] public int rows3;
    [HideInInspector] public int rows4;
    [HideInInspector] public int rows5;

    [HideInInspector] public int cols1;
    [HideInInspector] public int cols2;
    [HideInInspector] public int cols3;
    [HideInInspector] public int cols4;
    [HideInInspector] public int cols5;

    //LISTS + ARRAYS
    [HideInInspector] public List<int[,]> layoutList;
    [HideInInspector] public int[] rowsArray;
    [HideInInspector] public int[] colsArray;

    //ROTATIONS
    [HideInInspector] public Quaternion rotation;

    //Tile type constants
    private const int EMPTY = 0;
    private const int CORNER = 1;
    private const int WALL = 2;
    private const int FLOOR = 3;
    private const int DOOR = 4;
    private const int OUTPUT = 5;
    private const int INNER_WALL = 6;

    //Tile handler dictionary - maps tile type to handler function
    private Dictionary<int, System.Action<int, int, int, int[,], int, int>> tileHandlers;

    //Position lookup - maps layoutIndex -> tileType -> list of Vector2Int positions
    [HideInInspector] public Dictionary<int, Dictionary<int, List<Vector2Int>>> tilePositionsByRoom;

    void Start()
    {
        InitializeTileHandlers();
        RoomGeneration();
    }

    void InitializeTileHandlers()
    {
        tileHandlers = new Dictionary<int, System.Action<int, int, int, int[,], int, int>>()
        {
            { EMPTY, (x, y, layoutIdx, layout, rows, cols) => { } },
            { CORNER, HandleCorner },
            { WALL, HandleWall },
            { FLOOR, HandleFloor },
            { DOOR, HandleDoor },
            { OUTPUT, HandleOutput },
            { INNER_WALL, HandleInnerWall }
        };

        //Initialize position lookup
        tilePositionsByRoom = new Dictionary<int, Dictionary<int, List<Vector2Int>>>();
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

        layout4 = roomLayout.layout4;
        rows4 = layout4.GetLength(0);
        cols4 = layout4.GetLength(1);

        layout5 = roomLayout.layout5;
        rows5 = layout5.GetLength(0);
        cols5 = layout5.GetLength(1);

        layoutList = new List<int[,]>();
        layoutList.Add(layout1);
        layoutList.Add(layout2);
        layoutList.Add(layout3);
        layoutList.Add(layout4);
        layoutList.Add(layout5);

        rowsArray = new int[] { rows1, rows2, rows3, rows4, rows5 };
        colsArray = new int[] { cols1, cols2, cols3, cols4, cols5 };

        //Parent Object. The instantiated prefabs are placed into this parent.
        roomParent1 = new GameObject("Room");
        roomParent2 = new GameObject("Room2");
        roomParent3 = new GameObject("Room3");
        roomParent4 = new GameObject("Room4");
        roomParent5 = new GameObject("Room5");

        roomParent = new GameObject[] { roomParent1, roomParent2, roomParent3, roomParent4, roomParent5 };

        //Creates layoutList based on the "layoutList" list index
        for (int i = 0; i < layoutList.Count; i++)
        {
            //Initialize tile position lookup for this room
            Dictionary<int, List<Vector2Int>> roomPositions = new Dictionary<int, List<Vector2Int>>();
            for (int tileType = 0; tileType <= 6; tileType++)
            {
                roomPositions[tileType] = new List<Vector2Int>();
            }
            tilePositionsByRoom[i] = roomPositions;

            for (int y = 0; y < rowsArray[i]; y++)
            {
                for (int x = 0; x < colsArray[i]; x++)
                {
                    int tileType = layoutList[i][y, x];

                    //Record position in lookup
                    tilePositionsByRoom[i][tileType].Add(new Vector2Int(x, y));

                    //Use hash map to retrieve and execute tile handler
                    if (tileHandlers.TryGetValue(tileType, out var handler))
                    {
                        handler(x, y, i, layoutList[i], rowsArray[i], colsArray[i]);
                    }
                }
            }

            CombineRoomMeshes(roomParent[i]);
        }
        //Invokes event listener in FloorCreator.cs to run once room generation code is complete.
        OnRoomGenComplete.Invoke();
    }

    private void HandleCorner(int x, int y, int layoutIdx, int[,] layout, int rows, int cols)
    {
        Vector3 position = new Vector3(x, 0, -y);

        //Check for adjacent corners and walls
        bool top = (y > 0) && (layout[y - 1, x] == CORNER || layout[y - 1, x] == WALL);
        bool bottom = (y < rows - 1) && (layout[y + 1, x] == CORNER || layout[y + 1, x] == WALL);
        bool left = (x > 0) && (layout[y, x - 1] == CORNER || layout[y, x - 1] == WALL);
        bool right = (x < cols - 1) && (layout[y, x + 1] == CORNER || layout[y, x + 1] == WALL);

        //Determine rotation based on wall positions
        rotation = GetCornerRotation(top, bottom, left, right);

        //Check for adjacent floors
        top = (y > 0) && (layout[y - 1, x] == FLOOR);
        bottom = (y < rows - 1) && (layout[y + 1, x] == FLOOR);
        left = (x > 0) && (layout[y, x - 1] == FLOOR);
        right = (x < cols - 1) && (layout[y, x + 1] == FLOOR);

        //Use innerCorner if adjacent to floors, otherwise regular corner
        GameObject prefab = (top || bottom || left || right) ? innerCorner : corner;
        Instantiate(prefab, position, rotation, roomParent[layoutIdx].transform);
    }

    private Quaternion GetCornerRotation(bool top, bool bottom, bool left, bool right)
    {
        if (bottom && right) return Quaternion.Euler(0, 90, 0);
        if (bottom && left) return Quaternion.Euler(0, 180, 0);
        if (top && right) return Quaternion.Euler(0, 0, 0);
        if (top && left) return Quaternion.Euler(0, 270, 0);
        return Quaternion.identity;
    }

    private void HandleWall(int x, int y, int layoutIdx, int[,] layout, int rows, int cols)
    {
        Vector3 position = new Vector3(x, 0, -y);

        //Check for adjacent floors and inner walls
        bool top = (y > 0) && (layout[y - 1, x] == FLOOR || layout[y - 1, x] == INNER_WALL);
        bool bottom = (y < rows - 1) && (layout[y + 1, x] == FLOOR || layout[y + 1, x] == INNER_WALL);
        bool left = (x > 0) && (layout[y, x - 1] == FLOOR || layout[y, x - 1] == INNER_WALL);
        bool right = (x < cols - 1) && (layout[y, x + 1] == FLOOR || layout[y, x + 1] == INNER_WALL);

        //Check if adjacent to doors or outputs
        bool doorTop = (y > 0) && (layout[y - 1, x] == DOOR || layout[y - 1, x] == OUTPUT);
        bool doorBottom = (y < rows - 1) && (layout[y + 1, x] == DOOR || layout[y + 1, x] == OUTPUT);
        bool doorLeft = (x > 0) && (layout[y, x - 1] == DOOR || layout[y, x - 1] == OUTPUT);
        bool doorRight = (x < cols - 1) && (layout[y, x + 1] == DOOR || layout[y, x + 1] == OUTPUT);

        //Don't place wall if adjacent to door or output
        if (doorTop || doorBottom || doorLeft || doorRight)
            return;

        //Determine rotation based on floor positions
        rotation = GetWallRotation(top, bottom, left, right);
        Instantiate(outerWall, position, rotation, roomParent[layoutIdx].transform);
    }

    private Quaternion GetWallRotation(bool top, bool bottom, bool left, bool right)
    {
        if (top) return Quaternion.Euler(0, 90, 0);
        if (bottom) return Quaternion.Euler(0, 270, 0);
        if (left) return Quaternion.Euler(0, 0, 0);
        if (right) return Quaternion.Euler(0, 180, 0);
        return Quaternion.identity;
    }

    private void HandleFloor(int x, int y, int layoutIdx, int[,] layout, int rows, int cols)
    {
        Vector3 position = new Vector3(x, 0, -y);
        Instantiate(floor, position, Quaternion.identity, roomParent[layoutIdx].transform);
    }

    private void HandleDoor(int x, int y, int layoutIdx, int[,] layout, int rows, int cols)
    {
        Vector3 position = new Vector3(x, 0, -y);

        bool top = (y > 0) && (layout[y - 1, x] == FLOOR);
        bool bottom = (y < rows - 1) && (layout[y + 1, x] == FLOOR);
        bool left = (x > 0) && (layout[y, x - 1] == FLOOR);
        bool right = (x < cols - 1) && (layout[y, x + 1] == FLOOR);

        rotation = GetDoorRotation(top, bottom, left, right);
        Instantiate(door, position, rotation, roomParent[layoutIdx].transform);
    }

    private Quaternion GetDoorRotation(bool top, bool bottom, bool left, bool right)
    {
        if (top || bottom) return Quaternion.Euler(0, 90, 0);
        if (left || right) return Quaternion.Euler(0, 0, 0);
        return Quaternion.identity;
    }

    private void HandleOutput(int x, int y, int layoutIdx, int[,] layout, int rows, int cols)
    {
        Vector3 position = new Vector3(x, 0, -y);

        bool top = (y > 0) && (layout[y - 1, x] == FLOOR);
        bool bottom = (y < rows - 1) && (layout[y + 1, x] == FLOOR);
        bool left = (x > 0) && (layout[y, x - 1] == FLOOR);
        bool right = (x < cols - 1) && (layout[y, x + 1] == FLOOR);

        rotation = GetOutputRotation(top, bottom, left, right);
        Instantiate(output, position, rotation, roomParent[layoutIdx].transform);
    }

    private Quaternion GetOutputRotation(bool top, bool bottom, bool left, bool right)
    {
        if (top) return Quaternion.Euler(0, 270, 0);
        if (bottom) return Quaternion.Euler(0, 90, 0);
        if (left) return Quaternion.Euler(0, 180, 0);
        if (right) return Quaternion.Euler(0, 0, 0);
        return Quaternion.identity;
    }

    private void HandleInnerWall(int x, int y, int layoutIdx, int[,] layout, int rows, int cols)
    {
        Vector3 position = new Vector3(x, 0, -y);

        bool top = (y > 0) && (layout[y - 1, x] == INNER_WALL);
        bool bottom = (y < rows - 1) && (layout[y + 1, x] == INNER_WALL);
        bool left = (x > 0) && (layout[y, x - 1] == INNER_WALL);
        bool right = (x < cols - 1) && (layout[y, x + 1] == INNER_WALL);

        rotation = GetInnerWallRotation(top, bottom, left, right);

        if (top && left || top && right || bottom && left || bottom && right)
        {
            Instantiate(innerWallCorner, position, rotation, roomParent[layoutIdx].transform);
        }
        else
        {
            Instantiate(innerWall, position, rotation, roomParent[layoutIdx].transform);
        }

    }

    private Quaternion GetInnerWallRotation(bool top, bool bottom, bool left, bool right)
    {
        if (bottom && right) return Quaternion.Euler(0, 0, 0);
        if (bottom && left) return Quaternion.Euler(0, 90, 0);
        if (top && right) return Quaternion.Euler(0, 270, 0);
        if (top && left) return Quaternion.Euler(0, 180, 0);

        if (top || bottom) return Quaternion.Euler(0, 0, 0);
        if (left || right) return Quaternion.Euler(0, 90, 0);
        return Quaternion.identity;
    }

    /// <summary>
    /// Get all positions of a specific tile type in a room
    /// </summary>
    public List<Vector2Int> GetTilePositions(int roomIndex, int tileType)
    {
        if (tilePositionsByRoom.TryGetValue(roomIndex, out var roomPositions))
        {
            if (roomPositions.TryGetValue(tileType, out var positions))
            {
                return positions;
            }
        }
        return new List<Vector2Int>();
    }

    public Vector2Int? FindTilePosition(int roomIndex, int tileType)
    {
        List<Vector2Int> positions = GetTilePositions(roomIndex, tileType);
        return positions.Count > 0 ? positions[0] : null;
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

        //Increase vertices limit
        combinedMesh.indexFormat = IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combine, true, true);
        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();

        // Add MeshCollider to the roomParent
        MeshCollider meshCollider = roomParent.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = combinedMesh;
    }
}