using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredCubeSpawner : MonoBehaviour
{
    [SerializeField] MaterialIndexing materials;
    [SerializeField] public int[] cubesPerColor = new int[3];
    [SerializeField] GameObject prefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float delay = 0f;
    public List<GameObject> spawnedCubes;
    public bool isSpawning = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator CubeSpawn(int material, float delay, bool useList = false)
    {
        yield return new WaitForSeconds(delay);
        GameObject cube = Instantiate(prefab, spawnPoint.transform.position, Quaternion.Euler(Random.Range(0f, 90f), Random.Range(0f, 90f), Random.Range(0f, 90f)));
        cube.GetComponent<MeshRenderer>().material = materials.materials[material];
        if (useList)
        {
            spawnedCubes.Add(cube);
        }
    }

    public IEnumerator DoneSpawning(float delay)
    {
        yield return new WaitForSeconds(delay);
        isSpawning = false;
    }

    public void StartSpawning(bool useList = false)
    {
        isSpawning = true;

        float d = 0f;

        for (int j = 0; j < cubesPerColor[0]; j++)
        {
            d += delay;
            StartCoroutine(CubeSpawn(0, d,useList));
        }

        d = 0.1f;

        for (int j = 0; j < cubesPerColor[1]; j++)
        {
            d += delay;
            StartCoroutine(CubeSpawn(1, d, useList));
        }

        d = 0.2f;

        for (int j = 0; j < cubesPerColor[2]; j++)
        {
            d += delay;
            StartCoroutine(CubeSpawn(2, d, useList));
        }

        StartCoroutine(DoneSpawning(d));

    }
}