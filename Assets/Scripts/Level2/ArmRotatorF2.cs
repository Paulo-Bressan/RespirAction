using UnityEngine;

public class ArmRotatorF2 : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [Tooltip("Ajuste em graus se o sprite do braço não estiver alinhado corretamente (ex: 90f).")]
    [SerializeField] private float rotationOffset = 0f;

    void Start()
    {
        
    }

    void Update()
    {
        // 1. Pega a posição do Mouse
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 2. Calcula a direção do braço para o mouse
        Vector3 direction = mousePos - transform.position;

        // 3. Calcula o ângulo em graus (Atan2 é ideal para 2D)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 4. Aplica a rotação no eixo Z
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + rotationOffset)); ;
    }
}
