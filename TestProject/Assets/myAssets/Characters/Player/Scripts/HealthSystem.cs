using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{

    public Image health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health.fillAmount = 0.6f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
