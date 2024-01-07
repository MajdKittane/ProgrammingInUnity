using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredCubeSpawner : MonoBehaviour
{
    [SerializeField] MaterialIndexing materials;
    [SerializeField] public int[] cubesPerColor = new int[3];
    [SerializeField] GameObject prefab;
    [SerializeField] Transform spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        float d = 0f;
        for(int i = 0; i<3; i++)
        {
            for (int j=0; j<cubesPerColor[i]; j++)
            {
                d += 0.5f;
                StartCoroutine(CubeSpawn(i,d));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator CubeSpawn(int material, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject smallcube = Instantiate(prefab, spawnPoint.transform.position, Quaternion.Euler(Random.Range(0f, 90f), Random.Range(0f, 90f), Random.Range(0f, 90f)));
        smallcube.GetComponent<MeshRenderer>().material = materials.materials[material];
    }
}
