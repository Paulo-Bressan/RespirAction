using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapArea : MonoBehaviour
{
    public GameObject currentObject = null;

    private void OnDrawGizmos()
    {
        // Gizmos para ajudar a visualizar a area de snap

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
