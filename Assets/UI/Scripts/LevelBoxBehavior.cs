using System.Collections;
using UnityEngine;

public class LevelBoxBehavior : MonoBehaviour
{
    [Tooltip("ID da caixa para usoo no menuFasesAlt")]
    [SerializeField] private int id = 0;

    [Tooltip("Referencia do menuFasesAlt")]
    [SerializeField] private MenuFasesAlt menuFasesAlt = null;

    // coroutine para espera antes do carregamento
    private IEnumerator coroutine; 
    // flag que determina se o jogador esta em contato com a caixa
    // se passar a espera e o jogador ainda estiver em contato, chama
    // menufasesalt, senao aborta carregamento
    private bool hasPlayer; 

    void Start()
    {
        if (id == 0) Debug.Log("[LEVELBOX] Id nao configurada");
        if (!menuFasesAlt) Debug.Log("[LEVELBOX] menuFasesAlt faltando");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "player")
        {
            Debug.Log("[LEVELBOX] Player entrou na caixa  " + id);
            hasPlayer = true;
            
            // chamando coroutine com delay de 1.5 segundo
            coroutine = callMenuFases(1.5f);
            StartCoroutine(coroutine);
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("[LEVELBOX] Player saiu da caixa  " + id);
        hasPlayer = false;
    }

    private IEnumerator callMenuFases(float waitTime)
    {
        Debug.Log("[MENU FASES] Inicio da espera para carregar fase");
        yield return new WaitForSeconds(waitTime);

        if (hasPlayer) menuFasesAlt.loadLevel(id);
        else Debug.Log("[MENU FASES] Player saiu, carregamento abortado");
    }
}
