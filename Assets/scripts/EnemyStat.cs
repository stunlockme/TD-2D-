using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    [SerializeField]
    private Stat Health;

    private void Start()
    {
        Health.Initialize();
        Debug.Log("value set");
    }

}
