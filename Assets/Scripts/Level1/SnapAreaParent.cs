using UnityEngine;

public class SnapAreaParent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Move3D>().insideSnapArea = true;
        Debug.Log(other + " has entered the snap zone");
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<Move3D>().insideSnapArea = false;
        Debug.Log(other + " has left the snap zone");
    }

    private void OnDrawGizmos()
    {
        // Gizmos para ajudar a visualizar a area de snap

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
