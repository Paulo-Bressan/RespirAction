using UnityEngine;

public class NodeClickManager : MonoBehaviour
{
    private Vector3 mousePosition;
    private RaycastHit2D raycasthit2D;
    private GameObject foundNode;
    private PlayerMovementF2 playerMovement;

    [Header("Referências")]

    [Tooltip("Objeto do jogador")]
    [SerializeField] private GameObject player = null;

    [Tooltip("Script do AudioManagerScene")]
    [SerializeField] private AudioManagerScene audioManagerScene = null;

    [Tooltip("Script do menu de pausa")]
    [SerializeField] private PauseMenu pauseMenu = null;

    [Tooltip("Script do movimento do diafragma")]
    [SerializeField] private ScalePulsator scalePulsator = null;

    [Header("Interação de nós")]

    [Tooltip("Qual nó o mouse está em cima (null se nenhum)")]
    [SerializeField] private GameObject hoverNode = null;

    [Tooltip("Qual nó está sendo puxado (null se nenhum)")]
    [SerializeField] private GameObject pulledNode = null;

    

    void Start()
    {
        if (player)
            playerMovement = player.GetComponent<PlayerMovementF2>();
        else
            Debug.LogWarning("[MANAGER DE NÓ] JOGADOR FALTANDO");

        if (!audioManagerScene)
            Debug.LogWarning("[MANAGER DE NÓ] AUDIOMANAGER FALTANDO");
        if (!pauseMenu)
            Debug.LogWarning("[MANAGER DE NÓ] MENU DE PAUSA FALTANDO");
        if (!scalePulsator)
            Debug.LogWarning("[MANAGER DE NÓ] SCRIPT DO DIAFRAGMA FALTANDO");
    }

    void Update()
    {
        // coisas do raycast
        mousePosition = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);

        raycasthit2D = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
        
        if (raycasthit2D && !pauseMenu.isPaused)
        {
            // se o raycast acertou algo, verifica se é do tag node. se sim, este é o hovernode
            // tambem so faz isto se o jogo nao esta pausado para evitar comportamentos estranhos
            foundNode = raycasthit2D.collider.gameObject;
            hoverNode = (foundNode.tag == "node") ? foundNode : null;
        }
        else hoverNode = null;

        if (Input.GetMouseButtonDown(0))
        {
            audioManagerScene.PlaySound(0);
            // ao clicar o mouse, se existe um hovernode E ainda não existe um pullednode,
            // o pullednode vai ser igual ao hovernode
            if (hoverNode)
            {
                if (!pulledNode) pulledNode = hoverNode;
                playerMovement.SetInteractingState(true);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // ao soltar o mouse, varias considerações são feitas. se existe um hovernode
            // e um pullednode, fazemos verificações para ver se a conexão é permitida.
            // se sim, chamamos o método que cuida do ligamento das conexões.

            if(hoverNode && pulledNode)
            {
                NodeBehavior hoverBehavior = hoverNode.GetComponent<NodeBehavior>();
                NodeBehavior pulledBehavior = pulledNode.GetComponent<NodeBehavior>();

                if (pulledBehavior.isPositive)
                {
                    if (checkConnection(hoverBehavior, pulledBehavior))
                    {
                        Debug.Log("[MANAGER DE NÓ] Puxando para nó correto");
                        audioManagerScene.PlaySound(2);
                        hoverBehavior.handleConnection(pulledBehavior.connectionID);
                        if (hoverBehavior.nodeID == 13) // no final
                            scalePulsator.toggleConnection();
                    }
                    else
                    {
                        Debug.Log("[MANAGER DE NÓ] Puxando para nó errado");
                        audioManagerScene.PlaySound(3);
                    }
                }
                else 
                {
                    Debug.Log("[MANAGER DE NÓ] Puxando de um nó desligado");
                    audioManagerScene.PlaySound(3);
                }
            }
            else audioManagerScene.PlaySound(1);

            pulledNode = null;
            playerMovement.SetInteractingState(false);
        }
    }

    // verifica se os nós escolhidos podem ser conectados
    private bool checkConnection(NodeBehavior hoverBehavior, NodeBehavior pulledBehavior) 
    {
        // verifica se o hovernode está em sequencia do pullednode
        bool isSequence = pulledBehavior.nodeID == (hoverBehavior.nodeID - 1);

        // caso especifico dos nos esq_1 e dir_1 que possuem 3 conexoes em sequencia
        if (hoverBehavior.nodeID == 5)
            if (pulledBehavior.nodeID == 2 || pulledBehavior.nodeID == 3 || pulledBehavior.nodeID == 4)
                isSequence = true;

        // verifica alinhamento (para evitar cruzar esquerda e direita)
        // se algum dos nós a serem conectados é do tipo 0 (central), aprova imediato
        // senao, verifica se o tipo do hovernode é igual ao tipo do pullednode
        bool isAligned = (pulledBehavior.nodeType == 0 || hoverBehavior.nodeType == 0) ?
            true : pulledBehavior.nodeType == hoverBehavior.nodeType;

        // se passar nas duas verificacoes, retorna true
        if (isSequence && isAligned) return true;
        return false;
    }

    // getter para pullednode
    // pode ser criado um para hovernode se precisar tbm
    public GameObject getPulledNode()
    {
        if (pulledNode)
            return pulledNode;
        else return null;
    }
}
