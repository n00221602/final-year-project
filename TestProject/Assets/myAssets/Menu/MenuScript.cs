using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject menuUI;
    public GameObject generateUI;
    public TMP_InputField inputField;
    public TMP_InputField widthInputField;
    public TMP_InputField heightInputField;

    public GridLayoutGroup gridLayoutGroup;

    string widthInput;
    string heightInput;

    //public RoomGen roomGen;

    void Start()
    {
        //Make sure only the menu UI is active when the scene first starts
        if (generateUI.activeSelf)
        {
            generateUI.SetActive(false);
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
        //This script takes in the 2D arrays created from the text input field in GenerateUI and generates a level based on the characters in the array.
        ;


        string submitedText = inputField.text;

    }

    public void StoreInput()
    {
        //store the input text from inputField as a variable. convert to to json?
        Debug.Log("Input: " + inputField.text);

    }

    public void StoreGridSize(TMP_InputField inputFieldThatChanged)
    {
        if (inputFieldThatChanged == widthInputField)
        {
            widthInput = inputFieldThatChanged.text;
            //widthInput = 
            Debug.Log("Width: " + widthInput);
        }
        else if (inputFieldThatChanged == heightInputField)
        {
            heightInput = inputFieldThatChanged.text;
            Debug.Log("Height: " + heightInput);
        }
    }




    public void GridHandler()
    {
        //gridParent.sizeDelta = new Vector2(500, 500);
        gridLayoutGroup.cellSize = new Vector2(int.Parse(widthInput) / 400, int.Parse(heightInput) / 400);
    }
}

