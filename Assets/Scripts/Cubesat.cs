using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Cubesat : MonoBehaviour 
{
    [System.Serializable]
    public class DialogStep
    {
        [Multiline]
        public string dialog;
        [Tooltip("If null he don't move")]
        [Space]
        public Transform parkPos;
        [Tooltip("What to look at during this step")]
        public Transform lookTarget;
        [Space]
        [Tooltip("If false cubesat will wait to talk until he reaches parkPos, otherwise he just yaps")]
        public bool moveWhileTalking = true;
        [Tooltip("How fast he yap")]
        public float charactersPerSecond = 40;
        [Tooltip("How fast he zoom (meeters/seconds)")]
        public float movespeed = 3;
        [Tooltip("Cubesat plays this sound at the same moment that dialog starts typing")]
        public AudioClip voiceline;
        [Space]

        [Tooltip("This dialog step won't end until Confirm() is called")]
        public bool waitForConfirm = true;
        public UnityEvent onWaitingForConfirm;
        [Space]

        public UnityEvent onDialogBegin;
        public UnityEvent onDialogEnd;
    }

    [Serializable]
    public class DialogPath
    {
        [Tooltip("This is what you pass into \"StartDialog()\"")]
        public string pathName;
        public List<DialogStep> steps;
        [Tooltip("This gets called when a dialog path is finished or cancled. String is the path name, bool is if it was cancled (in case we want to check if one finished)")]
        public UnityEvent<string, bool> onDialogEnd;
    }

    public AudioSource AudioSource;
    public TextMeshProUGUI text;

    private DialogPath currentDialogPath;
    private Coroutine currentDialogPathRoutine;

    [Tooltip("Degrees per second to turn")]
    public float headTurnSpeed = 180;
    public Transform currentLookTarget;


    public bool waitingForConfirm;
    [Space]

    [Tooltip("All the dialog that cubsat can ever say, he has no free will")]
    public List<DialogPath> dialogPaths;
    public void Start()
    {
        waitingForConfirm = false;
    }

    public void Update()
    {
        if (currentLookTarget != null)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(currentLookTarget.position - transform.position), headTurnSpeed * Time.deltaTime);
        }
    }

    public void StartDialog(string pathName)
    {
        if (currentDialogPathRoutine != null)
        {
            StopCoroutine(currentDialogPathRoutine);
            currentDialogPathRoutine = null;
            currentDialogPath?.onDialogEnd?.Invoke(currentDialogPath.pathName, true);
            currentDialogPath = null;
        }

        foreach (var path in dialogPaths)
        {
            Debug.Log(path.pathName + " == " + pathName);
            if (path.pathName == pathName)
            {
                currentDialogPathRoutine = StartCoroutine(ExecuteDialogPath(path));
                return;
            }
        }
        Debug.LogError($"Dialog path {pathName} was not found!");

    }

    IEnumerator ExecuteDialogStep(DialogStep step)
    {
        currentLookTarget = step.lookTarget;

        // Start talking if not waiting on move
        float talkStart = Time.time;
        if(step.moveWhileTalking)
        {
            if(step.voiceline)
            {
                AudioSource.clip = step.voiceline;
                AudioSource.Play();
            }
            step.onDialogBegin?.Invoke();
        }



        if(step.parkPos != null)
        {
            do
            {
                // Smooth damp later
                if (step.lookTarget)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(step.lookTarget.position - transform.position), headTurnSpeed * Time.deltaTime);
                }

                if (step.moveWhileTalking)
                {
                    AnimateText(step.dialog, step.charactersPerSecond, Time.time - talkStart);
                }

                transform.position = Vector3.MoveTowards(transform.position, step.parkPos.transform.position, step.movespeed * Time.deltaTime);

                yield return null;
            }
            while (transform.position != step.parkPos.transform.position);

        }

        if (step.lookTarget != null)
        {
            while (transform.rotation != Quaternion.LookRotation(step.lookTarget.position - transform.position))
            {
                if (step.moveWhileTalking)
                {
                    AnimateText(step.dialog, step.charactersPerSecond, Time.time - talkStart);
                }
                yield return null;
            }
        }


        if (!step.moveWhileTalking)
        {
            talkStart = Time.time;
            if (step.voiceline)
            {
                AudioSource.clip = step.voiceline;
                AudioSource.Play();
            }
            step.onDialogBegin?.Invoke();
        }
        Debug.Log(Time.time - talkStart);
      
        while (Time.time - talkStart - 1 < (step.dialog.Length / step.charactersPerSecond))
        {
            Debug.Log("Dialog length: " + (step.dialog.Length / step.charactersPerSecond));
            AnimateText(step.dialog, step.charactersPerSecond, Time.time - talkStart);
            yield return null;
        }

        text.text = step.dialog;

        if (step.waitForConfirm)
        {
            waitingForConfirm = true;
            step.onWaitingForConfirm?.Invoke();
            while(waitingForConfirm)
            {
                yield return null;
            }
        }

        step.onDialogEnd?.Invoke();
    }

    IEnumerator ExecuteDialogPath(DialogPath path)
    {

        currentDialogPath = path;
        foreach (var dialogStep in path.steps)
        {
            yield return ExecuteDialogStep(dialogStep);
        }

        path.onDialogEnd?.Invoke(path.pathName, false);

        currentDialogPath = null;
    }



    void AnimateText(string dialog, float charactersPerSecond, float currentTime)
    {
        Debug.Log("printing chars" + currentTime * charactersPerSecond + " dialog time: " + (currentTime / charactersPerSecond));
        text.text = dialog.Substring(0, (int)Mathf.Min( (currentTime * charactersPerSecond), dialog.Length));
    }
}
