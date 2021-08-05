using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] private GameObject deathPrefab;
    [SerializeField] private GameObject jumpPrefab;

    private void OnEnable()
    {
        PlayerMovement.OnJumped += PlayJumpVFX;
        Player.OnDied += PlayDeathVFX;
    }

    private void OnDisable()
    {
        PlayerMovement.OnJumped -= PlayJumpVFX;
        Player.OnDied -= PlayDeathVFX;
    }

    private void PlayDeathVFX()
    {
        Instantiate(deathPrefab, transform.position, Quaternion.identity);
    }
    
    private void PlayJumpVFX()
    {
        Instantiate(jumpPrefab, transform.position, Quaternion.identity);
    }
}
