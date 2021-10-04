using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrintOutText : MonoBehaviour
{
    [Tooltip("time it takes to print 1 char")]
    [SerializeField] float printSpeed, startDelay,endDelay;
   
    [SerializeField] string missingLetter;
    [Tooltip("use a time instead of a speed")]
    [SerializeField] bool timed, deactivateOnEnd;
    [SerializeField] float timeToPrint;
    TextMesh textMeshComp;
    AudioSource audioRef;
    Text textComp; //if you're using ui text
    bool ui;
    string storedText;
    private void Awake()
    {
        textMeshComp = GetComponent<TextMesh>();
        textComp = GetComponent<Text>();

        audioRef = GetComponent<AudioSource>();

        if (textComp)
        {
            ui = true;
            storedText = textComp.text;
            textComp.text = "";
            if (timed)
            {
                printSpeed = timeToPrint / storedText.Length;
            }
        }
        else
        {
            //otherwise use the text mesh
            storedText = textMeshComp.text;
            textMeshComp.text = "";
            if (timed)
            {
                printSpeed = timeToPrint / storedText.Length;
            }
        }
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        if (textComp)
        {
            textComp.text = "";
            StartCoroutine(PrintText());
        }
        else
        {
            //otherwise use the text mesh
            textMeshComp.text = "";
            StartCoroutine(PrintTextMesh());
        }

    }

    IEnumerator PrintTextMesh()
    {
        yield return new WaitForSeconds(startDelay);
        int i = 0;
        while (textMeshComp.text.Length < storedText.Length)
        {
            if (storedText[i].ToString() == missingLetter)
            {
                textMeshComp.text += " ";
            }
            else
            {
                textMeshComp.text += storedText[i];
                if (audioRef)
                {
                    audioRef.pitch = Random.Range(.7f, 1.2f);
                    audioRef.Play();
                }
            }
            i++;
            yield return new WaitForSeconds(printSpeed);
        }
        yield return new WaitForSeconds(endDelay);

        //animates out
        if(deactivateOnEnd) transform.parent.gameObject.SetActive(false);
    }
    IEnumerator PrintText()
    {
        yield return new WaitForSeconds(startDelay);
        int i = 0;
        while (textComp.text.Length <storedText.Length)
        {
            if (storedText[i].ToString() == missingLetter)
            {
                textComp.text += " ";
            }
            else
            {
                textComp.text += storedText[i];
                if (audioRef)
                {
                    audioRef.pitch = Random.Range(.7f, 1.2f);
                    audioRef.Play();
                }
            }
            i++;
            yield return new WaitForSeconds(printSpeed);
        }

        yield return new WaitForSeconds(endDelay);
        //animates out
        if (deactivateOnEnd) transform.root.gameObject.SetActive(false);
    }
}
