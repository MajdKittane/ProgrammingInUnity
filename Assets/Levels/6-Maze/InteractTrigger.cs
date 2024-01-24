using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    public GameObject interactObject;
    TriggerManager triggerManager;
    
    // Start is called before the first frame update
    void Start()
    {
        triggerManager = FindAnyObjectByType<TriggerManager>();
        triggerManager.RegisterInteractTrigger(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerManager == null)
        {
            triggerManager = FindAnyObjectByType<TriggerManager>();
            triggerManager.RegisterInteractTrigger(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            triggerManager.OnInteractTrigger(this, true);
            Debug.LogError("Entered" + this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            triggerManager.OnInteractTrigger(this, false);
            Debug.LogError("Exited" + this);
        }
    }
}