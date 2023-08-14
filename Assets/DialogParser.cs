using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogParser : MonoBehaviour
{
    [Header("Generic")]
    [SerializeField] public TextMeshProUGUI textMesh;
    [SerializeField] TextMeshProUGUI speakerMesh;
    public DialogGraphAsset Dialog;
    public DialogSegment CurrentSegment;
    public GameObject ChoicePanel;
    public GameObject ChoiceButtonPrefab;

    private bool starting = true;

    private void Start()
    {
        imageBasePosition = image.transform.position;
        StartCoroutine(AwaitingInputImageLoop());
        starting = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Clicked();
        }
    }

    public static DialogParser GetDialogParser()
    {
        return Camera.main.GetComponent<DialogParser>();
    }
    public void DoDialogChoice(DialogChoice dc)
    {
        ChoicePanel.SetActive(false);
        foreach (Transform t in ChoicePanel.transform)
        {
            Destroy(t.gameObject);
        }
        NextDialog(Dialog.GetSegment(dc.Link));
    }

    public void LoadNewDialog(DialogGraphAsset dialog)
    {
        Dialog = dialog;
        NextDialog(Dialog.GetFirstSegment());
    }

    public void Clicked()
    {
        //Check if starting
        if (starting)
        {
            starting = false;
            NextDialog(Dialog.GetFirstSegment());
            return;
        }
        //Check if writing
        if (writing)
            hastened = true;
        else
        {
            //Check if awaiting a choice
            if (!ChoicePanel.activeInHierarchy)
            {
                //Check if another page to go to.
                if (textMesh.pageToDisplay >= textMesh.textInfo.pageCount)
                {
                    //Go down the list, and go to the first valid segment
                    DialogChoice choice = null;
                    foreach (DialogChoice dc in CurrentSegment.GetDialogChoices())
                    {
                        if (choice != null)
                            break;
                        bool valid = true;
                        if (dc.Validator != null)
                        {
                            valid = dc.Validator.Validate();
                        }
                        if (valid)
                        {
                            choice = dc;
                        }
                    }
                    if (choice != null)
                        NextDialog(Dialog.GetSegment(choice.Link));
                }
                else
                {

                    int page = textMesh.pageToDisplay;
                    page++;
                    StartCoroutine(TextWriteout(page));
                }
            }
        }
          
    }

    public void NextDialog(DialogSegment ds)
    {
        
        //Set Text
        textMesh.SetText(ds.GetIntro());
        //set speaker
        if (!string.IsNullOrEmpty(ds.Speaker))
        {
            speakerMesh.transform.parent.gameObject.SetActive(true);
            speakerMesh.SetText(ds.Speaker);
            speakerMesh.color = ds.SpeakerColor;
        }
        else
        {
            speakerMesh.transform.parent.gameObject.SetActive(false);
        }
        textMesh.maxVisibleCharacters = 0;
        CurrentSegment = ds;
        //Do effects
        if (ds.EffectsOnEnter != null)
            ds.EffectsOnEnter.DoEffect();
        StartCoroutine(TextWriteout(1));
       
        
    }

    [Header("Writing Variables")]
    bool writing = false;
    bool hastened = false;
    public float TextTimeDelay = 0.2f;
    public float HastenedWriteoutMultiplier = 10;
    public IEnumerator TextWriteout(int page)
    {
        image.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.01f);
        writing = true;
        hastened = false;
        int charCount;
        textMesh.pageToDisplay = page;

        if (textMesh.textInfo.pageCount > 1)
        {
            charCount = textMesh.textInfo.pageInfo[page - 1].lastCharacterIndex;
            textMesh.maxVisibleCharacters = textMesh.textInfo.pageInfo[page - 1].firstCharacterIndex;
        }
        else
            charCount = textMesh.textInfo.characterCount;

        int iter = textMesh.maxVisibleCharacters-1;
        if (iter < 0)
            iter = 0;
        while (iter < charCount)
        {
            iter++;
            textMesh.maxVisibleCharacters = iter;
            float waitTime = TextTimeDelay;
            if (hastened)
                waitTime /= HastenedWriteoutMultiplier;
            yield return new WaitForSeconds(waitTime);
        }
        //Finished
        writing = false;
        //Set Display
        if (textMesh.pageToDisplay >= textMesh.textInfo.pageCount)
            SetAwaitType(AwaitType.Next);
        else if (textMesh.textInfo.pageCount > 1)
            SetAwaitType(AwaitType.Page);
        //Display Choices
        if (CurrentSegment.hasChoices && CurrentWaitType != AwaitType.Page)
        {
            SetAwaitType(AwaitType.Input);
            DisplayChoices();
        }
    }

    void DisplayChoices()
    {
        ChoicePanel.SetActive(true);
        foreach (DialogChoice dc in CurrentSegment.GetDialogChoices())
        {
            bool valid = true;
            if (dc.Validator != null)
            {
                valid = dc.Validator.Validate();
            }
            if (valid)
            {
                GameObject g = Instantiate(ChoiceButtonPrefab, ChoicePanel.transform);
                g.GetComponent<DoChoice>().SetText(dc.ChoiceText);
                g.GetComponent<DoChoice>().SetChocice(dc);
            }
        }
    }

    [Header("Images")]
    public Image image;
    public Sprite Dot1;
    public Sprite Dot2;
    public Sprite Dot3;
    public Sprite Arrow;
    void SetAwaitType(AwaitType at)
    {
        if (at == AwaitType.Next)
        {
            image.sprite = Arrow;
            image.gameObject.SetActive(true);
            image.GetComponent<Animator>().SetBool("Floating", true);
        }
        if (at == AwaitType.Page)
        {
            image.sprite = Dot1;
            dotProgress = 0;
            image.gameObject.SetActive(true);
            image.GetComponent<Animator>().SetBool("Floating", false);
        }
        if (at == AwaitType.Input)
        {
            image.gameObject.SetActive(false);
        }
        CurrentWaitType = at;
    }


    enum AwaitType {Next,Page,Input};
    AwaitType CurrentWaitType = AwaitType.Input;
    [Header("Image Animation Numbers")]
    [SerializeField] int dotProgress = 0;
    [SerializeField] float dotTickRate = 0.35f;
    Vector3 imageBasePosition;

    public IEnumerator AwaitingInputImageLoop()
    {
        while (true)
        {
            if (CurrentWaitType == AwaitType.Input)
                yield return new WaitForSeconds(0.25f);
            else if (CurrentWaitType == AwaitType.Next)
            {
                /*
                imageSway += (imageSwayRate * swayUp);
                if (imageSway > imageSwayMax)
                    swayUp = -1;
                else if (imageSway < imageSwayMin)
                    swayUp = 1;
                image.transform.position = imageBasePosition + new Vector3(0, imageSway, 0);
                yield return new WaitForEndOfFrame();
                */
                
                yield return new WaitForSeconds(.25f);
            }
            else
            {
                //Dot
                yield return new WaitForSeconds(dotTickRate);
                if (CurrentWaitType == AwaitType.Page)
                {
                    dotProgress++;
                    int dotnum = dotProgress % 3;
                    if (dotnum == 0)
                        image.sprite = Dot1;
                    else if (dotnum == 1)
                        image.sprite = Dot2;
                    else
                        image.sprite = Dot3;
                }
            }
        }
    }
}
