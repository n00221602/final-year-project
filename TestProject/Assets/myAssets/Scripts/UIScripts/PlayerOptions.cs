using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerOptions : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenuUI;
    [SerializeField] private GameObject celShaderOptionsUI;
    [SerializeField] private UniversalRendererData rendererData;

    [SerializeField] private bool outlineEnabled = true;
    [SerializeField] private bool celShaderEnabled = true;
    [SerializeField] private bool greyscaleEnabled = false;
    [SerializeField] private bool noirEnabled = false;
    [SerializeField] private bool roomMaskEnabled = true;

    [SerializeField] private Toggle celShaderOnlyToggle;
    [SerializeField] private Toggle hatchToggle;
    [SerializeField] private Toggle halftoneToggle;

    public GameObject playerNoirLight;

    private bool isOptionsOpen = false;
    private float previousTimeScale = 1f;

    private void Start()
    {
        optionsMenuUI.SetActive(false);

        SetFeatureActive("Outline", outlineEnabled);
        SetFeatureActive("CelShader", celShaderEnabled);
        SetFeatureActive("Greyscale", greyscaleEnabled);
        SetFeatureActive("Noir", noirEnabled);
        SetFeatureActive("RoomMask", roomMaskEnabled);

        celShaderOptionsUI.SetActive(false);

        //Ensure time scale is normal when starting the game
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptionsMenu();
        }

    }

    private void ToggleOptionsMenu()
    {

        if (isOptionsOpen)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            optionsMenuUI.SetActive(true);
            isOptionsOpen = false;
        }
        else
        {
            Time.timeScale = previousTimeScale;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            optionsMenuUI.SetActive(false);
            isOptionsOpen = true;
        }
    }

    public void ToggleOutline()
    {
        if (outlineEnabled)
        {
            outlineEnabled = false;
            SetFeatureActive("EdgeDetection", false);
        }
        else
        {
            outlineEnabled = true;
            SetFeatureActive("EdgeDetection", true);
        }
    }

    public void ToggleNoir()
    {
        if (noirEnabled)
        {
            noirEnabled = false;
            SetFeatureActive("NoirMask", false);
            SetFeatureActive("Greyscale", false);
            playerNoirLight.SetActive(false);
        }
        else
        {
            noirEnabled = true;
            SetFeatureActive("NoirMask", true);
            SetFeatureActive("Greyscale", true);
            playerNoirLight.SetActive(true);
        }
    }

    public void ToggleRoomMask()
    {
        if (roomMaskEnabled)
        {
            roomMaskEnabled = false;
            SetFeatureActive("RoomMask", false);
        }
        else
        {
            roomMaskEnabled = true;
            SetFeatureActive("RoomMask", true);
        }
    }

    public void ToggleCelShader()
    {
        if (celShaderEnabled)
        {
            celShaderEnabled = false;
            SetFeatureActive("CelShader", false);
            celShaderOptionsUI.SetActive(false);
        }
        else
        {
            celShaderEnabled = true;
            SetFeatureActive("CelShader", true);
            celShaderOptionsUI.SetActive(true);
        }
    }

    public void ToggleCelShaderOnly()
    {
        if (!celShaderOnlyToggle.isOn)
            return;

        Shader.SetGlobalInt("_CelShaderToggle", 1);
    }

    public void ToggleHatch()
    {
        if (!hatchToggle.isOn)
            return;

        Shader.SetGlobalInt("_CelShaderToggle", 0);
        Shader.SetGlobalInt("_CelShadowToggle", 1);
    }

    public void ToggleHalftone()
    {
        if (!halftoneToggle.isOn)
            return;

        Shader.SetGlobalInt("_CelShaderToggle", 0);
        Shader.SetGlobalInt("_CelShadowToggle", 0);
    }

    public void ResetToDefault()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
        //Reset all settings to default values
        outlineEnabled = true;
        celShaderEnabled = true;
        greyscaleEnabled = false;
        noirEnabled = false;
        roomMaskEnabled = true;
        SetFeatureActive("EdgeDetection", outlineEnabled);
        SetFeatureActive("CelShader", celShaderEnabled);
        SetFeatureActive("Greyscale", greyscaleEnabled);
        SetFeatureActive("NoirMask", noirEnabled);
        SetFeatureActive("RoomMask", roomMaskEnabled);
        celShaderOptionsUI.SetActive(celShaderEnabled);
        celShaderOnlyToggle.isOn = false;
        hatchToggle.isOn = false;
        halftoneToggle.isOn = false;
        Shader.SetGlobalInt("_CelShaderToggle", 0);
        Shader.SetGlobalInt("_CelShadowToggle", 0);
        playerNoirLight.SetActive(false);


    }

    private void SetFeatureActive(string featureName, bool isActive)
    {
        if (rendererData == null)
            return;

        var feature = rendererData.rendererFeatures.Find(f => f.name == featureName);
        if (feature != null)
            feature.SetActive(isActive);
    }
}