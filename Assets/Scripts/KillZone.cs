using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    
    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerInsideTrigger(other))
        {
            sceneLoader.LoadScene(0);
        }
    }

    private bool IsPlayerInsideTrigger(Collider other)
    {
        return other.gameObject.CompareTag(TagsNames.Player);
    }
}