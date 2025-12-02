using UnityEngine;

/// <summary>
/// Objeto que, ao ser ativado pelo jogador, salva a sua posição como novo ponto de respawn.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu é o Jogador (usando a Tag "Player")
        if (other.CompareTag("player"))
        {
            // Tenta obter o script PlayerMovement
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                // 1. Atualiza o ponto de respawn do jogador para a posição deste objeto
                player.UpdateCheckpoint(transform.position);

                // 2. Desativa este objeto Checkpoint para que não possa ser ativado novamente.
                // Isso evita o spam de log e garante que o ponto é salvo apenas uma vez.
                gameObject.SetActive(false); 
                
                // Opcional: Aqui você pode adicionar lógica de som ou animação de "Checkpoint Salvo"
            }
        }
    }
}