using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private Collider col;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("finish");
        col.enabled = false;
    }
}