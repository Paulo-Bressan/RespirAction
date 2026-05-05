using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ManagerVideoRespiracao : MonoBehaviour
{
    [Tooltip("Componente de VideoPlayer")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Tooltip("Objeto do botao de inspiracao")]
    [SerializeField] private GameObject botaoInsp;

    [Tooltip("Asset do video de inspiracao")]
    [SerializeField] private VideoClip videoInspiracao;

    [Tooltip("Asset do video de expiracao")]
    [SerializeField] private VideoClip videoExpiracao;

    // flag se animacao de inspiracao comecou
    private bool isBreathing = false;

    // marcador de tempo
    private float marcadorTempo = 0;

    void Start()
    {
        // Certifica-se que a biblioteca UnityEngine.Video está no seu projeto.
        if (!videoPlayer || !TimeManager.instance)
        {
            Debug.LogError("[CUTSCENE] VideoPlayer não está configurado ou TimeManager não implementado.");
            return;
        }

        if (!botaoInsp)
            Debug.LogWarning("[CUTSCENE] Botao de inspiracao faltando");
        if (!videoInspiracao)
            Debug.LogWarning("[CUTSCENE] Video de inspiracao faltando");
        if (!videoExpiracao)
            Debug.LogWarning("[CUTSCENE] Video de expiracao faltando");
    }

    // funcao que cuida da animacao em si
    // id corresponde a animacao:
    // 1 = inspiracao / 2 = expiracao
    // tempo eh o tempo de inicio da animacao
    private void tocarAnimacao(int id, float tempo)
    {
        switch (id)
        {
            case 1:
                Debug.Log("[CUTSCENE] Tocando animação de inspiração");
                videoPlayer.clip = videoInspiracao;
                break;
            case 2:
                Debug.Log("[CUTSCENE] Tocando animação de expiração");
                videoPlayer.clip = videoExpiracao;
                break;
        }

        videoPlayer.time = tempo;
        videoPlayer.Play();
    }

    public void botaoPrecionado()
    {
        Debug.Log("[CUTSCENE] Mouse precionado");
        marcadorTempo = TimeManager.instance.elapsedTime;
        tocarAnimacao(1, 0);
    }

    public void botaoSolto()
    {
        Debug.Log("[CUTSCENE] Mouse solto");

        float diferencaTempo = TimeManager.instance.elapsedTime - marcadorTempo;
        Debug.Log("[CUTSCENE] Diferença de tempo (s) entre clicar e soltar = " + diferencaTempo);

        // LEIA ME
        // inserir aqui algum evento se a diferença de tempo for, por exemplo, >= 5
        // isto significa que a pessoa respirou direto
        // logo podemos progredir em um contador para alguma coisa ou sei la

        tocarAnimacao(2, 5.2f - diferencaTempo);
    }
}