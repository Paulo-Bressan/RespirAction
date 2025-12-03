using UnityEngine;
using UnityEngine.UI;

public class PressaoManagerScript : MonoBehaviour
{
    public Slider slider1;
    public Slider slider2;

    /*
    private Vector3 fixedScale;
    private float localHeight;
    public float heightRatio;
    */

    private void Start()
    {
        /*
        fixedScale = medidor.transform.localScale;
        localHeight = fixedScale.z;

        if (heightRatio == 0) heightRatio = 0.5f;
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeManager.instance)
        {
            /*
            localHeight = (fixedScale.z *  (1 - heightRatio)) + 
                          (fixedScale.z * ((TimeManager.instance.timeSineWave + 1) / 2) * heightRatio);

            medidor.transform.localScale = 
                new Vector3(fixedScale.x, fixedScale.y, localHeight);
            */

            if (slider1 && slider2)
            {
                slider1.value = (TimeManager.instance.timeSineWave + 1) / 2;
                slider2.value = (TimeManager.instance.timeSineWave + 1) / 2;
            }
            else Debug.Log("Sliders faltando");
        }
    }
}
