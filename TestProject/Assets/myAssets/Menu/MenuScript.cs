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
    public TMP_InputField aiKeyInput;
    public GameObject gridPrefab;
    public GameObject loadingText;

    public Transform layoutHolder;
    public GameObject savedLayoutPrefab;
    public RawImage layoutRawImage;

    public GridLayoutGroup gridLayoutGroup;

    [HideInInspector] public int[,] userLayout;
    public static List<int[,]> userLayoutList;
    public static List<Texture2D> layoutTextureList;

    string widthInput;
    string heightInput;

    float squareInputAmount;
    int currentSquareAmount;
    float totalSquaresWidth;
    float totalSquaresHeight;

    [HideInInspector] public UnityEvent onUserSubmit;

    public RoomGen roomGen;
    int aiLayoutArrayWidth;
    int aiLayoutArrayHeight;
    int[,] aiLayoutArray;

    void Start()
    {
        //Make sure only the menu UI is active when the scene first starts
        menuUI.SetActive(true);
        generateUI.SetActive(false);
        viewLayoutsUI.SetActive(false);

        if (userLayoutList == null)
        {
            userLayoutList = new List<int[,]>();
        }

        if (layoutTextureList == null)
        {
            layoutTextureList = new List<Texture2D>();
        }

        if (userLayoutList.Count > 0)
        {
            RefreshLayoutView();
        }
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
            }
        }

        roomGen.CreateRoomPreview(userLayout);

    }

    public void StoreGridSize(TMP_InputField inputField)
    {
        if (inputField == widthInputField)
        {
            widthInput = inputField.text;
        }
        else if (inputField == heightInputField)
        {
            heightInput = inputField.text;
        }
    }

    public void SaveLayout()
    {
        if (userLayout != null)
        {
            userLayoutList.Add(userLayout);
            Texture sourceTexture = layoutRawImage.texture;
            Texture2D texture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);
            Graphics.CopyTexture(sourceTexture, texture);

            layoutTextureList.Add(texture);

            GameObject currentLayout = savedLayoutPrefab;
            currentLayout.GetComponent<RawImage>().texture = texture;

            Instantiate(currentLayout, layoutHolder);
        }
    }

    public void RemoveLayout()
    {
        //Find all active/toggled layouts
        Toggle[] toggles = layoutHolder.GetComponentsInChildren<Toggle>();
        List<int> removalList = new List<int>();

        foreach (Toggle toggle in toggles)
        {
            //If toggled, add to removal list
            if (toggle.isOn)
            {
                int index = toggle.transform.GetSiblingIndex();
                if (index >= 0 && index < userLayoutList.Count)
                {
                    removalList.Add(index);
                }
            }
        }

        //Removes from list, starting from the end.
        for (int i = removalList.Count - 1; i >= 0; i--)
        {
            userLayoutList.RemoveAt(removalList[i]);
            layoutTextureList.RemoveAt(removalList[i]);
        }

        //Destroy all toggled gameobjects
        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
            {
                Destroy(toggle.gameObject);
            }
        }
    }

    public void RefreshLayoutView()
    {
        //Repopulate layout view based on userLayoutList
        for (int i = 0; i < userLayoutList.Count; i++)
        {
            Texture2D texture = layoutTextureList[i];
            GameObject currentLayout = savedLayoutPrefab;
            currentLayout.GetComponent<RawImage>().texture = texture;
            Instantiate(currentLayout, layoutHolder);
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

        //Adjust the cell sizes depending on the total number of squares in the grid.
        gridLayoutGroup.cellSize = new Vector2(gridHolderWidth / totalSquaresWidth, gridHolderHeight / totalSquaresHeight);

    }

    public void OnAIButton()
    {
        loadingText.SetActive(true);
        string userInput = @"Generate ONLY a C# 2D array in this exact format with no other text:

                            LEGEND: 0=empty, 1=corner, 2=wall, 3=floor, 4=entry, 5=exit, 6=inner_wall

                            RULES:
                            - Size: 12x12 to 25x25
                            - Entry(4): middle of column 0, not adjacent to 1 or 6
                            - Exit(5): middle of last column, not adjacent to 1 or 6
                            - Empty(0): outside rooms only
                            - Corners(1): perimeter corners only
                            - Walls(2): perimeter only
                            - Floor(3): inside rooms only
                            - Inner walls(6): 2x2 pillars maximum, not diagonal
                            - Symmetrical pattern

                            Example array format (no spaces):
                            {{1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},{2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,2},{2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,2},{2,3,6,6,3,3,3,3,3,3,3,3,6,6,3,2},{2,3,6,6,3,3,3,3,3,3,3,3,6,6,3,2},{2,3,3,3,3,3,3,6,6,3,3,3,3,3,3,2},{4,3,3,3,3,3,3,6,6,3,3,3,3,3,3,2},{2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,5},{2,3,3,3,3,3,3,6,6,3,3,3,3,3,3,2},{2,3,3,3,3,3,3,6,6,3,3,3,3,3,3,2},{2,3,6,6,3,3,3,3,3,3,3,3,6,6,3,2},{2,3,6,6,3,3,3,3,3,3,3,3,6,6,3,2},{2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,2},{2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,2},{1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1}}";
        StartCoroutine(SendOpenAIRequest(userInput));
    }

    public IEnumerator SendOpenAIRequest(string userInput)
    {
        string url = "https://api.openai.com/v1/chat/completions";
        string apiKey = aiKeyInput.text;

        //User input is tweaked to remove special characters in order to be compatible with JSON.
        string formattedUserInput = userInput
        .Replace("\\", "\\\\")
        .Replace("\"", "\\\"")
        .Replace("\n", "\\n")
        .Replace("\r", "\\r")
        .Replace("\t", "\\t");

        //User input is formatted into a valid JSON input.
        string json = $@"
        {{  
            ""model"": ""gpt-4o-mini"",
            ""messages"": [
                {{""role"": ""user"", ""content"": ""{(formattedUserInput)}""}}
            ]
        }} ";

        byte[] body = Encoding.UTF8.GetBytes(json);

        //Web request is created and sent to the AI model.
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string responseText = request.downloadHandler.text;

        //Outputted response is parsed to extract the content field which contains the layout.
        int contentStart = responseText.IndexOf("\"content\": \"") + 12;
        int contentEnd = responseText.IndexOf("\",\n        \"refusal\"");

        string contentJson = responseText.Substring(contentStart, contentEnd - contentStart);
        contentJson = contentJson.Replace("\\n", "\n").Replace("\\\"", "\"");

        HandleAIRequest(contentJson);
    }

    public void HandleAIRequest(string aiResponse)
    {
        try
        {
            //Remove any potential spacing from the AI response.
            string cleaned = System.Text.RegularExpressions.Regex.Replace(aiResponse, @"\s+", "");

            //Find the outer most brances that are containing the 2D array.
            int firstBrace = cleaned.IndexOf('{');
            int lastBrace = cleaned.LastIndexOf('}');

            if (firstBrace == -1 || lastBrace == -1 || firstBrace >= lastBrace)
            {
                Debug.LogError("Invalid array format");
                return;
            }

            //Clean string for extracting the 2D array between the outer braces.
            string arrayContent = cleaned.Substring(firstBrace + 1, lastBrace - firstBrace - 1);

            //Trim braces so only the 2D array content remains.
            arrayContent = arrayContent.TrimStart('{').TrimEnd('}');

            //Split the content into individual rows based on the "{}" braces
            string[] rowStrings = arrayContent.Split(new string[] { "},{" }, System.StringSplitOptions.None);

            aiLayoutArrayHeight = rowStrings.Length;
            aiLayoutArrayWidth = rowStrings[0].Split(',').Length;

            aiLayoutArray = new int[aiLayoutArrayHeight, aiLayoutArrayWidth];

            for (int y = 0; y < aiLayoutArrayHeight; y++)
            {
                string[] values = rowStrings[y].Split(',');

                for (int x = 0; x < aiLayoutArrayWidth && x < values.Length; x++)
                {
                    if (int.TryParse(values[x], out int parsedValue))
                    {
                        aiLayoutArray[y, x] = parsedValue;
                    }
                }
            }

            Debug.Log("Layout parsed successfully: " + aiLayoutArrayHeight + " x " + aiLayoutArrayWidth);

            //Set the grid width and height to match the AI layout.
            widthInput = aiLayoutArrayWidth.ToString();
            heightInput = aiLayoutArrayHeight.ToString();

            //Assign the grid size to match the created layout.
            totalSquaresWidth = aiLayoutArrayWidth;
            totalSquaresHeight = aiLayoutArrayHeight;
            GridHandler();

            //Update grid input fields with AI layout values
            int squareIndex = 0;
            for (int y = 0; y < aiLayoutArrayHeight; y++)
            {
                for (int x = 0; x < aiLayoutArrayWidth; x++)
                {
                    TMP_InputField inputField = gridHolder.GetChild(squareIndex).GetComponentInChildren<TMP_InputField>();
                    inputField.text = aiLayoutArray[y, x].ToString();
                    squareIndex++;
                }
            }

            //Call OnSubmitButton to process and save the layout
            OnSubmitButton();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to parse layout: " + ex.Message);
        }

        loadingText.SetActive(false);
    }

}
