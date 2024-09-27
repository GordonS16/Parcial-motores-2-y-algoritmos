using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null) //If the instance is null then we need to find it
            {
                //Finds the object
                instance = FindObjectOfType<T>();
            }

            //Returns the instance
            return instance;
        }

    }

}
