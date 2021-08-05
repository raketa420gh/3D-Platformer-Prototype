using UnityEngine;

public class Rotator : MonoBehaviour
{
    #region Variables
    
    [SerializeField] [Range(0, 10)] private float speedX;
    [SerializeField] [Range(0, 10)] private float speedY;
    [SerializeField] [Range(0, 10)] private float speedZ;
    
    #endregion


    #region Unity livecycle

    private void Update()
    {
        transform.Rotate(speedX, speedY, speedZ, Space.Self);
    }

    #endregion
}
