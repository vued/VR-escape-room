using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSlideController : MonoBehaviour
{
    private bool openTrigger = true;
    private bool closeTrigger = false;

    [SerializeField] private Transform slideOpen;
    [SerializeField] private Transform slideClose;

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

            Debug.Log("openTrigger/closeTrigger: " + openTrigger + "/" + closeTrigger);

            if (openTrigger)
            {
                Debug.Log("Colliding with Door Open Trigger: 1");
                isActive = false;
                audioSrc.PlayOneShot(audioClipOut);
                StartCoroutine(TablePullPush(slideClose.position, slideOpen.position));
                Debug.Log("Colliding with Door Open Trigger: 2");
                openTrigger = false;
                closeTrigger = true;
            }
            else if (closeTrigger)
            {
                Debug.Log("Colliding with Door Close Trigger: 1");
                isActive = false;
                audioSrc.PlayOneShot(audioClipIn);
                StartCoroutine(TablePullPush(slideOpen.position, slideClose.position));
                Debug.Log("Colliding with Door Close Trigger: 2");
                openTrigger = true;
                closeTrigger = false;
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
