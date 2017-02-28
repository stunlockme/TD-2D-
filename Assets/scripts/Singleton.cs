using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    //create a generic data type where the type inherits from monobehaviour
    private static T instance;

    //create a empty readonly object
    private static readonly object padlock = new object();

    public static T Instance
    {
        get
        {
            //marks the following statements as critical
            lock (padlock)
            {
                //null check and initialise the generic type
                if (instance == null)
                    instance = FindObjectOfType<T>();

                return instance;
            }
        }
    }

}
