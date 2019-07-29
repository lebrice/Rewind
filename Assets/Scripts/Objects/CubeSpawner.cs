using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public Transform CubePrefab;
    public Vector3 CubeSpawnPosition = new Vector3(-10, 0, 10);
    public int CubeCount = 27;
    // Start is called before the first frame update
    void Start()
    {
        SpawnCubePile();
    }


    private void SpawnCubePile()
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
                    Transform cube = Instantiate(CubePrefab, CubeSpawnPosition + relativePosition, Quaternion.identity);
                    if (numCubes == CubeCount)
                        return;
                }
            }
        }
    }
}
