using DG.Tweening;
using UnityEngine;

public class FallenObstacle : MonoBehaviour
{
    [Header("Move Settings")] 
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;

    [Header("Timing Settings")] 
    [SerializeField] private float startPositionDelay;
    [SerializeField] private float toEndPositionMoveTime;
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPosition, 0.5f);
        Gizmos.DrawSphere(endPosition, 0.5f);
        Gizmos.DrawLine(startPosition, endPosition);
    }
    
    public void StartFall()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(startPositionDelay);
        sequence.Append(transform.DOLocalMove(endPosition, toEndPositionMoveTime).SetEase(Ease.Linear));
        sequence.SetLoops(1);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerInsideTrigger(other))
        {
            StartFall();
        }
    }
    
    private bool IsPlayerInsideTrigger(Collider other)
    {
        return other.gameObject.CompareTag(TagsNames.Player);
    }
}
