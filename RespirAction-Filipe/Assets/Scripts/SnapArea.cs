using UnityEngine;

public class SnapArea : MonoBehaviour
{
    public int hasObject = 0;

    // este script fica de olho nas movimentações das peças
    // que entram e saem do snap. Quando uma peça entra ou sai,
    // atualizamos o script do objeto que entrou para marcar qual
    // snap que ele acabou de entrar. Além disto, atualizamos um
    // contador interno para evitar multiplos objetos no mesmo snap

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name + " has entered " + gameObject.name);
        other.GetComponent<Move3D>().insideSnap = true;
        other.GetComponent<Move3D>().snapArea = gameObject;

        hasObject++;
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log(other.gameObject.name + " has left " + gameObject.name);
        other.GetComponent<Move3D>().insideSnap = false;
        other.GetComponent<Move3D>().snapArea = null;

        hasObject--;
    }
    private void OnDrawGizmos()
    {
        // Gizmos para ajudar a visualizar a area de snap

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
