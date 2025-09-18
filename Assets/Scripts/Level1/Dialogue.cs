using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Dialogue : MonoBehaviour
{
    public string[] dialogue;
    public int dialogueIndex;

    public GameObject dialoguePanel;
    public Text dialogueText;

    public bool startDialogue = false;
    public bool readyDialogue = false;

    private bool hideDialogueCoroutineStarted = false;

    private Animator dialoguePanelAnimator;


      
    void Start()
    {
        dialoguePanelAnimator = dialoguePanel.GetComponent<Animator>();
        if (dialoguePanelAnimator == null)
        {
            Debug.LogError("Componente Animator não encontrado no dialoguePanel.");
        }
        dialoguePanel.SetActive(false);    
    }

   
    void Update()
    {
        if(readyDialogue){
            if(!startDialogue){
                StartDialogue();
            }
            else if (dialogueText.text == dialogue[dialogueIndex]){
                StartCoroutine(HideDialogueAfterDelay(3f)); 
                hideDialogueCoroutineStarted = true;
            }
        }
        
    }

    void StartDialogue(){
        startDialogue = true;
        hideDialogueCoroutineStarted = false;
        dialoguePanel.SetActive(true);
        StartCoroutine(ShowDialogue());
    }

    IEnumerator ShowDialogue(){
        dialogueText.text = "";
        foreach (char letter in dialogue[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.025f);
        }
    }

    IEnumerator HideDialogueAfterDelay(float delay)
    {
       
        yield return new WaitForSeconds(delay);
        
        if (dialoguePanelAnimator != null)
        {
            dialoguePanelAnimator.Play("StartFadeOut");
        }
        
        yield return new WaitForSeconds(0.4f);
        dialoguePanel.SetActive(false);
        startDialogue = false;
        readyDialogue = false;
    }
}
