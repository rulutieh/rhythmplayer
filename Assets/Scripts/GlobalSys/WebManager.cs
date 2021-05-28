using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebManager : MonoBehaviour
{
    private static WebManager singleton = null;

    public static WebManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                var go = new GameObject("WebManager Singleton");
                singleton = go.AddComponent<WebManager>();
            }
            return singleton;
        }
    }

    private void Awake()
    {
        if (singleton == null)
            singleton = this as WebManager;
            //DontDestroyOnLoad(this.gameObject);
        else
        {
            Debug.LogError("This instance is singleton");
            Destroy(this.gameObject);
        }
    }

}
