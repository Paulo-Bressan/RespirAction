using UnityEngine;

/// <summary>
/// Objeto que força o jogador a renascer no último checkpoint ao colidir.
/// </summary>
public class Deathzone : MonoBehaviour
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
                // Chama o método Respawn do jogador.
                player.Respawn();
            }
        }
    }
}