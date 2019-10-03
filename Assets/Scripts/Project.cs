using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Project : MonoBehaviour
{
    string projectName;

    Material defaultSky;
    Material skyboxMaterial;

    TextMeshProUGUI text;

    void Start()
    {
        defaultSky = RenderSettings.skybox;
    }

    public void SetUpProject(string _name, Material _sky)
    {
        projectName = _name;
        skyboxMaterial = _sky;
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = projectName;
    }

    public void Select()
    {

    }

    public void Interact()
    {
        RenderSettings.skybox = skyboxMaterial;
    }

    public void StopInteraction()
    {
        RenderSettings.skybox = defaultSky;
    }
}
