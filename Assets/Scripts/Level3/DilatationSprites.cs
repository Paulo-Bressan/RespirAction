using UnityEngine;

public class ScalePulsator : MonoBehaviour
{
    [Header("Configuração de Dilatação (Por Eixo)")]
    [Tooltip("Quanto o X cresce no pico da onda (Ex: 0.5 = +50% de largura).")]
    [SerializeField] private float pulseStrengthX = 0.5f;

    [Tooltip("Quanto o Y cresce no pico da onda (Ex: 0.1 = +10% de altura).")]
    [SerializeField] private float pulseStrengthY = 0.1f;

    [Tooltip("Contador de progresso das conexões do diafragma")]
    [SerializeField] private int connectionCounter = 0;

    private Vector3 initialScale;

    void Start()
    {
        if (TimeManager.instance == null)
            Debug.LogWarning("[DIAFRAGMA] Time manager faltando");
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (TimeManager.instance != null)
        {
            // 1. Pega a onda do TimeManager
            float rawSine = TimeManager.instance.timeSineWave * -1;

            // 2. Pega a força original e escala de acordo com progresso do jogo
            // tecnicamente desnecessario fazer nesta parte mas fica mais claro
            float pulseX = pulseStrengthX * connectionCounter;
            float pulseY = pulseStrengthY * connectionCounter;

            // 3. Aplica Mathf.Abs para garantir que o objeto SÓ CRESÇA (dilatação)
            // Se tirar o Abs, ele vai encolher quando a onda for negativa.
            //float positiveWave = Mathf.Abs(rawSine);

            // 4. Calcula o aumento específico para cada eixo
            // (Tamanho Original * Porcentagem de Força * Valor da Onda)
            float extraSizeX = initialScale.x * pulseX * rawSine;
            float extraSizeY = initialScale.y * pulseY * rawSine;

            // 5. Aplica a nova escala final
            transform.localScale = new Vector3(
                initialScale.x + extraSizeX, 
                initialScale.y + extraSizeY, 
                initialScale.z
            );
        }
    }

    public void toggleConnection()
    {
        // é chamado externamente quando uma conexão final é feita
        // para de reagir quando tem duas conexões para evitar
        // adições infinitas. O connectioncounter é usado para determinar
        // a velocidade de movimento do diafragma no Update()

        if (connectionCounter == 0)
            connectionCounter = 1;
        else if (connectionCounter == 1)
            connectionCounter = 2;
        else { } // nada
    }
}