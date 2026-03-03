using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic; // Necessário para usar Listas

public class LogicaQuiz : MonoBehaviour
{
    // --- ESTRUTURA DOS DADOS ---
    [System.Serializable] // Isso faz aparecer bonitinho no Inspector
    public class Questao
    {
        [TextArea] public string enunciado; // O texto da pergunta
        public string[] alternativas;       // As 4 opções de resposta
        public int indiceCorreta;           // 0, 1, 2 ou 3
        [TextArea] public string explicacao; // Texto da explicação final
    }

    // --- VARIÁVEIS DO INSPECTOR ---
    public List<Questao> listaDeQuestoes;   // Lista onde você vai cadastrar as perguntas

    [Header("Referências da UI")]
    public TextMeshProUGUI textoEnunciado;  // Arraste o texto da pergunta aqui
    public TextMeshProUGUI textoExplicacao; // Arraste o texto do painel de explicação
    public TextMeshProUGUI[] textosBotoes;  // Arraste os TEXTOS que estão dentro dos botões
    public Button[] botoes;                 // Os botões (já existia)
    public GameObject painelExplicacao;     // O painel (já existia)

    // --- VARIÁVEIS DE CONTROLE ---
    private int indicePerguntaAtual = 0;

    void Start()
    {
        painelExplicacao.SetActive(false);
        CarregarPergunta();
    }

    void CarregarPergunta()
    {
        if (indicePerguntaAtual < listaDeQuestoes.Count)
        {
            Questao q = listaDeQuestoes[indicePerguntaAtual];

            // 1. Atualiza os textos da tela
            textoEnunciado.text = q.enunciado;
            textoExplicacao.text = q.explicacao;

            // 2. Atualiza os botões
            for (int i = 0; i < botoes.Length; i++)
            {
                // Atualiza o texto do botão (verifica se existe alternativa para evitar erro)
                if (i < q.alternativas.Length)
                    textosBotoes[i].text = q.alternativas[i];

                // Reseta a cor e a interação
                botoes[i].interactable = true;
                botoes[i].image.color = Color.white; // Ou a cor original dos seus botões
            }
        }
        else
        {
            Debug.Log("Fim do Quiz!");
            // Aqui você pode carregar uma cena de "Vitória" ou reiniciar
        }
    }

    public void VerificarResposta(int indiceClicado)
    {
        // Pega a resposta correta da pergunta ATUAL
        int respostaCorretaAtual = listaDeQuestoes[indicePerguntaAtual].indiceCorreta;

        // Desativa botões
        foreach (Button b in botoes) b.interactable = false;

        if (indiceClicado == respostaCorretaAtual)
        {
            botoes[indiceClicado].image.color = Color.green;
        }
        else
        {
            botoes[indiceClicado].image.color = Color.red;
            botoes[respostaCorretaAtual].image.color = Color.green;
        }

        StartCoroutine(EsperarEMostrarExplicacao());
    }

    IEnumerator EsperarEMostrarExplicacao()
    {
        yield return new WaitForSeconds(5f);
        painelExplicacao.SetActive(true);
    }

    // --- NOVA FUNÇÃO: CHAMAR NO BOTÃO "PRÓXIMA" ---
    public void ProximaPergunta()
    {
        painelExplicacao.SetActive(false); // Esconde a explicação
        indicePerguntaAtual++;             // Aumenta o índice
        CarregarPergunta();                // Monta a nova tela
    }
}