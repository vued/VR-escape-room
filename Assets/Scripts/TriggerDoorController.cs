using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorController : MonoBehaviour
{
    //[SerializeField] private Animator myDoor = null;

    private bool openTrigger = true;
    private bool closeTrigger = false;

    [SerializeField] private Transform doorOpen;
    [SerializeField] private Transform doorClose;

    private bool isActive = false;

    private float speed = 0.1f;
    private float duration = 1.0f;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colliding with Door: " + other.name);
        
        if (other.gameObject.name.Equals("Left Base Controller") || other.gameObject.name.Equals("Right Base Controller"))
        {

            Debug.Log("openTrigger/closeTrigger: " + openTrigger + "/" + closeTrigger);
            
            if (openTrigger)
            {
                Debug.Log("Colliding with Door Open Trigger: 1");
                isActive = false;
                StartCoroutine(DoorRotation(doorClose.rotation, doorOpen.rotation));
                Debug.Log("Colliding with Door Open Trigger: 2");
                openTrigger = false;
                closeTrigger = true;
            }
            else if (closeTrigger)
            {
                Debug.Log("Colliding with Door Close Trigger: 1");
                isActive = false;
                StartCoroutine(DoorRotation(doorOpen.rotation, doorClose.rotation));
                Debug.Log("Colliding with Door Close Trigger: 2");
                openTrigger = true;
                closeTrigger = false;
            }
        }
    }

    private IEnumerator DoorRotation(Quaternion from, Quaternion to)
    {
        if (isActive)
            yield break;

        isActive = true;

        float counter = 0;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(from, to, counter);
            yield return null;
        }

        yield return new WaitForSeconds(2);

        isActive = false;
    }
}
