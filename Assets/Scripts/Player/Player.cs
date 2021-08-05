using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerMouseLook))]
[RequireComponent(typeof(PlayerVFX))]

public class Player : MonoBehaviour
{
    #region Variables

    [SerializeField] private MeshRenderer[] playerMeshes;

    #endregion
    
    
    #region Events

    public static event Action OnDied;

    #endregion

    
    #region Unity lifecycle

    private void OnEnable()
    {
        KillZone.OnGotPlayer += Die;
    }

    private void OnDisable()
    {
        KillZone.OnGotPlayer -= Die;
    }

    #endregion


    #region Private methods

    private void Die()
    {
        DisableMeshesView();
        OnDied?.Invoke();
    }

    private void DisableMeshesView()
    {
        foreach (var mesh in playerMeshes)
        {
            mesh.enabled = false;
        }
    }

    #endregion
}
