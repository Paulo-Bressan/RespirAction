using UnityEngine;

/// <summary>
/// Controla a rotação do objeto do braço para apontar para um alvo específico (o tile interativo).
/// O braço trava a rotação na direção do alvo quando a interação é iniciada.
/// </summary>
public class ArmRotator : MonoBehaviour
{
    // A posição para a qual o braço deve apontar (o Tile)
    private Transform targetTile;
    
    [Tooltip("Ajuste em graus se o sprite do braço não estiver alinhado corretamente (ex: 90f).")]
    [SerializeField] private float rotationOffset = 0f;

    /// <summary>
    /// Define o alvo de rotação. Se for null, o braço para de rotacionar (e reseta a rotação).
    /// Chamado pelo PlayerMovement.cs.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        targetTile = newTarget;

        if (targetTile != null)
        {
            // O braço está a ser ativado. Calculamos e travamos a rotação imediatamente.
            RotateArmToTarget();
        }
        else
        {
            // O braço está a ser desativado. Reseta a rotação para o padrão do objeto pai (Jogador).
            transform.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// Calcula o ângulo e aplica a rotação ao braço, travando-o na direção do alvo.
    /// </summary>
    private void RotateArmToTarget()
    {
        if (targetTile == null) return;
        
        // 1. Calcula a direção do braço para o alvo
        Vector3 direction = targetTile.position - transform.position;
        
        // 2. Calcula o ângulo em graus (Atan2 é ideal para 2D)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // 3. Aplica a rotação no eixo Z (travando-a)
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + rotationOffset));
    }
}