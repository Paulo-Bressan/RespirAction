using UnityEngine;

public class CheckpointF2 : MonoBehaviour
{
    [Tooltip("Script controlador da camera")]
    [SerializeField] private CameraControllerF2 cameraController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu é o Jogador (usando a Tag "Player")
        if (other.CompareTag("player"))
        {
            Debug.Log("[CAMERA] Player entrou na hitbox de zoom");
            if(cameraController)
                cameraController.EnterFocus();
            else
                Debug.Log("[CAMERA] Controlador de camera faltando no checkpoint");

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // verifica
        if (other.CompareTag("player"))
        {
            Debug.Log("[CAMERA] Player saiu da hitbox de zoom");
            if (cameraController)
                cameraController.LeaveFocus();
            else
                Debug.Log("[CAMERA] Controlador de camera faltando no checkpoint");
        }
    }
}
