﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    public Shader[] shaders;
    private int currentShader;

    public static ShaderManager Instance { get; private set; }

    public delegate void StateAction();
    public event StateAction StateChange;

    public Shader CurrentShader { get { return shaders[currentShader]; } }

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

        currentShader = 0;
	}
	
	void Update ()
    {
		if (Input.GetButtonDown("Shader"))
        {
            if (++currentShader >= shaders.Length)
                currentShader = 0;

            UpdateAllShaders();
        }
	}

    void UpdateAllShaders ()
    {
        if (StateChange != null)
        {
            StateChange();
        }

#if UNITY_EDITOR
        Debug.Log(shaders[currentShader].name);
#endif
    }
}
