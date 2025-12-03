using TMPro;
using UnityEngine;

public class LevelManager3 : MonoBehaviour
{
    public float timerLength;
    private float remainingTime;

    public TextMeshProUGUI timerText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (timerLength == 0) timerLength = 300f;
        remainingTime = timerLength;
    }

    // Update is called once per frame
    void Update()
    {
        if (remainingTime > 0)
            remainingTime = timerLength - TimeManager.instance.elapsedTime;

        if (TimeManager.instance || timerText)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else Debug.Log("Falta alguma associacao em algum lugar");
    }

    /*
    Receita de miojo para quem nao sabia

    1 Coloque o fogo para ferver.
    2 Em seguida coloque as 4 colheres de catchup e mexa.
    3 Depois quando a água estiver fervendo ponhe o miojo.
    4 Em seguida coloque o tempero e as colheres de pimenta.
    5 Depois rale a mussarela em cima do miojo.
    6 Bom apetite!
     */
}
