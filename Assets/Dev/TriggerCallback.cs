using UnityEngine;
using UnityEngine.Events;

public class TriggerCallback : MonoBehaviour
{

    [Tooltip("The objects able to make this trigger emit the event. If let empty, all objects will be allowed.")]
    public GameObject[] objectsFilter = { };
    public UnityEvent onTriggerEnter = new UnityEvent();

    private void OnTriggerEnter(Collider other)
    {
        if (objectsFilter.Length > 0)
        {
            foreach (GameObject obj in objectsFilter)
            {
                if (other.gameObject == obj)
                {
                    onTriggerEnter.Invoke();
                    return;
                }
            }
        }
        else
        {
            onTriggerEnter.Invoke();
        }
    }

}
