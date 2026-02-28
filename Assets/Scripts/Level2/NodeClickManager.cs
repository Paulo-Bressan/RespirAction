using UnityEngine;

public class NodeClickManager : MonoBehaviour
{
    private Vector3 mousePosition;
    private RaycastHit2D raycasthit2D;
    private GameObject foundNode;
    private PlayerMovementF2 playerMovement;

    [Tooltip("Objeto do jogador")]
    public GameObject player = null;

    [Tooltip("Qual nó o mouse está em cima (null se nenhum)")]
    public GameObject hoverNode = null;

    [Tooltip("Qual nó está sendo puxado (null se nenhum)")]
    public GameObject pulledNode = null;

    void Start()
    {
        if (player)
            playerMovement = player.GetComponent<PlayerMovementF2>();
        else
            Debug.Log("[MANAGER DE NÓ] JOGADOR FALTANDO");
    }

    void Update()
    {
        // coisas do raycast
        mousePosition = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);

        raycasthit2D = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
        
        if (raycasthit2D)
        {
            // se o raycast acertou algo, verifica se é do tag node. se sim, este é o hovernode
            foundNode = raycasthit2D.collider.gameObject;
            hoverNode = (foundNode.tag == "node") ? foundNode : null;
        }
        else hoverNode = null;

        if (Input.GetMouseButtonDown(0))
        {
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
                        hoverBehavior.handleConnection(pulledBehavior.connectionID);
                    }
                    else Debug.Log("[MANAGER DE NÓ] Puxando para nó errado");
                }
                else Debug.Log("[MANAGER DE NÓ] Puxando de um nó desligado");
            }

            pulledNode = null;
            playerMovement.SetInteractingState(false);
        }
    }

    // verifica se os nós escolhidos podem ser conectados
    private bool checkConnection(NodeBehavior hoverBehavior, NodeBehavior pulledBehavior) 
    {
        // verifica se o hovernode está em sequencia do pullednode
        bool isSequence = pulledBehavior.nodeID == (hoverBehavior.nodeID - 1);

        // verifica alinhamento (para evitar cruzar esquerda e direita)
        // se algum dos nós a serem conectados é do tipo 0 (central), aprova imediato
        // senao, verifica se o tipo do hovernode é igual ao tipo do pullednode
        bool isAligned = (pulledBehavior.nodeType == 0 || hoverBehavior.nodeType == 0) ?
            true : pulledBehavior.nodeType == hoverBehavior.nodeType;

        // se passar nas duas verificacoes, retorna true
        if (isSequence && isAligned) return true;
        return false;
    }
}
