using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;


public class MenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject menuUI;
    public GameObject generateUI;
    public GameObject viewLayoutsUI;
    //public TMP_InputField inputField;
    public RectTransform gridHolder;
    public TMP_InputField widthInputField;
    public TMP_InputField heightInputField;
    public GameObject gridPrefab;

    public GridLayoutGroup gridLayoutGroup;

    [HideInInspector] public int[,] userLayout;
    [HideInInspector] public List<int[,]> userLayoutList;

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
        menuUI.SetActive(true);
        generateUI.SetActive(false);
        viewLayoutsUI.SetActive(false);

        userLayoutList = new List<int[,]>();

        //Set default grid width and height
        //widthInput = "5";
        //heightInput = "5";
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

    public void OnViewLayoutsButton()
    {

        menuUI.SetActive(false);
        viewLayoutsUI.SetActive(true);

    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    //GENERATE VIEW
    public void OnBackButton()
    {
        menuUI.SetActive(true);
        generateUI.SetActive(false);
        viewLayoutsUI.SetActive(false);
    }

    public void OnSubmitButton()
    {
        //Create a 2D array based on the width and height inputted by the user
        userLayout = new int[(int)totalSquaresHeight, (int)totalSquaresWidth];

        int squareIndex = 0;

        //Loop through each index and assign the corresponding square from the grid.
        for (int y = 0; y < userLayout.GetLength(0); y++)
        {
            for (int x = 0; x < userLayout.GetLength(1); x++)
            {
                userLayout[y, x] = int.Parse(gridHolder.GetChild(squareIndex).GetComponentInChildren<TMP_InputField>().text);
                squareIndex++;
                Debug.Log("Grid Array Value at [" + y + ", " + x + "]: " + userLayout[y, x]);

                Debug.Log("Child Amount: " + squareIndex);
            }
        }

        roomGen.CreateRoomPreview(userLayout);

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

    public void SaveLayout()
    {
        userLayoutList.Add(userLayout);

    }

    public void RemoveLayout()
    {
        if (userLayoutList.Count > 0)
        {
            userLayoutList.RemoveAt(userLayoutList.Count - 1);
        }
    }


    public void GridHandler()
    {
        float gridHolderWidth = gridHolder.rect.width;
        float gridHolderHeight = gridHolder.rect.height;

        totalSquaresWidth = float.Parse(widthInput);
        totalSquaresHeight = float.Parse(heightInput);

        squareInputAmount = totalSquaresWidth * totalSquaresHeight;
        currentSquareAmount = gridHolder.childCount;

        if (currentSquareAmount > squareInputAmount)
        {
            for (int i = 0; i < currentSquareAmount - squareInputAmount; i++)
            {
                Destroy(gridHolder.GetChild(i).gameObject);
            }
        }
        else if (currentSquareAmount < squareInputAmount)
        {
            for (int i = currentSquareAmount; i < squareInputAmount; i++)
            {
                Instantiate(gridPrefab, gridHolder);
            }
        }


        Debug.Log(gridHolderWidth);
        gridLayoutGroup.cellSize = new Vector2(gridHolderWidth / totalSquaresWidth, gridHolderHeight / totalSquaresHeight);
    }

    public void OnAIButton()
    {
        string userInput = @"Create a new layout that follows the provided legend and criteria: 
                            ## LEGEND
                            - 0 = empty space
                            - 1 = corner
                            - 2 = wall
                            - 3 = floor
                            - 4 = door/entry point
                            - 5 = exit point
                            - 6 = inner wall
                            - 7 = room teleporter (only in end room)

                            ## EXAMPLE LAYOUT (Do not copy this layout)
                               {
                                 { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
                                 { 2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,2},
                                 { 2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,2},
                                 { 2,3,6,6,3,3,3,3,3,3,3,3,6,6,3,2},
                                 { 2,3,6,6,3,3,3,3,3,3,3,3,6,6,3,2},
                                 { 2,3,3,3,3,3,3,6,6,3,3,3,3,3,3,2},
                                 { 4,3,3,3,3,3,3,6,6,3,3,3,3,3,3,2},
                                 { 2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,5},
                                 { 2,3,3,3,3,3,3,6,6,3,3,3,3,3,3,2},
                                 { 2,3,3,3,3,3,3,6,6,3,3,3,3,3,3,2},
                                 { 2,3,6,6,3,3,3,3,3,3,3,3,6,6,3,2},
                                 { 2,3,6,6,3,3,3,3,3,3,3,3,6,6,3,2},
                                 { 2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,2},
                                 { 2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,2},
                                 { 1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1}
                               }

                            ## CRITERIA      
                            4 is always in the middle of the far left column. 5 is always in the far right column. Both cannot be placed next to a corner (1) or an inner wall (6).
                            inner walls can be connected but not diagonally. inner walls placed in a small 2x2 sqaure will create a pillar. anything placed bigger than this will not work.
                            The room shapes and size can be anything with corners, such as L shaped or + shapes.
                            Rooms should follow a symmetrical pattern.

                            ## Format the response using the C# 2D array format. Do not include any spaces in the array.
                            ";
        StartCoroutine(SendOpenAIRequest(userInput));
    }

    public IEnumerator SendOpenAIRequest(string userInput)
    {
        string url = "https://api.openai.com/v1/chat/completions";
        string escapeUserInput = userInput
        .Replace("\\", "\\\\")
        .Replace("\"", "\\\"")
        .Replace("\n", "\\n")
        .Replace("\r", "\\r")
        .Replace("\t", "\\t");

        string json = $@"
        {{  
            ""model"": ""gpt-4.1-mini"",
            ""messages"": [
                {{""role"": ""user"", ""content"": ""{(escapeUserInput)}""}}
            ]
        }} ";

        byte[] body = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer API-KEY");
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string responseText = request.downloadHandler.text;

        // Extract the content from the response
        int contentStart = responseText.IndexOf("\"content\": \"") + 12;
        int contentEnd = responseText.IndexOf("\",\n        \"refusal\"");

        string contentJson = responseText.Substring(contentStart, contentEnd - contentStart);
        contentJson = contentJson.Replace("\\n", "\n").Replace("\\\"", "\"");

        HandleAIRequest(contentJson);
    }

    public void HandleAIRequest(string aiResponse)
    {
        Debug.Log(aiResponse);

        try
        {
            // Remove all whitespace (newlines, tabs, spaces)
            string cleaned = System.Text.RegularExpressions.Regex.Replace(aiResponse, @"\s+", "");

            // Remove outer braces
            cleaned = cleaned.Trim('{', '}');

            // Split by inner array closing and opening: },{
            string[] rowStrings = cleaned.Split(new string[] { "},{" }, System.StringSplitOptions.None);

            int height = rowStrings.Length;
            int width = rowStrings[0].Split(',').Length;

            int[,] layoutArray = new int[height, width];

            for (int y = 0; y < height; y++)
            {
                string[] values = rowStrings[y].Split(',');

                for (int x = 0; x < width && x < values.Length; x++)
                {
                    if (int.TryParse(values[x], out int parsedValue))
                    {
                        layoutArray[y, x] = parsedValue;
                    }
                }
            }

            Debug.Log("Layout parsed successfully: " + height + " x " + width);
            roomGen.CreateRoomPreview(layoutArray);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to parse layout: " + ex.Message);
        }
    }

}
