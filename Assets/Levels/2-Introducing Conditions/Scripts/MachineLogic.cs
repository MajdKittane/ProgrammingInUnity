using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;
using System.Threading;

public class MachineLogic : AbstractPuzzle
{

    [SerializeField] ColoredCubeSpawner spawner;
    [SerializeField] Transform cubePosition;
    [SerializeField] TMPro.TextMeshProUGUI[] cubesColors;
    [SerializeField] Transform defaultPos;
    [SerializeField] GameObject smallCube;
    [SerializeField] Transform smallCubeSpawn;
    int[] slicesPerColor = new int[3];
    int[] slices = new int[3];
    int[] results = new int[3];
    bool rotating;
    bool noCubes = false;
    float angle = 0f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        spawner.StartSpawning();
        int[] spawnedCubes = spawner.cubesPerColor;
        for (int i = 0; i < 3; i++)
        {
            cubesColors[i].transform.parent.parent.gameObject.GetComponent<MeshRenderer>().material.color = spawner.colors[i];
            slicesPerColor[i] = Random.Range(3, 10);
            slices[i] = spawnedCubes[i] * slicesPerColor[i];
            cubesColors[i].text = slicesPerColor[i].ToString();
        }

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (rotating)
        {
            cubePosition.rotation = Quaternion.Euler(new Vector3(angle, angle, angle));
            angle += 90 * Time.deltaTime;
        }

        if (noCubes)
        {
            CheckRemainingCubes();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ColoredCube>() != null && levelManager.codeSaved)
        {
            other.transform.parent = cubePosition;
            other.gameObject.transform.localPosition = Vector3.zero;
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            other.gameObject.GetComponent<Collider>().enabled = false;
            for (int i = 0; i <= 2; i++)
            {
                if (other.gameObject.GetComponent<MeshRenderer>().material.color.Equals(spawner.colors[i]))
                {
                    TestCase(other.gameObject, i);
                }
            }
        }
        else if (other.gameObject.GetComponent<ColoredCube>() != null && !levelManager.codeSaved)
        {
            other.gameObject.transform.parent = null;
            other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            other.gameObject.GetComponent<Collider>().enabled = true;
            other.gameObject.transform.position = defaultPos.position;
        }
    }

    public void TestCase(GameObject cube, int colorIndex)
    {
        rotating = true;
        env.store["cube"] = new Integer { value = colorIndex };

        Run();

        Integer slices = (Integer)env.Get("slices");
        if (slices != null)
        {
            float d = 0f;
            for (int i = 1; i <= slices.value; i++)
            {
                d += 0.2f;
                StartCoroutine(SmallCubeSpawn(colorIndex, d));
            }
            spawner.cubesPerColor[colorIndex]--;
            StartCoroutine(DestroyCube(cube, d));

        }
        else
        {
            cube.transform.parent = null;
            cube.GetComponent<Rigidbody>().isKinematic = false;
            cube.GetComponent<Collider>().enabled = true;
            cube.transform.position = defaultPos.position;

        }
    }

    public IEnumerator SmallCubeSpawn(int colorIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject smallcube = Instantiate(smallCube, smallCubeSpawn.GetChild(Random.Range(0, 10)).transform.position, Quaternion.Euler(Random.Range(0f, 90f), Random.Range(0f, 90f), Random.Range(0f, 90f)));
        smallcube.GetComponent<MeshRenderer>().material.color = spawner.colors[colorIndex];
        results[colorIndex]++;
    }

    public IEnumerator DestroyCube(GameObject cube, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(cube);
        CubeDestroyPost();
    }

    void CubeDestroyPost()
    {
        for (int i = 0; i < 3; i++)
        {
            if (spawner.cubesPerColor[i] != 0)
            {
                return;
            }
        }
        noCubes = true;
    }


    public override void CheckResult()
    {
        for (int i = 0; i < 3; i++)
        {
            if (results[i] != slices[i])
            {
                levelManager.Lose();
                return;
            }
        }
        levelManager.Win();
    }

    void CheckRemainingCubes()
    {

        if (FindObjectsOfType<ColoredCube>().Length == 0)
        {
            CheckResult();
        }
    }

    public override void Action()
    {
        return;
    }

}
