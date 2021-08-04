using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Variables

    private static T instance;

    #endregion


    #region Properties

    public static T Instance => instance;

    #endregion


    #region Unity lifecycle

    protected void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);

            return;
        }

        instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    #endregion
}
