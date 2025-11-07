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
}