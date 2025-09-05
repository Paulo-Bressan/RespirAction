using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapArea : MonoBehaviour
{
    public int hasObject = 0;
    public GameObject currentObject = null;

    // este script fica de olho nas movimentações das peças
    // que entram e saem do snap. Quando uma peça entra ou sai,
    // atualizamos o script do objeto que entrou para marcar qual
    // snap que ele acabou de entrar. Além disto, atualizamos um
    // contador interno para evitar multiplos objetos no mesmo snap

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name + " has entered " + gameObject.name);
        other.GetComponent<Move3D>().snapArea = gameObject;

        hasObject++;

        if (hasObject == 1) currentObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log(other.gameObject.name + " has left " + gameObject.name);
        if (other.GetComponent<Move3D>().snapArea == gameObject)
            other.GetComponent<Move3D>().snapArea = null;

        hasObject--;

        if (hasObject == 0) currentObject = null;
    }
    private void OnDrawGizmos()
    {
        // Gizmos para ajudar a visualizar a area de snap

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
