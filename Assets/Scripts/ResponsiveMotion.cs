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
    enum Phase { Start, Walking, InFront, End };
    Phase currPhase = Phase.Start;

    // Where the player is looking
    enum GazePosition { Left, Centre, Right, None };

    public FoveInterface2 fove;

    public Transform CharacterStart;
    public Transform CharacterInFront;

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
            case Phase.InFront:
                InFrontUpdate();
                break;
            case Phase.End:
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
        
        if (timeLookingAtModel >= 1f)
        {
            Invoke("AnimStartLooking", 0f);
            StartCoroutine(ChangeSitting(true, 2f));

            Invoke("AnimGetUp", 8f);
            StartCoroutine(WalkToPlayer(12f));

            currPhase = Phase.Walking;
        }

        timeSinceLast = 0f;
    }

    private void InFrontUpdate()
    {
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

    private IEnumerator WalkToPlayer(float time)
    {
        yield return new WaitForSeconds(time);

        float startTime = Time.time;
        float changeInTime = 0f;
        animator.SetFloat(speedHash, .5f);

        while (changeInTime < 5f)
        {
            changeInTime = Time.time - startTime;
            currPos = Vector3.Lerp(CharacterStart.position, CharacterInFront.position, changeInTime / 5f);
            yield return null;
        }

        animator.SetFloat(speedHash, 0f);

        yield return new WaitForSeconds(1f);
        currPhase = Phase.InFront;
    }

    private IEnumerator ChangeSitting(bool sitStraight, float startTime)
    {
        yield return new WaitForSeconds(startTime);
        for (float f = sitStraight ? 0f : 1f; sitStraight ? f >= 1 : f <= 0;)
        {
            animator.SetFloat(sitStyleHash, f);
            f += .3f * (sitStraight ? Time.deltaTime : -Time.deltaTime);
            yield return null;
        }
    }

    void AnimGetUp()
    {
        animator.SetTrigger(getUpHash);
    }

    void AnimSitDown()
    {
        animator.SetTrigger(sitDownHash);
    }

    void AnimStartLooking()
    {
        lookingActive = true;
        timeOfLookingChange = Time.time;
        justChangedLooking = true;
    }

    void AnimStopLooking()
    {
        lookingActive = false;
        timeOfLookingChange = Time.time;
        justChangedLooking = true;
    }
}
