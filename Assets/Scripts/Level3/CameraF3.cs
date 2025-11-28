using UnityEngine;

/// <summary>
/// Controla o comportamento da câmera, alternando entre seguir o jogador e exibir uma visão geral do mapa.
/// Utiliza LateUpdate para garantir suavidade no acompanhamento do alvo.
/// </summary>
public class CameraController : MonoBehaviour
{
    // =================================================================
    // CONFIGURAÇÕES DO ALVO
    // =================================================================
    [Header("Alvo")]
    [Tooltip("Transform do jogador que a câmera deve seguir.")]
    [SerializeField] private Transform playerTransform;

    // =================================================================
    // CONFIGURAÇÕES DO MAPA E POSICIONAMENTO
    // =================================================================
    [Header("Modo Mapa")]
    [Tooltip("Tecla que alterna entre a visão do jogador e a visão do mapa.")]
    [SerializeField] private KeyCode mapKey = KeyCode.M;

    [Tooltip("Posição Z da câmera durante o gameplay normal (Zoom normal).")]
    [SerializeField] private float normalZPosition = -10f; // Nota: Geralmente em 2D usa-se negativo (-10), ajustei o padrão mas respeite o seu projeto.

    [Tooltip("Posição Z da câmera no modo mapa (Zoom out/afastado).")]
    [SerializeField] private float mapZPosition = -50f; 

    // Estado interno para saber se a câmera está travada no player ou no modo mapa
    private bool isFollowingPlayer = true;

    void Start()
    {
        // Garante que a câmera comece na posição correta do jogador se ele existir
        if (playerTransform != null)
        {
            transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, normalZPosition);
        }
    }

    void Update()
    {
        // INPUT DO JOGADOR
        // Verifica se a tecla de mapa foi pressionada neste frame
        if (Input.GetKeyDown(mapKey))
        {
            // Inverte o estado (se era true vira false, e vice-versa)
            isFollowingPlayer = !isFollowingPlayer;

            if (isFollowingPlayer)
            {
                // Se voltamos a seguir o player, o LateUpdate cuidará do posicionamento no próximo ciclo.
                // Não é necessário código aqui pois o LateUpdate roda todo frame.
            }
            else
            {
                // Se ativamos o modo mapa, movemos a câmera IMEDIATAMENTE para a posição estática do mapa.
                // (0, 0) é assumido como o centro do mundo.
                transform.position = new Vector3(0, 0, mapZPosition);
            }
        }
    }

    /// <summary>
    /// LateUpdate é chamado após todos os Updates. 
    /// É ideal para câmeras para garantir que o jogador já terminou de se mover no frame.
    /// Isso evita que a câmera "trema" ou tenha jitter.
    /// </summary>
    void LateUpdate()
    {
        // Só atualiza a posição se estivermos no modo de seguir o jogador
        if (isFollowingPlayer)
        {
            // Verificação de segurança: se o player for destruído, paramos de tentar segui-lo
            if (playerTransform == null)
            {
                return;
            }
            
            // Obtém a posição atual do jogador
            Vector3 playerPos = playerTransform.position;
            
            // Move a câmera para o X e Y do jogador, mas mantém o Z fixo configurado para gameplay
            transform.position = new Vector3(playerPos.x, playerPos.y, normalZPosition);
        }
    }
}