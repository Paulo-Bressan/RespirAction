using UnityEngine;

public class RotationManager : MonoBehaviour
{
    // A velocidade da rotação
    public float rotationSpeed = 5f;
    
    // O objeto que está selecionado
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
                // A gente verifica se o objeto clicado tem a tag "Peca"
                if (hit.transform.CompareTag("Peca"))
                {
                    // Se sim, ele se torna o objeto selecionado
                    selectedObject = hit.transform;
                    // Define a rotação alvo como a rotação atual do objeto
                    targetRotation = selectedObject.rotation;
                }
            }
        }
        
        //  Lógica de Rotação
        if (selectedObject != null)
        {
            // Verifica a entrada do teclado para as rotações
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                targetRotation *= Quaternion.Euler(90, 0, 0);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                targetRotation *= Quaternion.Euler(-90, 0, 0);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                targetRotation *= Quaternion.Euler(0, 90, 0);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                targetRotation *= Quaternion.Euler(0, -90, 0);
            }

            //Eixos X e Y estão sendo rotacionados : Observando a execução vejo que ha uma alteração no eixo Z em alguns momentos, creio que
            // Seja devido a profundidade do objeto alterar em algumas rotações 

            // Interpolação suave para a rotação alvo
            selectedObject.rotation = Quaternion.Slerp(selectedObject.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}