using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponsiveMotion : MonoBehaviour
{

    Animator animator;

    private readonly int getUpHash = Animator.StringToHash("GetUp");
    private readonly int sitDownHash = Animator.StringToHash("SitDown");
    private readonly int sitStyleHash = Animator.StringToHash("SitStyle");
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int lrHash = Animator.StringToHash("LeftRight");
    private readonly int turnMoveHash = Animator.StringToHash("TurnOrMove");

    float speed = 0;
    float lr = 0;
    float turnMove = 1;

    bool lookingActive = false;
    float timeOfLookingChange = 0f;
    bool justChangedLooking = false;

    public Transform playerLoc;

    // For start
    float timeSinceLast = 0f;
    float timeLookingAtModel = 0f;
    
    // What phase we're in
    enum Phase { Start, WalkingTo, InFront, ToCorner, End, None };
    Phase currPhase = Phase.Start;

    // For in front
    public float inFrontTime = 20f;

    // Where the player is looking
    enum GazePosition { Left, Centre, Right, None };

    public FoveInterface2 fove;

    public Transform CharStartLoc;
    public Transform CharFrontLoc;
    public Transform CharCornerLoc;
    public Transform CharEndLoc;


    Vector3 currPos;

    // Start - finds animator
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.SetLookAtPosition(playerLoc.position);

        currPos = gameObject.transform.position;
    }

    private void Update()
    {
        switch (currPhase)
        {
            case Phase.Start:
                StartUpdate();
                break;
            case Phase.WalkingTo:
                WalkingToUpdate();
                break;
            case Phase.InFront:
                InFrontUpdate();
                break;
            case Phase.ToCorner:
                ToCornerUpdate();
                break;
            case Phase.End:
                EndUpdate();
                break;
            case Phase.None:
                break;
        }
    }

    private void StartUpdate()
    {
        timeSinceLast += Time.deltaTime;
        if (timeSinceLast < .1f)
            return;

        GazePosition gaze = GetGaze();

        if (gaze == GazePosition.Centre || gaze == GazePosition.Right)
            timeLookingAtModel += timeSinceLast;
        
        if (timeLookingAtModel >= 2f)
        {
            lookingActive = true;
            timeOfLookingChange = Time.time;
            justChangedLooking = true;
            
            StartCoroutine(SitUp(2f));

            Invoke("AnimGetUp", 4f);
            StartCoroutine(WalkTo(CharFrontLoc.position, 6f, 5f, Phase.InFront));

            currPhase = Phase.WalkingTo;
        }

        timeSinceLast = 0f;
    }

    private void WalkingToUpdate()
    {
    }

    private void InFrontUpdate()
    {
        inFrontTime -= Time.deltaTime;
        if (inFrontTime <= 0)
        {
            currPhase = Phase.ToCorner;
            WalkTo(CharCornerLoc.position, 2f, 4f, Phase.ToCorner);
            WalkTo(CharEndLoc.position, 6f, 4f, Phase.End);
        }

        GazePosition gaze = GetGaze();

        if (gaze == GazePosition.Left)
        {
            animator.SetFloat(speedHash, .2f);
            currPos = gameObject.transform.position + (Vector3.forward * .2f * Time.deltaTime);
        }
        else if (gaze == GazePosition.Right)
        {
            animator.SetFloat(speedHash, .2f);
            currPos = Vector3.Lerp(gameObject.transform.position, gameObject.transform.position + Vector3.back, .2f * Time.deltaTime);
        }
    }

    private void ToCornerUpdate()
    {

    }

    private void EndUpdate()
    {
        //Send something to the upper  level
    }

    private GazePosition GetGaze()
    {
        Collider hit;
        if (!fove.Gazecast(1 << 8, out hit)) 
            return GazePosition.None;

        switch (hit.gameObject.name)
        {
            case "L Collider":
                return GazePosition.Left;
            case "R Collider":
                return GazePosition.Right;
            default:
                return GazePosition.Centre;
        }
    }

    private void OnAnimatorMove()
    {
        gameObject.transform.position = currPos;
    }

    // If lookingActive is true, will look at the player
    public void OnAnimatorIK(int layerIndex)
    {
        if (justChangedLooking)
        {
            float changeInTime = Time.time - timeOfLookingChange;
            if (lookingActive)
            {
                if (changeInTime < 2f)
                {
                    animator.SetLookAtWeight(Mathf.Lerp(0, 1, changeInTime / 2f));
                }
                else
                {
                    animator.SetLookAtWeight(1);
                    justChangedLooking = false;
                }
            }
            else
            {
                if (changeInTime < 2f)
                {
                    animator.SetLookAtWeight(Mathf.Lerp(1, 0, changeInTime / 2f));
                }
                else
                {
                    animator.SetLookAtWeight(0);
                    justChangedLooking = false;
                }
            }
        }
        else
        {
            if (lookingActive)
            {
                animator.SetLookAtWeight(1);
            }
            else
            {
                animator.SetLookAtWeight(0);
            }
        }
    }

    private IEnumerator WalkTo(Vector3 endPos, float holdTime, float time, Phase endPhase)
    {
        yield return new WaitForSeconds(holdTime);
        Vector3 startPos = gameObject.transform.position;

        float startTime = Time.time;
        float changeInTime = 0f;
        animator.SetFloat(speedHash, .5f);

        while (changeInTime < time)
        {
            changeInTime = Time.time - startTime;
            currPos = Vector3.Lerp(startPos, endPos, changeInTime / time);
            yield return null;
        }

        animator.SetFloat(speedHash, 0f);
        currPhase = endPhase != Phase.None ? endPhase : currPhase;
    }

    private IEnumerator SitUp(float startTime)
    {
        yield return new WaitForSeconds(startTime);
        for (float f = 0f; f >= 1; f+= .3f * Time.deltaTime)
        {
            animator.SetFloat(sitStyleHash, f);
            yield return null;
        }
    }

    void AnimGetUp()
    {
        animator.SetTrigger(getUpHash);
    }

    void AnimStopLooking()
    {
        lookingActive = false;
        timeOfLookingChange = Time.time;
        justChangedLooking = true;
    }
}