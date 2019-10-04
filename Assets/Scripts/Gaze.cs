﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OuterRimStudios.Utilities;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Gaze : MonoBehaviour
{
    [Space, Header("Look Interaction")]
    public float lookTime;
    public float lookRadius;
    public LayerMask interactionLayer;

    public Image circle;

    float time;
    float pointerTime;

    Project lastProject;

    bool waiting;

    public void Start()
    {
        ResetTime();
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
        waiting = true;
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5);
        waiting = false;
    }

    public void Update()
    {
        if (waiting) return;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, lookRadius, out hit, 1000f, interactionLayer))
        {
            lastProject = hit.transform.GetComponent<Project>();
            lastProject.Interact();

            DockManager.Instance.Interacting();

            MathUtilities.Timer(ref time);
            pointerTime = MathUtilities.MapValue(0, lookTime, 1, 0, time);
            Debug.Log("Pointer Time: " + pointerTime);
            Debug.Log("Time: " + time);
            if (time <= 0)
            {
                DockManager.Instance.Selected();
                lastProject.Select();
            }
        }
        else
        {
            if (lastProject)
            {
                lastProject.StopInteraction();
                lastProject = null;
            }

            DockManager.Instance.StoppedInteracting();
            ResetTime();
        }

        if (circle.fillAmount != pointerTime)
            circle.fillAmount = Mathf.MoveTowards(circle.fillAmount, pointerTime, 1);
        else
            circle.fillAmount = pointerTime;
    }

    void ResetTime()
    {
        time = lookTime;
        pointerTime = 0;
    }
}

