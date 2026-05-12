using UnityEngine;
using UnityEngine.EventSystems; // Necessário para mexer com eventos de mouse/toque na UI

// As interfaces IDragHandler e IBeginDragHandler permitem que o Unity saiba que esse objeto pode ser arrastado
public class ArrastarPainel : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        // Pega as referências do próprio painel e do Canvas onde ele está
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // (Opcional) Joga o painel para a frente de tudo quando o jogador clica para arrastar
        transform.SetAsLastSibling(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            // Move o painel acompanhando o mouse/dedo, respeitando a escala da tela (Canvas Scaler)
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
}
