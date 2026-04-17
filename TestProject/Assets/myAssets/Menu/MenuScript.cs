using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class MenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject menuUI;
    public GameObject generateUI;
    //public TMP_InputField inputField;
    public RectTransform gridParent;
    public TMP_InputField widthInputField;
    public TMP_InputField heightInputField;
    public GameObject gridPrefab;

    public GridLayoutGroup gridLayoutGroup;

    [HideInInspector] public int[,] userLayout;

    string widthInput;
    string heightInput;

    float squareInputAmount;
    int currentSquareAmount;
    float totalSquaresWidth;
    float totalSquaresHeight;

    [HideInInspector] public UnityEvent onUserSubmit;

    public RoomGen roomGen;

    void Start()
    {
        //Make sure only the menu UI is active when the scene first starts
        if (generateUI.activeSelf)
        {
            generateUI.SetActive(false);
        }

        //Set default grid width and height
        widthInput = "5";
        heightInput = "5";
    }

    // Update is called once per frame
    public void OnStartButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void OnGenerateButton()
    {

        menuUI.SetActive(false);
        generateUI.SetActive(true);

    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    //GENERATE VIEW
    public void OnBackButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }

    public void OnSubmitButton()
    {
        //Create a 2D array based on the width and height inputted by the user
        int[,] gridArray = new int[(int)totalSquaresHeight, (int)totalSquaresWidth];

        int squareIndex = 0;

        //Loop through each index and assign the corresponding square from the grid.
        for (int y = 0; y < gridArray.GetLength(0); y++)
        {
            for (int x = 0; x < gridArray.GetLength(1); x++)
            {
                gridArray[y, x] = int.Parse(gridParent.GetChild(squareIndex).GetComponentInChildren<TMP_InputField>().text);
                squareIndex++;
                Debug.Log("Grid Array Value at [" + y + ", " + x + "]: " + gridArray[y, x]);

                Debug.Log("Child Amount: " + squareIndex);
            }
        }

        roomGen.CreateRoomPreview(gridArray);

    }

    public void StoreGridSize(TMP_InputField inputField)
    {
        if (inputField == widthInputField)
        {
            widthInput = inputField.text;
            //widthInput = 
            Debug.Log("Width: " + widthInput);
        }
        else if (inputField == heightInputField)
        {
            heightInput = inputField.text;
            Debug.Log("Height: " + heightInput);
        }
    }




    public void GridHandler()
    {
        float gridParentWidth = gridParent.sizeDelta[0];
        float gridParentHeight = gridParent.sizeDelta[1];

        totalSquaresWidth = float.Parse(widthInput);
        totalSquaresHeight = float.Parse(heightInput);

        squareInputAmount = totalSquaresWidth * totalSquaresHeight;
        currentSquareAmount = gridParent.childCount;

        if (currentSquareAmount > squareInputAmount)
        {
            for (int i = 0; i < currentSquareAmount - squareInputAmount; i++)
            {
                Destroy(gridParent.GetChild(i).gameObject);
            }
        }
        else if (currentSquareAmount < squareInputAmount)
        {
            for (int i = currentSquareAmount; i < squareInputAmount; i++)
            {
                Instantiate(gridPrefab, gridParent);
            }
        }


        Debug.Log(gridParentWidth);
        gridLayoutGroup.cellSize = new Vector2(gridParentWidth / totalSquaresWidth, gridParentHeight / totalSquaresHeight);
    }
}

