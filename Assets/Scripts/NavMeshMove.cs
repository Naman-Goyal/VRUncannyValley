using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMove : MonoBehaviour
{

    Animator animator;

    private readonly int getUpHash = Animator.StringToHash("GetUp");
    private readonly int sitDownHash = Animator.StringToHash("SitDown");
    private readonly int sitStyleHash = Animator.StringToHash("SitStyle");
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int lrHash = Animator.StringToHash("LeftRight");
    private readonly int turnMoveHash = Animator.StringToHash("TurnOrMove");

    bool lookingActive = false;
    float timeOfLookingChange = 0f;
    bool justChangedLooking = false;

    public Transform playerLoc;

    // For start
    float timeSinceLast = 0f;
    float timeLookingAtModel = 0f;

    // What phase we're in
    public enum Phase { Start, WalkingTo, InFront, ToCorner, End, None };
    [System.NonSerialized]
    public Phase currPhase = Phase.Start;

    // For in front
    public float inFrontTime = 20f;

    public float locationBuffer = 1f;

    // Where the player is looking
    enum GazePosition { Left, Centre, Right, None };

    public FoveInterface2 fove;

    public Transform CharStartLoc;
    public Transform CharFrontLoc;
    public Transform CharCornerLoc;
    public Transform CharEndLoc;

    private NavMeshAgent nma;


    Vector3 currPos;

    // Start - finds animator
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.SetLookAtPosition(playerLoc.position);

        currPos = gameObject.transform.position;
        nma = gameObject.transform.parent.GetComponent<NavMeshAgent>();
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

        if (timeLookingAtModel >= .6f)
        {
            lookingActive = true;
            timeOfLookingChange = Time.time;
            justChangedLooking = true;

            StartCoroutine(SitUp(2f));

            Invoke("AnimGetUp", 4f);
            nma.destination = CharFrontLoc.position;
            currPhase = Phase.WalkingTo;
        }

        timeSinceLast = 0f;
    }

    private void WalkingToUpdate()
    {
        //see if close enough
        if (Vector3.Distance(gameObject.transform.position, CharFrontLoc.position) < locationBuffer)
        {
            currPhase = Phase.InFront;
        }
    }

    private void InFrontUpdate()
    {
        inFrontTime -= Time.deltaTime;

        RaycastHit hit;
        if (Physics.Raycast(fove.GetGazeRays().left, out hit, 1000f, 1 << 8))  
        {
            if (CharFrontLoc.position.z - hit.point.z > 3f)
            {
                nma.destination = new Vector3(CharFrontLoc.position.x, CharFrontLoc.position.y, hit.point.z);
            }
        }

        if (inFrontTime <= 0)
        {
            currPhase = Phase.ToCorner;
            nma.destination = CharCornerLoc.position;
        }
    }

    private void ToCornerUpdate()
    {
        //see if close enough
        if (Vector3.Distance(gameObject.transform.position, CharCornerLoc.position) < locationBuffer)
        {
            currPhase = Phase.End;
            nma.destination = CharEndLoc.position;
            lookingActive = false;
            timeOfLookingChange = Time.time;
            justChangedLooking = true;
        }
    }

    private void EndUpdate()
    {
        if (Vector3.Distance(gameObject.transform.position, CharCornerLoc.position) < locationBuffer)
        {
            gameObject.transform.parent.gameObject.GetComponent<SwitchCharacterScript>().NextAvatar();
            currPhase = Phase.None;
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

    private IEnumerator SitUp(float startTime)
    {
        yield return new WaitForSeconds(startTime);
        for (float f = 0f; f >= 1; f += .3f * Time.deltaTime)
        {
            animator.SetFloat(sitStyleHash, f);
            yield return null;
        }
    }

    void AnimGetUp()
    {
        animator.SetTrigger(getUpHash);
    }
}