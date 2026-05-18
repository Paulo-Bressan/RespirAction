using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PressaoManagerManual : MonoBehaviour
{
    [Tooltip("Objeto do slider da alveolar")]
    [SerializeField] private Slider slider1;
    [Tooltip("Objeto do slider da intrapleural")]
    [SerializeField] private Slider slider2;

    // flag se todas referencias estão ok no start
    // para evitar check constante no update
    private bool statusOk = false;    

    // flag do estado de animacao
    // o que cada numero significa esta elaborado dentro de Update()
    private int animStatus = 0;

    // variaveis para ra evitar a criacao de var local o tempo todo
    private float valorBarra = 0;
    private float diff = 0;

    // marca tempo para registrar diferenças (ex: entre clicar e solta)
    private float marcadorTempo = 0;
    private float diffAbsoluta = 0;
    
    // variavel auxiliar para guardar tempo anterior
    private float valorAnt = 0;

    // eventos para logica de jogo
    public UnityEvent respCedo;
    public UnityEvent respTarde;
    public UnityEvent respIdeal;


    void Start()
    {
        if (TimeManager.instance)
        {
            if (slider1 && slider2) statusOk = true;
            else Debug.LogWarning("[UI] Sliders faltando");
        }
        else Debug.LogWarning("[UI] Falha ao encontar Timer");
    }

    void Update()
    {
        if (statusOk)
            switch (animStatus)
            {
                case 0:
                    // sem animacao  ----------------------------
                    slider1.value = 0.5f;
                    slider2.value = 0.5f;
                    break;

                case 1:
                    // inspiracao -------------------------------

                    // conta tempo de 0 a 5 segundos a partir do instante do click
                    diffAbsoluta = TimeManager.instance.elapsedTime - marcadorTempo;
                    diff = MathF.Min(5, diffAbsoluta);

                    // barra em si é 0.5 menos os segundos dividos por 10 (logo vai de 0.5 a 0 em 5 segundos)
                    valorBarra = 0.5f - (diff / 10);

                    slider1.value = valorBarra;
                    slider2.value = valorBarra;
                    valorAnt = valorBarra;
                    break;

                case 2:
                    // expiracao  -------------------------------

                    // conta tempo de 0 a 5 segundos a partir do instante de soltar
                    diff = MathF.Min(5, (TimeManager.instance.elapsedTime - marcadorTempo));

                    // barra em si é os segundos divididos por 5
                    // ex: se valorAnt é 0, a barra vai de 0 até 1 em 5 segundos
                    valorBarra = MathF.Min(1, valorAnt + (diff / 5));

                    slider1.value = valorBarra;
                    slider2.value = MathF.Min(0.5f, valorBarra);

                    // progressao para proxima etapa qnd barra chegar no inverso do anterior
                    if (valorBarra >= (1 - valorAnt))
                    {
                        Debug.Log("[PRESSAO] Barra no limite superior, trocando animStatus para 3");
                        marcadorTempo = TimeManager.instance.elapsedTime;
                        valorAnt = valorBarra;
                        animStatus = 3;
                    }
                    break;

                case 3:
                    // retorno ao neutro --------------------------

                    // conta tempo de 0 a 1 segundos
                    diff = MathF.Min(1, (TimeManager.instance.elapsedTime - marcadorTempo));
                    
                    // barra vai de 1 a 0.5 em 1 segundo
                    valorBarra = valorAnt - (diff / 2);

                    slider1.value = valorBarra;

                    // progressao para proxima etapa qnd barra chegar no 0.5
                    if (valorBarra <= 0.5f)
                    {
                        Debug.Log("[PRESSAO] Barra no 0.5, trocando animStatus para 0");
                        animStatus = 0;
                    }
                    break;
            }
        else
        {
            slider1.value = 0.5f;
            slider2.value = 0.5f;
        }
    }

    public void botaoPrecionado()
    {
        Debug.Log("[PRESSAO] Mouse precionado, trocando animStatus para 1");
        marcadorTempo = TimeManager.instance.elapsedTime;
        animStatus = 1;
    }

    public void botaoSolto()
    {
        Debug.Log("[PRESSAO] Mouse solto, trocando animStatus para 2");
        marcadorTempo = TimeManager.instance.elapsedTime;
        animStatus = 2;

        // logica da fase
        if (valorAnt > 0.2)
        {
            Debug.Log("[PRESSAO] Mouse solto cedo, chamando evento RespCedo");
            respCedo.Invoke();
        }
        else if (diffAbsoluta > 7.0f)
        {
            Debug.Log("[PRESSAO] Mouse solto tarde, chamando evento RespTarde");
            respTarde.Invoke();
        }
        else
        {
            Debug.Log("[PRESSAO] Mouse solto ideal, chamando evento RespIdeal");
            respIdeal.Invoke();
        }

    }
}
