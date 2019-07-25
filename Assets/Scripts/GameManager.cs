﻿using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SharedControls Controls;
    public RewindController TimeController { get; private set; }
    //Static instance of GameManager which allows it to be accessed by any other script.
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // setup and ensure singleton.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitGame();
        }
    }

    void InitGame()
    {

    }

    public Transform CubePrefab;
    public Vector3 CubeSpawnPosition = new Vector3(-10, 0, 10);
    public int CubeCount = 27;

    private void SpawnCubes()
    {
        int cubesPerAxis = Mathf.CeilToInt(Mathf.Pow(CubeCount, 1f / 3));
        int numCubes = 0;

        for (int i = 0; i < cubesPerAxis; i++)
        {
            for (int j = 0; j < cubesPerAxis; j++)
            {
                for (int k = 0; k < cubesPerAxis; k++, numCubes++)
                {
                    var relativePosition = new Vector3(i, j, k);
                    relativePosition.Scale(CubePrefab.lossyScale);
                    Instantiate(CubePrefab, CubeSpawnPosition + relativePosition, Quaternion.identity);
                    if (numCubes == CubeCount)
                        return;
                }
            }
        }
    }
}