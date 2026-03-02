using UnityEngine;

public class ScalePulsator : MonoBehaviour
{
    [Header("Configuração de Dilatação (Por Eixo)")]
    [Tooltip("Quanto o X cresce no pico da onda (Ex: 0.5 = +50% de largura).")]
    [SerializeField] private float pulseStrengthX = 0.5f;

    [Tooltip("Quanto o Y cresce no pico da onda (Ex: 0.1 = +10% de altura).")]
    [SerializeField] private float pulseStrengthY = 0.1f;

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (TimeManager.instance != null)
        {
            // 1. Pega a onda do TimeManager
            float rawSine = TimeManager.instance.timeSineWave * -1;

            // 2. Aplica Mathf.Abs para garantir que o objeto SÓ CRESÇA (dilatação)
            // Se tirar o Abs, ele vai encolher quando a onda for negativa.
            //float positiveWave = Mathf.Abs(rawSine);

            // 3. Calcula o aumento específico para cada eixo
            // (Tamanho Original * Porcentagem de Força * Valor da Onda)
            float extraSizeX = initialScale.x * pulseStrengthX * rawSine;
            float extraSizeY = initialScale.y * pulseStrengthY * rawSine;

            // 4. Aplica a nova escala final
            transform.localScale = new Vector3(
                initialScale.x + extraSizeX, 
                initialScale.y + extraSizeY, 
                initialScale.z
            );
        }
    }
}