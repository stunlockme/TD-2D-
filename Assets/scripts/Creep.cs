using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creep : MonoBehaviour
{
    public void Spawn()
    {
        this.transform.position = LevelGenerator.Instance.creepGate.transform.position;
    }
}
