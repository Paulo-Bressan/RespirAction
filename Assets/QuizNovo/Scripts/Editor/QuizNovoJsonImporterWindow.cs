using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuizNovoJsonImporterWindow : EditorWindow
{
    private const string CaminhoJson = "Assets/QuizNovo/Data/quiz_perguntas_13.json";
    private const string CaminhoBanco = "Assets/QuizNovo/Data/BancoPerguntasQuiz.asset";
    private const string CaminhoImagens = "Assets/QuizNovo/QuestionImages";
    private const int QuantidadeEsperada = 13;

    [SerializeField] private TextAsset arquivoJson;
    [SerializeField] private QuizNovoDatabase bancoDestino;
    [SerializeField] private DefaultAsset pastaDasImagens;

    [MenuItem("RespirAction/Quiz Novo/Importar Perguntas do JSON")]
    private static void AbrirJanela()
    {
        QuizNovoJsonImporterWindow janela = GetWindow<QuizNovoJsonImporterWindow>();
        janela.titleContent = new GUIContent("Importar Quiz Novo");
        janela.minSize = new Vector2(440f, 260f);
        janela.Show();
    }

    private void OnEnable()
    {
        PreencherReferenciasPadrao();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("Importador de perguntas", EditorStyles.boldLabel);
        EditorGUILayout.Space(6f);

        arquivoJson = (TextAsset)EditorGUILayout.ObjectField(
            "Arquivo JSON", arquivoJson, typeof(TextAsset), false);
        bancoDestino = (QuizNovoDatabase)EditorGUILayout.ObjectField(
            "Banco de destino", bancoDestino, typeof(QuizNovoDatabase), false);
        pastaDasImagens = (DefaultAsset)EditorGUILayout.ObjectField(
            "Pasta das imagens", pastaDasImagens, typeof(DefaultAsset), false);

        EditorGUILayout.Space(8f);
        MostrarAvisosDeReferencias();
        EditorGUILayout.Space(12f);

        GUIStyle estiloBotao = new GUIStyle(GUI.skin.button)
        {
            fontSize = 15,
            fontStyle = FontStyle.Bold,
            fixedHeight = 44f
        };

        if (GUILayout.Button("IMPORTAR 13 PERGUNTAS", estiloBotao))
        {
            ImportarPerguntas();
        }
    }

    private void PreencherReferenciasPadrao()
    {
        if (arquivoJson == null)
        {
            arquivoJson = AssetDatabase.LoadAssetAtPath<TextAsset>(CaminhoJson);
        }

        if (bancoDestino == null)
        {
            bancoDestino = AssetDatabase.LoadAssetAtPath<QuizNovoDatabase>(CaminhoBanco);
        }

        if (pastaDasImagens == null)
        {
            pastaDasImagens = AssetDatabase.LoadAssetAtPath<DefaultAsset>(CaminhoImagens);
        }
    }

    private void MostrarAvisosDeReferencias()
    {
        if (arquivoJson == null)
        {
            EditorGUILayout.HelpBox("Selecione o arquivo JSON das perguntas.", MessageType.Error);
        }

        if (bancoDestino == null)
        {
            EditorGUILayout.HelpBox("Selecione o banco de perguntas de destino.", MessageType.Error);
        }

        if (pastaDasImagens == null)
        {
            EditorGUILayout.HelpBox("Selecione a pasta que contém as imagens.", MessageType.Error);
        }
        else
        {
            string caminhoPasta = AssetDatabase.GetAssetPath(pastaDasImagens);
            if (!AssetDatabase.IsValidFolder(caminhoPasta))
            {
                EditorGUILayout.HelpBox("A referência de imagens deve ser uma pasta válida do projeto.", MessageType.Error);
            }
        }
    }

    private void ImportarPerguntas()
    {
        if (!ValidarReferencias(out string erroReferencias))
        {
            ExibirErro(erroReferencias);
            return;
        }

        DadosQuizDto dados;

        try
        {
            if (string.IsNullOrWhiteSpace(arquivoJson.text))
            {
                ExibirErro("O arquivo JSON está vazio.");
                return;
            }

            dados = JsonUtility.FromJson<DadosQuizDto>(arquivoJson.text);
        }
        catch (Exception excecao)
        {
            ExibirErro($"Não foi possível ler o JSON:\n{excecao.Message}");
            return;
        }

        if (!ValidarDados(dados, out string erroDados))
        {
            ExibirErro(erroDados);
            return;
        }

        List<QuestaoQuizNovo> novasQuestoes;
        int imagensAssociadas;
        int perguntasSemImagem;
        int imagensAusentes;

        try
        {
            novasQuestoes = MontarQuestoes(
                dados,
                out imagensAssociadas,
                out perguntasSemImagem,
                out imagensAusentes);
        }
        catch (Exception excecao)
        {
            ExibirErro($"Falha ao preparar as perguntas para importação:\n{excecao.Message}");
            return;
        }

        bool confirmou = EditorUtility.DisplayDialog(
            "Substituir perguntas?",
            $"Todas as questões atuais de '{bancoDestino.name}' serão substituídas pelas 13 questões do JSON. Deseja continuar?",
            "Substituir",
            "Cancelar");

        if (!confirmou)
        {
            return;
        }

        // Mantém uma cópia para restaurar o banco caso a gravação falhe.
        List<QuestaoQuizNovo> questoesAnteriores = bancoDestino.questoes != null
            ? new List<QuestaoQuizNovo>(bancoDestino.questoes)
            : new List<QuestaoQuizNovo>();

        try
        {
            Undo.RecordObject(bancoDestino, "Importar perguntas do Quiz Novo");

            if (bancoDestino.questoes == null)
            {
                bancoDestino.questoes = new List<QuestaoQuizNovo>();
            }

            bancoDestino.questoes.Clear();
            bancoDestino.questoes.AddRange(novasQuestoes);

            EditorUtility.SetDirty(bancoDestino);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = bancoDestino;
            EditorGUIUtility.PingObject(bancoDestino);

            MostrarResumo(
                novasQuestoes.Count,
                imagensAssociadas,
                perguntasSemImagem,
                imagensAusentes);
        }
        catch (Exception excecao)
        {
            RestaurarBanco(questoesAnteriores);
            ExibirErro($"A importação falhou e o conteúdo anterior foi restaurado:\n{excecao.Message}");
        }
    }

    private bool ValidarReferencias(out string erro)
    {
        if (arquivoJson == null)
        {
            erro = "O arquivo JSON não foi atribuído.";
            return false;
        }

        if (bancoDestino == null)
        {
            erro = "O banco de destino não foi atribuído.";
            return false;
        }

        if (pastaDasImagens == null)
        {
            erro = "A pasta das imagens não foi atribuída.";
            return false;
        }

        string caminhoPasta = AssetDatabase.GetAssetPath(pastaDasImagens);
        if (!AssetDatabase.IsValidFolder(caminhoPasta))
        {
            erro = "A referência selecionada para as imagens não é uma pasta válida do projeto.";
            return false;
        }

        erro = string.Empty;
        return true;
    }

    private static bool ValidarDados(DadosQuizDto dados, out string erro)
    {
        if (dados == null)
        {
            erro = "O JSON não contém uma estrutura válida.";
            return false;
        }

        if (dados.questoes == null)
        {
            erro = "O campo 'questoes' não foi encontrado no JSON.";
            return false;
        }

        if (dados.questoes.Count != QuantidadeEsperada)
        {
            erro = $"O JSON deve conter exatamente 13 questões, mas contém {dados.questoes.Count}.";
            return false;
        }

        for (int i = 0; i < dados.questoes.Count; i++)
        {
            QuestaoDto questao = dados.questoes[i];
            if (questao == null)
            {
                erro = $"A questão {i + 1} está vazia ou inválida.";
                return false;
            }

            if (questao.indiceAlternativaCorreta < 0 || questao.indiceAlternativaCorreta > 3)
            {
                erro = $"A questão {i + 1} possui indiceAlternativaCorreta fora do intervalo de 0 a 3.";
                return false;
            }
        }

        erro = string.Empty;
        return true;
    }

    private List<QuestaoQuizNovo> MontarQuestoes(
        DadosQuizDto dados,
        out int imagensAssociadas,
        out int perguntasSemImagem,
        out int imagensAusentes)
    {
        List<QuestaoQuizNovo> questoes = new List<QuestaoQuizNovo>(dados.questoes.Count);
        string caminhoPasta = AssetDatabase.GetAssetPath(pastaDasImagens).Replace('\\', '/').TrimEnd('/');

        imagensAssociadas = 0;
        perguntasSemImagem = 0;
        imagensAusentes = 0;

        for (int i = 0; i < dados.questoes.Count; i++)
        {
            QuestaoDto origem = dados.questoes[i];
            Sprite sprite = null;

            if (string.IsNullOrWhiteSpace(origem.imagemAuxiliar))
            {
                perguntasSemImagem++;
            }
            else
            {
                string nomeArquivo = ObterNomeArquivo(origem.imagemAuxiliar);
                string caminhoImagem = $"{caminhoPasta}/{nomeArquivo}";
                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(caminhoImagem);

                if (sprite != null)
                {
                    imagensAssociadas++;
                }
                else
                {
                    imagensAusentes++;
                    Debug.LogWarning(
                        $"Questão {i + 1}: imagem '{nomeArquivo}' não encontrada em '{caminhoImagem}'.");
                }
            }

            questoes.Add(new QuestaoQuizNovo
            {
                pergunta = origem.pergunta,
                imagemAuxiliar = sprite,
                alternativaA = origem.alternativaA,
                alternativaB = origem.alternativaB,
                alternativaC = origem.alternativaC,
                alternativaD = origem.alternativaD,
                indiceAlternativaCorreta = origem.indiceAlternativaCorreta,
                justificativa = origem.justificativa
            });
        }

        return questoes;
    }

    private static string ObterNomeArquivo(string caminho)
    {
        string caminhoNormalizado = caminho.Replace('\\', '/').Trim();
        int ultimoSeparador = caminhoNormalizado.LastIndexOf('/');
        return ultimoSeparador >= 0
            ? caminhoNormalizado.Substring(ultimoSeparador + 1)
            : caminhoNormalizado;
    }

    private void RestaurarBanco(List<QuestaoQuizNovo> questoesAnteriores)
    {
        try
        {
            if (bancoDestino.questoes == null)
            {
                bancoDestino.questoes = new List<QuestaoQuizNovo>();
            }

            bancoDestino.questoes.Clear();
            bancoDestino.questoes.AddRange(questoesAnteriores);
            EditorUtility.SetDirty(bancoDestino);
            AssetDatabase.SaveAssets();
        }
        catch (Exception excecao)
        {
            Debug.LogError($"Também não foi possível restaurar o banco: {excecao.Message}");
        }
    }

    private static void MostrarResumo(
        int perguntasImportadas,
        int imagensAssociadas,
        int perguntasSemImagem,
        int imagensAusentes)
    {
        bool resultadoEsperado = perguntasImportadas == 13
            && imagensAssociadas == 11
            && perguntasSemImagem == 2
            && imagensAusentes == 0;

        string comparacao = resultadoEsperado
            ? "O resultado corresponde ao esperado."
            : "ATENÇÃO: o resultado é diferente do esperado (13 importadas, 11 imagens associadas, 2 sem imagem definida e 0 imagens ausentes).";

        string resumo =
            $"Perguntas importadas: {perguntasImportadas}\n" +
            $"Imagens associadas: {imagensAssociadas}\n" +
            $"Perguntas sem imagem definida no JSON: {perguntasSemImagem}\n" +
            $"Imagens não encontradas: {imagensAusentes}\n\n" +
            comparacao;

        Debug.Log($"Importação do Quiz Novo concluída.\n{resumo}");
        EditorUtility.DisplayDialog("Importação concluída", resumo, "OK");
    }

    private static void ExibirErro(string mensagem)
    {
        Debug.LogError($"Importador do Quiz Novo: {mensagem}");
        EditorUtility.DisplayDialog("Erro na importação", mensagem, "OK");
    }

    [Serializable]
    private class DadosQuizDto
    {
        public string nome;
        public int quantidade;
        public List<QuestaoDto> questoes;
    }

    [Serializable]
    private class QuestaoDto
    {
        public string pergunta;
        public string imagemAuxiliar;
        public string alternativaA;
        public string alternativaB;
        public string alternativaC;
        public string alternativaD;
        public int indiceAlternativaCorreta;
        public string justificativa;
    }
}
