using UnityEngine;

public class PlatformTriggerHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerInsideTrigger(other))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayerInsideTrigger(other))
        {
            other.transform.SetParent(null);
        }
    }

    private bool IsPlayerInsideTrigger(Collider collider)
    {
        return collider.gameObject.CompareTag(TagsNames.Player);
    }
}
