using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMove : MonoBehaviour
{

    Animator animator;

    private readonly int sitUpHash = Animator.StringToHash("SitUp");// trigger
    private readonly int getUpHash = Animator.StringToHash("GetUp");// trigger
    private readonly int vXHash = Animator.StringToHash("v_x");     // x dir of move
    private readonly int vYHash = Animator.StringToHash("v_y");     // y dir of move
    private readonly int moveHash = Animator.StringToHash("Move");  // boolean

    bool lookingActive = false;
    float timeOfLookingChange = 0f;
    bool justChangedLooking = false;

    public Transform playerLoc;

    // For start
    float timeSinceLast = 0f;
    float timeLookingAtModel = 0f;

    // What phase we're in
    public enum Phase { Start, WalkingTo, InFront, End, None };
    [System.NonSerialized]
    public Phase currPhase = Phase.Start;

    // For in front
    public float inFrontTime = 20f;
    bool walkingBack = false;

    // Where the player is looking
    enum GazePosition { Left, Centre, Right, None };

    public FoveInterface2 fove;
    public Transform CharFrontLoc;
    public Transform CharEndLoc;

    private NavMeshAgent nma;

    // Start - finds animator
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        nma = gameObject.transform.parent.GetComponent<NavMeshAgent>();
        nma.updatePosition = false;
        nma.destination = transform.position;
    }

    private void Update()
    {
        Vector3 worldDeltaPosition = nma.nextPosition - transform.parent.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 velocity = new Vector2(dx, dy) / Time.deltaTime;
        if (velocity.SqrMagnitude() > 1f)
        {
            velocity.Normalize();
        }

        bool shouldMove = nma.remainingDistance > nma.radius;

        animator.SetFloat(vXHash, velocity.x);
        animator.SetFloat(vYHash, velocity.y);
        animator.SetBool(moveHash, shouldMove);
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

        if (timeLookingAtModel >= 1f)
        {
            StartCoroutine(ChangeLooking(0f, true));
            StartCoroutine(SitUp(2f));

            timeLookingAtModel = float.MinValue;            //make sure it doesn't happen again
        }

        timeSinceLast = 0f;
    }

    private void WalkingToUpdate()
    {
        if (nma.remainingDistance < nma.radius)
            currPhase = Phase.InFront;
    }

    private void InFrontUpdate()
    {
        nma.destination = transform.position;
        inFrontTime -= Time.deltaTime;

        RaycastHit hit;
        if (Physics.Raycast(fove.GetGazeRays().left, out hit, 1000f, 1 << 10))  
        {
            if (Mathf.Abs(transform.position.z - hit.point.z) > .5f)
            {
                nma.destination = new Vector3(transform.position.x, transform.position.y, hit.point.z);
            }
        }

        if (inFrontTime <= 0)
        {
            currPhase = Phase.End;
            nma.destination = CharEndLoc.position;
            StartCoroutine(ChangeLooking(2f, false));
        }
    }

    private void EndUpdate()
    {
        if (nma.remainingDistance < nma.radius)
        {
            gameObject.transform.parent.gameObject.GetComponent<SwitchCharacterScript>().NextAvatar();
            currPhase = Phase.None;
            gameObject.SetActive(false);
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
        animator.SetLookAtPosition(playerLoc.position);
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

    void OnAnimatorMove()
    {
        transform.parent.position = nma.nextPosition;
    }

    private IEnumerator SitUp(float startTime)
    {
        yield return new WaitForSeconds(startTime);
        animator.SetTrigger(sitUpHash);
        yield return new WaitForSeconds(2f);
        animator.SetTrigger(getUpHash);
        yield return new WaitForSeconds(2.25f);
        nma.destination = CharFrontLoc.position;
        currPhase = Phase.WalkingTo;
    }

    private IEnumerator ChangeLooking(float startTime, bool looking)
    {
        yield return new WaitForSeconds(startTime);
        lookingActive = looking;
        timeOfLookingChange = Time.time;
        justChangedLooking = true;
    }
}