using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTableController : MonoBehaviour
{
    private bool outTrigger = true;
    private bool inTrigger = false;

    [SerializeField] private Transform tableOut;
    [SerializeField] private Transform tableIn;

    [SerializeField] private AudioSource audioSrc;
    [SerializeField] private AudioClip audioClipOut;
    [SerializeField] private AudioClip audioClipIn;

    private bool isActive = false;

    private float speed = 0.1f;
    private float duration = 1.0f;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colliding with Door: " + other.name);

        if (other.gameObject.name.Equals("Left Base Controller") || other.gameObject.name.Equals("Right Base Controller"))
        {

            Debug.Log("openTrigger/closeTrigger: " + outTrigger + "/" + inTrigger);

            if (outTrigger)
            {
                Debug.Log("Colliding with Door Open Trigger: 1");
                isActive = false;
                audioSrc.PlayOneShot(audioClipOut);
                StartCoroutine(TablePullPush(tableIn.position, tableOut.position));
                Debug.Log("Colliding with Door Open Trigger: 2");
                outTrigger = false;
                inTrigger = true;
            }
            else if (inTrigger)
            {
                Debug.Log("Colliding with Door Close Trigger: 1");
                isActive = false;
                audioSrc.PlayOneShot(audioClipIn);
                StartCoroutine(TablePullPush(tableOut.position, tableIn.position));
                Debug.Log("Colliding with Door Close Trigger: 2");
                outTrigger = true;
                inTrigger = false;
            }
        }
    }

    private IEnumerator TablePullPush(Vector3 from, Vector3 to)
    {
        if (isActive)
            yield break;

        isActive = true;

        float counter = 0;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            transform.position = Vector3.Lerp(from, to, counter);
            yield return null;
        }

        yield return new WaitForSeconds(2);

        isActive = false;
    }
}
