using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;
using System.Threading;

public class Robot : MonoBehaviour, Observer
{
    [SerializeField] long cubeTest = 0;
    [SerializeField] bool useTestInput;
    [TextArea(3, 10)] [SerializeField] string testInput;
    [SerializeField] MaterialIndexing materials;
    [SerializeField] GameObject button;
    [SerializeField] TMPro.TextMeshProUGUI input;
    [SerializeField] TMPro.TextMeshProUGUI[] cubesColors;
    public int[] slices = new int[3];
    [SerializeField] ColoredCubeSpawner spawner;
    [SerializeField] int[] slicesPerColor = new int[3];
    public List<GameObject> spawnedCubes = null;
    [SerializeField] Transform dropPos;
    [SerializeField] GameObject smallCubePrefab;
    [SerializeField] float spawnDelay;
    int[] results = { 0, 0, 0 };
    bool isPicking = false;
    bool isHolding = false;
    bool isHeld = false;
    bool isMoving = false;
    bool hasMoved = false;
    bool isSlicing = false;
    bool noCubes = false;
    int nextCube = 0;
    Environment env;
    Thread thread;



    bool running = false;
    bool ran = false;
    // Start is called before the first frame update
    void Start()
    {
        thread = new Thread(Run);
        env = new Environment();
        spawner.StartSpawning(true);
        for (int i = 0; i<3; i++)
        {
            slicesPerColor[i] = Random.Range(3, 10);
            cubesColors[i].text = slicesPerColor[i].ToString();
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        if (env.Get("test") != null) cubeTest = ((Integer)env.Get("test")).value;

        if (!running && Input.GetKeyDown(KeyCode.Q)) running = true;
        if (running && !ran)
        {
            ran = true;
            thread.Start();
        }


        if (spawnedCubes == null || spawnedCubes.Count == 0)
        {
            spawnedCubes = spawner.isSpawning ? null : spawner.spawnedCubes;

            if (spawnedCubes.Count > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    slices[i] = spawner.cubesPerColor[i] * slicesPerColor[i];
                }
                isPicking = true;
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
            this.transform.position = dropPos.GetChild(Random.Range(0,10)).position;
            hasMoved = true;
        }

        if (isSlicing)
        {
            int i = 0;
            for (; i <= 2; i++)
            {
                if (spawnedCubes[nextCube].gameObject.GetComponent<MeshRenderer>().material.color.Equals(materials.materials[i].color))
                {
                    break;
                }
            }

            int slices = (int)((Integer)env.Get("slices")).value;
            float d = 0f;
            for (int j = 1; j<=slices; j++)
            {
                d += spawnDelay;
                StartCoroutine(SmallCubeSpawn(i, d));
            }
            isHolding = false;
            isMoving = false;
            isHeld = false;
            hasMoved = false;
            isSlicing = false;
            StartCoroutine(DestroyCube(spawnedCubes[nextCube], d));
        }
    }

    void Run()
    {
        
        Lexer lexer = new Lexer(useTestInput ? testInput : input.text);
        Program prog = new Parser(lexer).ParseProgram();
        Evaluator.Eval(prog, env, this);
    }
    void Observer.OnLoopIterationEnd()
    {
        isHolding = true;
        Thread.Sleep(100);
        isMoving = true;
        Thread.Sleep(100);

        isSlicing = true;

        //Thread.Sleep(100);

        try
        {
            Thread.Sleep(Timeout.Infinite);
        }
        catch (ThreadInterruptedException)
        {
            nextCube++;
        }
        
        
    }

    public IEnumerator SmallCubeSpawn(int material, float delay)
    {
        yield return new WaitForSeconds(delay);
        this.transform.position = dropPos.GetChild(Random.Range(0, 10)).position;
        GameObject smallcube = Instantiate(smallCubePrefab, this.transform.position, Quaternion.Euler(Random.Range(0f, 90f), Random.Range(0f, 90f), Random.Range(0f, 90f)));
        smallcube.GetComponent<MeshRenderer>().material = materials.materials[material];
        results[material]++;
    }

    public IEnumerator DestroyCube(GameObject cube, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(cube);
        CubeDestroyPost();
    }

    void CubeDestroyPost()
    {
        thread.Interrupt();
        for (int i = 0; i < 3; i++)
        {
            if (spawner.cubesPerColor[i] != 0)
            {
                return;
            }
        }
        noCubes = true;
    }
}
