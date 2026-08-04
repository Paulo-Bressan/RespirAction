using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizNovoController : MonoBehaviour
{
    [Header("Banco de perguntas")]
    [SerializeField] private QuizNovoDatabase bancoDePerguntas;

    [Header("Pergunta")]
    [SerializeField] private TMP_Text tituloPergunta;
    [SerializeField] private TMP_Text textoPergunta;
    [SerializeField] private Image imagemAuxiliar;
    [SerializeField] private ScrollRect scrollPergunta;

    [Header("Alternativa")]
    [SerializeField] private TMP_Text tituloAlternativa;
    [SerializeField] private TMP_Text textoAlternativa;
    [SerializeField] private ScrollRect scrollAlternativa;

    [Header("Navegacao")]
    [SerializeField] private Button botaoAnterior;
    [SerializeField] private Button botaoProximo;
    [SerializeField] private Button botaoMarcarAlternativa;

    [Header("Painel de resultado")]
    [SerializeField] private GameObject painelResultadoRoot;
    [SerializeField] private TMP_Text tituloResultado;
    [SerializeField] private TMP_Text textoJustificativa;
    [SerializeField] private ScrollRect scrollJustificativa;
    [SerializeField] private Button botaoContinuar;
    [SerializeField] private Color corCorreto = Color.green;
    [SerializeField] private Color corIncorreto = Color.red;

    private int indiceQuestaoAtual;
    private int indiceAlternativaAtual;
    private bool painelResultadoAberto;

    private void Start()
    {
        OcultarPainelResultado();
        VerificarReferenciasPainel();
        RegistrarEventos();

        if (!BancoValido())
        {
            return;
        }

        indiceQuestaoAtual = 0;
        indiceAlternativaAtual = 0;
        MostrarQuestaoAtual();
    }

    private void OnDestroy()
    {
        if (botaoAnterior != null)
        {
            botaoAnterior.onClick.RemoveListener(AlternativaAnterior);
        }

        if (botaoProximo != null)
        {
            botaoProximo.onClick.RemoveListener(ProximaAlternativa);
        }

        if (botaoMarcarAlternativa != null)
        {
            botaoMarcarAlternativa.onClick.RemoveListener(MarcarAlternativa);
        }

        if (botaoContinuar != null)
        {
            botaoContinuar.onClick.RemoveListener(ContinuarAposResultado);
        }
    }

    public void MostrarQuestaoAtual()
    {
        if (!QuestaoAtualValida())
        {
            return;
        }

        QuestaoQuizNovo questao = bancoDePerguntas.questoes[indiceQuestaoAtual];

        if (tituloPergunta != null)
        {
            tituloPergunta.text = $"Pergunta {indiceQuestaoAtual + 1}";
        }

        if (textoPergunta != null)
        {
            textoPergunta.text = questao.pergunta;
        }

        AtualizarImagemAuxiliar(questao.imagemAuxiliar);
        MostrarAlternativaAtual();
        ReposicionarScrollNoTopo(scrollPergunta);
    }

    public void MostrarAlternativaAtual()
    {
        if (!QuestaoAtualValida())
        {
            return;
        }

        QuestaoQuizNovo questao = bancoDePerguntas.questoes[indiceQuestaoAtual];
        char letraAlternativa = (char)('A' + indiceAlternativaAtual);

        if (tituloAlternativa != null)
        {
            tituloAlternativa.text = $"Alternativa {letraAlternativa}";
        }

        if (textoAlternativa != null)
        {
            textoAlternativa.text = questao.ObterAlternativa(indiceAlternativaAtual);
        }

        ReposicionarScrollNoTopo(scrollAlternativa);
    }

    public void AlternativaAnterior()
    {
        if (!QuestaoAtualValida())
        {
            return;
        }

        // Mantem a navegacao circular entre A e D.
        indiceAlternativaAtual = (indiceAlternativaAtual + 3) % 4;
        MostrarAlternativaAtual();
    }

    public void ProximaAlternativa()
    {
        if (!QuestaoAtualValida())
        {
            return;
        }

        indiceAlternativaAtual = (indiceAlternativaAtual + 1) % 4;
        MostrarAlternativaAtual();
    }

    public void MarcarAlternativa()
    {
        if (painelResultadoAberto || !QuestaoAtualValida())
        {
            return;
        }

        QuestaoQuizNovo questao = bancoDePerguntas.questoes[indiceQuestaoAtual];
        bool alternativaCorreta = indiceAlternativaAtual == questao.indiceAlternativaCorreta;

        if (tituloResultado != null)
        {
            tituloResultado.text = alternativaCorreta ? "CORRETO!" : "INCORRETO!";
            tituloResultado.color = alternativaCorreta ? corCorreto : corIncorreto;
        }

        if (textoJustificativa != null)
        {
            textoJustificativa.text = string.IsNullOrWhiteSpace(questao.justificativa)
                ? "Não há uma justificativa cadastrada para esta questão."
                : questao.justificativa;
        }

        painelResultadoAberto = true;

        if (painelResultadoRoot != null)
        {
            painelResultadoRoot.SetActive(true);
        }

        DefinirBotoesNavegacaoInteragiveis(false);
        ReposicionarScrollNoTopo(scrollJustificativa);
        Debug.Log(alternativaCorreta ? "Resposta correta." : "Resposta incorreta.");
    }

    public void ContinuarAposResultado()
    {
        OcultarPainelResultado();
        DefinirBotoesNavegacaoInteragiveis(true);

        if (!QuestaoAtualValida())
        {
            return;
        }

        if (indiceQuestaoAtual < bancoDePerguntas.questoes.Count - 1)
        {
            DefinirQuestao(indiceQuestaoAtual + 1);
            return;
        }

        Debug.Log("Quiz concluído.");
    }

    public void DefinirQuestao(int indice)
    {
        if (!BancoValido() || indice < 0 || indice >= bancoDePerguntas.questoes.Count)
        {
            return;
        }

        indiceQuestaoAtual = indice;
        indiceAlternativaAtual = 0;
        MostrarQuestaoAtual();
    }

    public void ProximaQuestao()
    {
        if (!QuestaoAtualValida())
        {
            return;
        }

        if (indiceQuestaoAtual >= bancoDePerguntas.questoes.Count - 1)
        {
            Debug.Log("Quiz terminado.");
            return;
        }

        DefinirQuestao(indiceQuestaoAtual + 1);
    }

    private void RegistrarEventos()
    {
        if (botaoAnterior != null)
        {
            botaoAnterior.onClick.AddListener(AlternativaAnterior);
        }

        if (botaoProximo != null)
        {
            botaoProximo.onClick.AddListener(ProximaAlternativa);
        }

        if (botaoMarcarAlternativa != null)
        {
            botaoMarcarAlternativa.onClick.AddListener(MarcarAlternativa);
        }

        if (botaoContinuar != null)
        {
            botaoContinuar.onClick.AddListener(ContinuarAposResultado);
        }
    }

    private bool BancoValido()
    {
        if (bancoDePerguntas == null)
        {
            Debug.LogWarning("Banco de perguntas nao foi atribuido.", this);
            return false;
        }

        if (bancoDePerguntas.questoes == null || bancoDePerguntas.questoes.Count == 0)
        {
            Debug.LogWarning("O banco de perguntas nao possui questoes.", this);
            return false;
        }

        return true;
    }

    private bool QuestaoAtualValida()
    {
        return BancoValido()
            && indiceQuestaoAtual >= 0
            && indiceQuestaoAtual < bancoDePerguntas.questoes.Count
            && bancoDePerguntas.questoes[indiceQuestaoAtual] != null;
    }

    private void AtualizarImagemAuxiliar(Sprite sprite)
    {
        if (imagemAuxiliar == null)
        {
            return;
        }

        imagemAuxiliar.sprite = sprite;
        // Desativa apenas o componente, preservando o quadro e o objeto pai.
        imagemAuxiliar.enabled = sprite != null;
    }

    private void OcultarPainelResultado()
    {
        painelResultadoAberto = false;

        if (painelResultadoRoot != null)
        {
            painelResultadoRoot.SetActive(false);
        }
    }

    private void DefinirBotoesNavegacaoInteragiveis(bool interagiveis)
    {
        if (botaoAnterior != null)
        {
            botaoAnterior.interactable = interagiveis;
        }

        if (botaoProximo != null)
        {
            botaoProximo.interactable = interagiveis;
        }

        if (botaoMarcarAlternativa != null)
        {
            botaoMarcarAlternativa.interactable = interagiveis;
        }
    }

    private void VerificarReferenciasPainel()
    {
        if (painelResultadoRoot == null)
        {
            Debug.LogWarning("Painel de resultado não foi atribuído.", this);
        }

        if (tituloResultado == null)
        {
            Debug.LogWarning("Título do resultado não foi atribuído.", this);
        }

        if (textoJustificativa == null)
        {
            Debug.LogWarning("Texto da justificativa não foi atribuído.", this);
        }

        if (scrollJustificativa == null)
        {
            Debug.LogWarning("Scroll da justificativa não foi atribuído.", this);
        }

        if (botaoContinuar == null)
        {
            Debug.LogWarning("Botão Continuar não foi atribuído.", this);
        }
    }

    private void ReposicionarScrollNoTopo(ScrollRect scroll)
    {
        if (scroll == null)
        {
            return;
        }

        Canvas.ForceUpdateCanvases();
        scroll.verticalNormalizedPosition = 1f;
    }
}
