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

    public Rigidbody rb;
    public float deadzone = 0.1f;
    public float acceleration = 20;
    public float maxSpeed = 10;
    public float maxAngularSpeed = 360;
    [Space]
    public AudioSource AudioSource;
    public TextMeshProUGUI text;

    private DialogPath currentDialogPath;
    private Coroutine currentDialogPathRoutine;

    [Tooltip("Degrees per second to turn")]
    public float headTurnSpeed = 180;
    public Transform currentPosTarget;
    public Transform currentLookTarget;

    // talking sounds
    public AudioClip[] talkingSamples;
    private bool talkingIsPlaying = false;

    // levitating sounds
    public AudioSource levitateSource;
    public AudioSource compressedAirSource;
    public float maxVolume = 1.0f;
    private bool levitateIsPlaying = false;
    

    public bool waitingForConfirm;
    [Space]

    [Tooltip("All the dialog that cubsat can ever say, he has no free will")]
    public List<DialogPath> dialogPaths;

    public Vector3 targetPos;
    public Quaternion targetRot;
    public void Start()
    {
        waitingForConfirm = false;
    }

    public void FixedUpdate()
    {
        if(currentPosTarget) 
            targetPos = currentPosTarget.position;
        if (currentLookTarget)
            targetRot = Quaternion.LookRotation(currentLookTarget.position - transform.position);

        rb.AddForce(CalcForceVec(targetPos, rb.position, rb.velocity, maxSpeed, acceleration, deadzone), ForceMode.Acceleration);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, maxAngularSpeed * Time.fixedDeltaTime));
    
    
        float speed = rb.velocity.magnitude;

        if (speed > 0.4f)
        {
            if (!levitateIsPlaying)
            {
                compressedAirSource.Play();
                levitateIsPlaying = true;
            }

            compressedAirSource.volume = Mathf.Clamp(speed / maxSpeed, 0f, maxVolume);
        }
        else
        {
            if (levitateIsPlaying)
            {
                compressedAirSource.Pause();
                levitateIsPlaying = false;
            }
        }
    }

    Vector3 CalcForceVec(Vector3 targetPos, Vector3 currentPos, Vector3 currentVelocity, float maxSpeed, float maxAcceleration, float deadzone)
    {
        var componentForce = new Vector3
        {
            x = CalcForce(targetPos.x, currentPos.x, currentVelocity.x, maxSpeed, maxAcceleration, deadzone),
            y = CalcForce(targetPos.y, currentPos.y, currentVelocity.y, maxSpeed, maxAcceleration, deadzone),
            z = CalcForce(targetPos.z, currentPos.z, currentVelocity.z, maxSpeed, maxAcceleration, deadzone)
        };

        return Vector3.ClampMagnitude(componentForce, maxAcceleration);
    }

    float CalcForce(float targetPos, float currentPos, float currentVelocity, float maxSpeed, float maxAcceleration, float deadzone)
    {
        float posDelta = targetPos - currentPos;
        float arriveTime = Mathf.Abs(posDelta) / Mathf.Abs(currentVelocity);
        float accelerateTime = Mathf.Abs(currentVelocity) / maxAcceleration;

        float perfectVelocity = Mathf.Sign(posDelta) * Mathf.Min(Mathf.Abs(posDelta) * maxSpeed, maxSpeed);
        float etaDiff = arriveTime - (accelerateTime);
        if (etaDiff < 0)
            perfectVelocity *= 0.001f;


        float velocityDelta = Mathf.Sign(perfectVelocity - currentVelocity);
        float acceleration = Mathf.Sign(velocityDelta) * maxAcceleration * Mathf.Clamp01(Mathf.Abs(posDelta) / deadzone);


        return acceleration;
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
        currentPosTarget = step.parkPos;

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

                if (step.moveWhileTalking)
                {
                    AnimateText(step.dialog, step.charactersPerSecond, Time.time - talkStart);
                }

                yield return null;
            }
            while ((transform.position - step.parkPos.transform.position).magnitude > 0.05f);

        }

        if (step.lookTarget != null)
        {
            while (Quaternion.Angle(transform.rotation,  Quaternion.LookRotation(step.lookTarget.position - transform.position)) > 2f)
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
      
        if (!step.voiceline)
        {
            StartRandomPlayback();
        }

        while (Time.time - talkStart - 1 < (step.dialog.Length / step.charactersPerSecond))
        {
            AnimateText(step.dialog, step.charactersPerSecond, Time.time - talkStart);
            yield return null;
        }

        if (!step.voiceline)
        {
            StopRandomPlayback();
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
        text.text = dialog.Substring(0, (int)Mathf.Min( (currentTime * charactersPerSecond), dialog.Length));
    }

    public void StartRandomPlayback()
    {
        if (!talkingIsPlaying)
        {
            talkingIsPlaying = true;
            StartCoroutine(PlayRandomClips());
        }
    }

    public void StopRandomPlayback()
    {
        talkingIsPlaying = false;
        if (AudioSource.isPlaying)
        {
            AudioSource.Stop();
        }
    }

    private IEnumerator PlayRandomClips()
    {
        while (talkingIsPlaying)
        {
            if (talkingSamples.Length > 0)
            {
                // Choose a random clip
                AudioClip randomClip = talkingSamples[UnityEngine.Random.Range(0, talkingSamples.Length)];
                AudioSource.clip = randomClip;
                AudioSource.Play();

                // Wait for the clip to finish before playing the next
                yield return new WaitForSeconds(randomClip.length);
            }
            else
            {
                Debug.LogWarning("No audio clips assigned!");
                yield break; // Exit the coroutine if there are no clips
            }
        }
    }
}
