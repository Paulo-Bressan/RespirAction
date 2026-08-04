using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestaoQuizNovo
{
    [TextArea] public string pergunta;
    public Sprite imagemAuxiliar;
    [TextArea] public string alternativaA;
    [TextArea] public string alternativaB;
    [TextArea] public string alternativaC;
    [TextArea] public string alternativaD;
    [Range(0, 3)] public int indiceAlternativaCorreta;
    [TextArea] public string justificativa;

    public string ObterAlternativa(int indice)
    {
        switch (indice)
        {
            case 0:
                return alternativaA;
            case 1:
                return alternativaB;
            case 2:
                return alternativaC;
            case 3:
                return alternativaD;
            default:
                return string.Empty;
        }
    }
}

[CreateAssetMenu(
    fileName = "BancoQuizNovo",
    menuName = "RespirAction/Quiz Novo/Banco de Perguntas")]
public class QuizNovoDatabase : ScriptableObject
{
    public List<QuestaoQuizNovo> questoes = new List<QuestaoQuizNovo>();
}
