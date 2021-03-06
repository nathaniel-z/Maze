﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PongSwitchManager : MonoBehaviour
{

    public static PongSwitchManager Instance { get; private set; }

    private List<GameObject> rootObjects;

    private const string PONG = "Pong";


    void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rootObjects = new List<GameObject>();
        SceneManager.GetActiveScene().GetRootGameObjects(rootObjects);
    }

    public void EnablePong ()
    {
        SceneManager.LoadSceneAsync(PONG, LoadSceneMode.Additive);

        foreach (GameObject obj in rootObjects)
        {
            obj.SetActive(false);
        }
    }

    public void DisablePong ()
    {
        SceneManager.UnloadSceneAsync(PONG);

        foreach (GameObject obj in rootObjects)
        {
            obj.SetActive(true);
        }
    }
}
