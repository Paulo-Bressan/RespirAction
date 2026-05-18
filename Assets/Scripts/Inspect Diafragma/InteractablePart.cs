using UnityEngine;

// Este script é apenas um "container" de dados.
// Ele diz "Eu sou interativo e este é o meu nome".
public class InteractablePart : MonoBehaviour
{
    [Tooltip("O nome que aparecerá no tooltip")]
    public string tooltipName = "Nome da Parte";

    [Header("Descrição Completa")]
    [Tooltip("O texto que aparecerá quando o objeto for clicado")]
    [TextArea(5, 10)] // Isso cria uma caixa de texto maior no Inspector
    public string description = "Insira a descrição aqui...";
    public AudioClip somDaDescricao; 

    // A Unity chama o OnMouseDown automaticamente quando o jogador clica no objeto.
    private void OnMouseDown()
    {
        // Se existir um som configurado no Inspector, ele vai tocar
        if (somDaDescricao != null) 
        {
            AudioSource.PlayClipAtPoint(somDaDescricao, Camera.main.transform.position);
        }
    }
}