using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NodeBehavior : MonoBehaviour
{
    [Header("Atributos principais")]
    [Tooltip("Ordem do nó")]
    public int nodeID;

    [Tooltip("Tipo do nó (0 centro / 1 esq / 2 dir)")]
    public int nodeType;

    [Tooltip("Flag que indica se nó está ligado")]
    public bool isPositive;

    [Header("Atributos de conexão")]
    [Tooltip("Quantidade de conexões (1 = ignora lista / 2+ segue a lista)")]
    public int connectionSize;

    [Tooltip("Ordem de conexão")]
    public int connectionID;
    
    [Tooltip("Lista de flags de conexões ativas")]
    public bool[] positiveConnectionList;

    [Tooltip("Cabos a serem ligados")]
    public GameObject[] CableList;

    [Header("Atributos de sprites")]
    [Tooltip("Flag se ignora transição de sprites")]
    [SerializeField] private bool ignoreSprite;

    [Tooltip("Lista de sprites, seguir msm padrao para todos")]
    [SerializeField] private Sprite[] nodeSprites;

    // referencias
    private SpriteRenderer spriteRenderer;
    

    void Start()
    {
        // inicializa PositiveConnectionList
        positiveConnectionList = new bool[connectionSize];

        // avisa se falta sprites, idealmente essa mensagem nunca deve aparecer
        if (!ignoreSprite)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (!spriteRenderer) Debug.Log("[NÓ] Faltando spriteRenderer");

            // checa se todos sprites estão presentes com um contador
            bool checkList = true;
            for (int i = 0; i < connectionSize; i++)
            {
                checkList = checkList && nodeSprites.ElementAt(i);
            }

            if (!checkList) 
                Debug.Log("[NÓ] Faltando sprites na lista de sprites de " + name);
        }
        

        // desativa todos cabos conectores antes da fase começar
        for (int i = 0; i < CableList.Length; i++)
        {
            CableList[i].SetActive(false);
        }
    }

    void Update()
    {
        
    }

    public void updateCables(int cableID)
    {
        if (CableList.Length >= cableID)
            CableList[cableID].SetActive(true);
        else
            Debug.Log("[NÓ] UpdateCables foi chamado para " + cableID + " mas este cabo não existe");
    }

    public void updateSprite(int conexAtivas)
    {
        // muda comportamento dependendo de quantas conexoes tem
        if (connectionSize == 1)
            spriteRenderer.sprite = nodeSprites[1]; // so atualiza para o final
        else
            spriteRenderer.sprite = nodeSprites[conexAtivas];
        // idealmente o tamanho de nodeSprites segue connectionSize
        // o ultimo nodeSprites deve ser o final para assim seguir a ideia de "enchimento"
    }

    public void handleConnection(int connectionID)
    {
        Debug.Log("[NÓ] Fazendo conexão em " + name + " na linha " + connectionID);

        if (positiveConnectionList.Length >= connectionID)
            positiveConnectionList[connectionID] = true;
        else
            Debug.Log("[NÓ] handleConnection foi chamado para " + connectionID + " mas esta conexão não existe");

        int n = 0; // contador de cabos ativos;
        for (int i = 0; i < connectionSize; i++)
            if (positiveConnectionList[i]) n++;

        // se todas conexoes ativas, liga  
        if (n == connectionSize)
        {
            Debug.Log("[NÓ] Com todas conexões ativas, o nó " + name + " agora está ligado");
            isPositive = true;
        }

        // updates visuais
        updateCables(connectionID);
        if (!ignoreSprite) updateSprite(n);
    }
}
