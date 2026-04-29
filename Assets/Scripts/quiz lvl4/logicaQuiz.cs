using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LogicaQuiz : MonoBehaviour
{
    // --- ESTRUTURA DOS DADOS ---
    [System.Serializable]
    public class Questao
    {
        [TextArea] public string enunciado;
        public string[] alternativas;
        public int indiceCorreta;
        [TextArea] public string explicacao;
        public Sprite imagemDaPergunta; 
    }

    // --- VARIÁVEIS DO INSPECTOR ---
    public List<Questao> listaDeQuestoes;

    [Header("Referências da UI")]
    public TextMeshProUGUI textoEnunciado;
    public TextMeshProUGUI textoExplicacao;
    public TextMeshProUGUI[] textosBotoes;
    public Button[] botoes;
    public GameObject painelExplicacao;

    [Header("Configuração do Quadro")]
    public GameObject objetoQuadroImagem;  // O GameObject inteiro do Quadro/Moldura
    public Image imagemDentroDoQuadro;     // O componente Image onde a foto vai aparecer

    [Header("Botões do Painel de Explicação")]
    public GameObject botaoProxima;
    public GameObject botaoMenu;

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

            textoEnunciado.text = q.enunciado;
            textoExplicacao.text = q.explicacao;

            // --- LÓGICA APENAS DO QUADRO ---
            if (q.imagemDaPergunta != null)
            {
                // Tem imagem: Liga apenas o quadro e coloca a foto (a doutora continua onde sempre esteve)
                objetoQuadroImagem.SetActive(true);
                imagemDentroDoQuadro.sprite = q.imagemDaPergunta;
            }
            else
            {
                // Não tem imagem: Desliga apenas o quadro
                objetoQuadroImagem.SetActive(false);
            }

            for (int i = 0; i < botoes.Length; i++)
            {
                if (i < q.alternativas.Length)
                    textosBotoes[i].text = q.alternativas[i];

                botoes[i].interactable = true;
                botoes[i].image.color = Color.white;
            }
        }
        else
        {
            Debug.Log("Fim do Quiz!");
        }
    }

    public void VerificarResposta(int indiceClicado)
    {
        int respostaCorretaAtual = listaDeQuestoes[indicePerguntaAtual].indiceCorreta;
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
        yield return new WaitForSeconds(2f);

        if (indicePerguntaAtual == listaDeQuestoes.Count - 1)
        {
            botaoProxima.SetActive(false);
            botaoMenu.SetActive(true);
        }
        else
        {
            botaoProxima.SetActive(true);
            botaoMenu.SetActive(false);
        }

        painelExplicacao.SetActive(true);
    }

    public void ProximaPergunta()
    {
        painelExplicacao.SetActive(false);
        indicePerguntaAtual++;
        CarregarPergunta();
    }
}