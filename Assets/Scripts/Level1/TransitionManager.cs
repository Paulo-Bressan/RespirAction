using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public GameObject diafragma;
    //public GameObject bgUI;
    //private SpriteRenderer bgUIRender;

    public GameObject firstDialogue;
    public GameObject secondDialogue;

    public GameObject continueButton1;
    public GameObject continueButton2;

    public GameObject audioManager;


    public bool playAnimation;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playAnimation = true;
        targetPosition = new Vector3(0.1f, 3, -0.3f);
        targetRotation = Quaternion.AngleAxis(5, Vector3.up) * diafragma.transform.rotation;

        secondDialogue.SetActive (false);
        continueButton2.SetActive(false);

        audioManager.GetComponent<AudioManagerScene>().PlaySound(0);

        //bgUIRender = bgUI.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playAnimation)
        {
            diafragma.transform.position = Vector3.Lerp(diafragma.transform.position, targetPosition, 0.05f);
            diafragma.transform.rotation = Quaternion.Slerp(diafragma.transform.rotation, targetRotation, 0.05f);
            //bgUIRender.color = Color.Lerp(Color.clear, Color.black, 0.05f);

            if (Vector3.Distance(diafragma.transform.position, targetPosition) < 0.01f)
            {
                playAnimation = false;
                transform.position = targetPosition;
            }
        }
    }

    public void openSecondDialogue()
    {
        firstDialogue.SetActive(false);
        secondDialogue.SetActive(true);

        continueButton1.SetActive(false);
        continueButton2.SetActive (true);

        audioManager.GetComponent<AudioManagerScene>().PlaySound(1);

    }
}
