using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;
using System.Threading;

public class Robot : AbstractPuzzle, Observer
{

    [SerializeField] GameObject button;
    [SerializeField] ColoredCubeSpawner spawner;
    [SerializeField] TMPro.TextMeshProUGUI[] cubesColors;
    [SerializeField] Transform dropPos;
    [SerializeField] float spawnDelay;
    [SerializeField] GameObject smallCubePrefab;
    List<GameObject> spawnedCubes = null;
    int[] slicesPerColor = new int[3];
    int[] slices = new int[3];
    int nextSlices;
    int nextCube = 0;
    int nextCubeColor = 0;
    bool noCubes = false;
    bool running, ran = false;
    bool isHolding, isHeld = false;
    bool isMoving, hasMoved = false;
    bool isSlicing = false;
    int[] results = { 0, 0, 0 };


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        spawner.StartSpawning(true);
        env.Set("n", new Integer { value = spawner.cubesPerColor[0]+ spawner.cubesPerColor[1]+ spawner.cubesPerColor[2] });
        env.Set("cube", new Integer { value = nextCubeColor });
        for (int i = 0; i < 3; i++)
        {
            slicesPerColor[i] = Random.Range(3, 10);
            cubesColors[i].text = slicesPerColor[i].ToString();
        }

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (running && !ran)
        {
            Debug.LogWarning("ActionCALLLED");
            ran = true;
            button.GetComponent<MeshRenderer>().material.color = Color.green;
            thread.Start();
        }

        if (spawnedCubes == null || spawnedCubes.Count == 0)
        {
            spawnedCubes = spawner.isSpawning ? null : spawner.spawnedCubes;

            if (spawnedCubes != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    slices[i] = spawner.cubesPerColor[i] * slicesPerColor[i];
                }

            }
        }



        if (isHolding && !isHeld)
        {
            GameObject cube = spawnedCubes[nextCube];
            this.transform.position = cube.transform.position - new Vector3(1f, 0f, 0f);
            cube.transform.parent = this.transform;
            cube.GetComponent<Rigidbody>().isKinematic = true;
            cube.GetComponent<Collider>().enabled = false;
            isHeld = true;
        }

        if (isMoving && !hasMoved)
        {
            this.transform.position = dropPos.GetChild(Random.Range(0, 10)).position;
            hasMoved = true;
        }

        if (isSlicing)
        {

            float d = 0f;
            for (int j = 1; j <= nextSlices; j++)
            {
                d += spawnDelay;
                Debug.LogWarning(nextCubeColor + "\t" + j);
                StartCoroutine(SmallCubeSpawn(nextCubeColor, d));
            }
            isHolding = false;
            isMoving = false;
            isHeld = false;
            hasMoved = false;
            isSlicing = false;
            StartCoroutine(DestroyCube(spawnedCubes[nextCube], d + spawnDelay));
        }

        for (int i = 0; i < 3; i++)
        {
            if (spawner.cubesPerColor[i] != 0)
            {
                return;
            }
        }
        noCubes = true;

        if (noCubes)
        {
            CheckRemainingCubes();
        }
    }

    public IEnumerator SmallCubeSpawn(int colorIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        this.transform.position = dropPos.GetChild(Random.Range(0, 10)).position;
        GameObject smallcube = Instantiate(smallCubePrefab, this.transform.position, Quaternion.Euler(Random.Range(0f, 90f), Random.Range(0f, 90f), Random.Range(0f, 90f)));
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
        spawner.cubesPerColor[nextCubeColor]--;
        if (nextCube + 1 < spawnedCubes.Count)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (spawnedCubes[nextCube + 1].gameObject.GetComponent<MeshRenderer>().material.color.Equals(spawner.colors[i]))
                {
                    nextCubeColor = i;
                }
            }
        }
        thread.Interrupt();
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

        if (!running && spawnedCubes != null && levelManager.codeSaved)
        {
            running = true;
        }
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

    public void OnBlockEnd()
    {
        return;
    }

    public void OnLetStatement()
    {
        return;
    }

    public void OnLoopEnd()
    {
        return;
    }

    public void OnLoopIterationEnd()
    {
        Debug.Log("ITERATION");
        nextSlices = (int)((Integer)env.Get("slices")).value;
        isHolding = true;
        Thread.Sleep(100);
        isMoving = true;
        Thread.Sleep(100);

        isSlicing = true;


        try
        {
            Thread.Sleep(Timeout.Infinite);
        }
        catch (ThreadInterruptedException)
        {
            nextCube++;
            env.Set("cube", new Integer { value = nextCubeColor });
        }


    }

    public void OnProgramEnd()
    {
        return;
    }
}
