using UnityEngine;

public class PressaoManagerScript : MonoBehaviour
{
    public GameObject medidor;

    private Vector3 fixedScale;

    private float localHeight;
    public float heightRatio;

    private void Start()
    {
        fixedScale = medidor.transform.localScale;
        localHeight = fixedScale.z;

        if (heightRatio == 0) heightRatio = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeManager.instance)
        {
            localHeight = (fixedScale.z *  (1 - heightRatio)) + 
                          (fixedScale.z * ((TimeManager.instance.timeSineWave + 1) / 2) * heightRatio);

            medidor.transform.localScale = 
                new Vector3(fixedScale.x, fixedScale.y, localHeight);
        }
    }
}
