using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;
using System.Threading;
using UnityEngine.SceneManagement;

public class Robot : MonoBehaviour, Observer
{
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
    [SerializeField] GameObject winUI;
    [SerializeField] GameObject loseUI;
    [SerializeField] string nextLevel;
    public int[] results = { 0, 0, 0 };
    bool isHolding = false;
    bool isHeld = false;
    bool isMoving = false;
    bool hasMoved = false;
    bool isSlicing = false;
    public bool noCubes = false;
    public int nextCube = 0;
    public int nextCubeColor = 0;
    Environment env;
    Thread thread;
    public int[] called = { 0,0,0};
    int nextSlices;

    bool running = false;
    bool ran = false;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        if (!running && Input.GetKeyDown(KeyCode.F)) running = true;
        if (running && !ran)
        {
            ran = true;
            button.GetComponent<MeshRenderer>().material = materials.materials[1];
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
            this.transform.position = dropPos.GetChild(Random.Range(0,10)).position;
            hasMoved = true;
        }

        if (isSlicing)
        {
             
            float d = 0f;
            for (int j = 1; j<=nextSlices; j++)
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
            called[nextCubeColor]++;
            StartCoroutine(DestroyCube(spawnedCubes[nextCube], d+spawnDelay));
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

    void Run()
    {
        
        Lexer lexer = new Lexer(useTestInput ? testInput : input.text);
        Program prog = new Parser(lexer).ParseProgram();
        env.Set("n",new Integer { value = spawnedCubes.Count });
        env.Set("cube", new Integer { value = nextCubeColor });
        Evaluator.Eval(prog, env, this);
    }
    void Observer.OnLoopIterationEnd()
    {
        nextSlices = (int)((Integer)env.Get("slices")).value;
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
            env.Set("cube", new Integer { value = nextCubeColor });
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
        spawner.cubesPerColor[nextCubeColor]--;
        if (nextCube+1 < spawnedCubes.Count)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (spawnedCubes[nextCube+1].gameObject.GetComponent<MeshRenderer>().material.color.Equals(materials.materials[i].color))
                {
                    nextCubeColor = i;
                }
            }
        }
        thread.Interrupt();
    }

    void CheckResult()
    {
        for (int i = 0; i < 3; i++)
        {
            if (results[i] != slices[i])
            {
                Lose();
                return;
            }
        }
        Win();
    }

    void Win()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        winUI.transform.root.gameObject.SetActive(true);
        winUI.SetActive(true);
        loseUI.SetActive(false);
    }

    void Lose()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        loseUI.transform.root.gameObject.SetActive(true);
        loseUI.SetActive(true);
        winUI.SetActive(false);
    }

    void CheckRemainingCubes()
    {

        if (FindObjectsOfType<ColoredCube>().Length == 0)
        {
            CheckResult();
        }
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(nextLevel);
    }

    void Observer.OnLetStatement()
    {
        return;
    }

    void Observer.OnBlockEnd()
    {
        return;
    }

    public void OnLoopEnd()
    {
        return;
    }
}
