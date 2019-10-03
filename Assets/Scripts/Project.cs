using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Project : MonoBehaviour
{
    string projectName;
    Material defaultSky;
    Material skyboxMaterial;
    int index;

    TextMeshProUGUI text;

    void Start()
    {
        defaultSky = RenderSettings.skybox;
    }

    public void SetUpProject(string _name, Material _sky, int _index)
    {
        projectName = _name;
        skyboxMaterial = _sky;
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = projectName;
        index = _index;
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
