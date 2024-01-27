using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [HideInInspector] public GameObject objectToPickup { get; private set; }
    [HideInInspector] public GameObject pickedObject { get; private set; }
    [SerializeField] Transform pickupPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (objectToPickup != null && pickedObject == null)
            {
                pickedObject = objectToPickup;
                objectToPickup = null;
                pickedObject.GetComponent<Rigidbody>().isKinematic = true;
                pickedObject.GetComponent<Collider>().enabled = false;
                pickedObject.transform.parent = pickupPoint;
                pickedObject.transform.localPosition = Vector3.zero;
            }
            else if (pickedObject != null)
            {
                pickedObject.transform.parent = null;
                pickedObject.GetComponent<Rigidbody>().isKinematic = false;
                pickedObject.GetComponent<Collider>().enabled = true;
                pickedObject = null;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<ColoredCube>())
        {
            objectToPickup = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<ColoredCube>())
        {
            objectToPickup = null;
        }
    }
}
