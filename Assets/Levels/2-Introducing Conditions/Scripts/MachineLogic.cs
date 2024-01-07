using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInterpreter;
using UnityEngine.SceneManagement;

public class MachineLogic : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI[] cubesColors;
    [SerializeField] ColoredCubeSpawner spawner;
    [SerializeField] int[] slicesPerColor = new int[3];
    public int[] slices = new int[3];
    [SerializeField] MaterialIndexing materials;
    [SerializeField] Transform cubePosition;
    [SerializeField] Transform defaultPos;
    [SerializeField] TMPro.TMP_InputField input;
    [SerializeField] GameObject smallCube;
    [SerializeField] Transform smallCubeSpawn;
    [SerializeField] GameObject winUI;
    [SerializeField] GameObject loseUI;
    public int[] results = new int[3];
    bool rotating = false;
    float angle = 0f;
    bool noCubes = false;
    Environment env;
    [SerializeField] string nextLevel;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        env = new Environment();
        int[] spawnedCubes = spawner.cubesPerColor;
        for (int i =0; i<3; i++)
        {
            slicesPerColor[i] = Random.Range(3, 10);
            slices[i] = spawnedCubes[i] * slicesPerColor[i];
            cubesColors[i].text = slicesPerColor[i].ToString();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
        if (rotating)
        {
            cubePosition.rotation = Quaternion.Euler(new Vector3(angle,angle,angle));
            angle += 50 * Time.deltaTime;
        }

        if (noCubes)
        {
            CheckRemainingCubes();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ColoredCube" && input.text!=null && input.text != "")
        {
            other.transform.parent = cubePosition;
            other.gameObject.transform.localPosition = Vector3.zero;
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            other.gameObject.GetComponent<Collider>().enabled = false;
            for (int i=0; i<=2; i++)
            {
                Debug.LogWarning(other.gameObject.GetComponent<MeshRenderer>().material.color + "\t" + materials.materials[i].color);
                if (other.gameObject.GetComponent<MeshRenderer>().material.color.Equals(materials.materials[i].color))
                {
                    Debug.LogWarning("I =   " + i);
                   TestCase(other.gameObject,i);
                }
            }
        }
        else if (other.gameObject.tag == "ColoredCube" && (input.text == null || input.text == ""))
        {
            other.gameObject.transform.parent = null;
            other.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            other.gameObject.GetComponent<Collider>().enabled = true;
            other.gameObject.transform.position = defaultPos.position;
        }
        else Debug.LogError(other.gameObject);
    }

    public void TestCase(GameObject cube, int materialIndex)
    {
        rotating = true;
        Debug.LogWarning("TESTCASE");
        Lexer lexer = new Lexer(input.text);
        Program prog = new Parser(lexer).ParseProgram();
        env = new Environment();
        env.store.Add("cube",new Integer{value=materialIndex});
        Evaluator.Eval(prog,env);
        Integer slices = (Integer)env.Get("slices");
        if (slices != null)
        {
            float d = 0f;
            for (int i=1; i<=slices.value; i++)
            {
                d += 0.2f;
                StartCoroutine(SmallCubeSpawn(materialIndex,d));
            }
            spawner.cubesPerColor[materialIndex]--;
            StartCoroutine(DestroyCube(cube,d));
            
        }
        else
        {
            Debug.LogWarning("ELSE");
            cube.transform.parent = null;
            cube.GetComponent<Rigidbody>().isKinematic = false;
            cube.GetComponent<Collider>().enabled = true;
            cube.transform.position = defaultPos.position;
            
        }
    }

    public IEnumerator SmallCubeSpawn(int material, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject smallcube = Instantiate(smallCube, smallCubeSpawn.GetChild(Random.Range(0, 10)).transform.position, Quaternion.Euler(Random.Range(0f,90f), Random.Range(0f, 90f), Random.Range(0f, 90f)));
        smallcube.GetComponent<MeshRenderer>().material = materials.materials[material];
        results[material]++;
    }

    public IEnumerator DestroyCube(GameObject cube,float delay)
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


    void CheckResult()
    {
        for(int i=0; i<3; i++)
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
}
