using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DockManager : MonoBehaviour
{
    public static DockManager Instance;
    public Dock[] testers;
    public GameObject projectTemplate;

    public GameObject environment;

    public AudioClip hover;
    public AudioClip selected;
    public AudioSource source;

    bool played;
    bool launched;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach(Dock tester in testers)
        {
            Project project = Instantiate(projectTemplate, transform).GetComponent<Project>();
            project.SetUpProject(tester.name, tester.sky);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        launched = false;
        played = false;
    }

    public void Interacting()
    {
        environment.SetActive(false);

        if(!played)
        {
            played = true;
            source.PlayOneShot(hover);
        }
    }

    public void StoppedInteracting()
    {
        environment.SetActive(true);
        played = false;
    }

    public void Selected()
    {
        if(!launched)
        {
            launched = true;
            source.PlayOneShot(selected);
        }
    }
}

[System.Serializable]
public class Dock
{
    public string name;
    public Material sky;
}
