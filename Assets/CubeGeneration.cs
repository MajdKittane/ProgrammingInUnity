using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGeneration : MonoBehaviour
{
    public GameObject cube;
    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i<4; i++)
        {
            Instantiate(cube, new Vector3(i*1.5f,0,0),false,this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
