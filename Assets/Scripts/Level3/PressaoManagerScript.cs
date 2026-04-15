using System;
using UnityEngine;
using UnityEngine.UI;

public class PressaoManagerScript : MonoBehaviour
{
    [Tooltip("Objeto do slider da alveolar")]
    [SerializeField] private Slider slider1;
    [Tooltip("Objeto do slider da intrapleural")]
    [SerializeField] private Slider slider2;

    // flag se todas referencias estão ok no start
    // para evitar check constante no update
    private bool statusOk = false;

    private void Start()
    {
        if (TimeManager.instance)
        { 
            if (slider1 && slider2) statusOk = true;
            else Debug.LogWarning("[UI] Sliders faltando");
        }
        else Debug.LogWarning("[UI] Falha ao encontar Timer");
    }

    // Update is called once per frame
    void Update()
    {
        if (statusOk)
        {
            // Para inverter o ritmo do slider, só trocar o sinal do
            // timemanager.instance.timesizewave
            slider1.value = (-TimeManager.instance.timeSineWave + 1) / 2;
            slider2.value = 
                MathF.Min(0.5f, (-TimeManager.instance.timeSineWave + 1) / 2);
        }
    }
}
