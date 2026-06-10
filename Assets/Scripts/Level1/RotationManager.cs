using UnityEngine;

public class RotationManager : MonoBehaviour
{
    // A velocidade da rotação
    public float rotationSpeed = 1f;
    
    private Transform selectedObject;
    
    // A rotação alvo do objeto selecionado
    private Quaternion targetRotation;

    void Update()
    {
        // Lógica de Seleção 
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                // verifica se o objeto clicado tem a tag "Peca"
                if (hit.transform.CompareTag("Peca"))
                {
                    // Se sim, ele se torna o objeto selecionado
                    selectedObject = hit.transform;
                    // Define a rotação alvo como a rotação atual do objeto
                    targetRotation = selectedObject.rotation;
                }
            }
        }
        
        // Lógica de Rotação 
        if (selectedObject != null)
        {
            // armazenar o incremento da rotação em um único frame
            Quaternion rotationDelta = Quaternion.identity;

            // Rotação Horizontal 
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                // Gira 90 graus em torno do eixo Y (Vector3.up)
                rotationDelta = Quaternion.AngleAxis(-5, Vector3.up);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                // Gira -90 graus em torno do eixo Y (Vector3.up)
                rotationDelta = Quaternion.AngleAxis(5, Vector3.up);
            }
            // Rotação Vertical
            // DESABILITADO POR DIFICULDADE
            else if (Input.GetKey(KeyCode.UpArrow) && false)
            {
                // Gira 90 graus em torno do eixo X 
                rotationDelta = Quaternion.AngleAxis(5, Vector3.right);
            }
            else if (Input.GetKey(KeyCode.DownArrow) && false)
            {
                // Gira -90 graus em torno do eixo X
                rotationDelta = Quaternion.AngleAxis(-5, Vector3.right);
            }
            
            // Aplica a rotação de "Mundo" à rotação alvo atual.
            // A ordem é importante: (Nova Rotação) * (Rotação Atual)
            if (rotationDelta != Quaternion.identity)
            {
                targetRotation = rotationDelta * targetRotation;
            }

            // Interpolação suave para a rotação alvo
            // O objeto só se move quando a rotação alvo é diferente da rotação atual.
            selectedObject.rotation = Quaternion.Slerp(selectedObject.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}