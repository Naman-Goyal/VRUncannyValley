using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionScript : MonoBehaviour {

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

    // Start - finds animator
    void Start () {
        animator = gameObject.GetComponent<Animator>();
        animator.SetLookAtPosition(playerLoc.position);

        StartCoroutine(ChangeSitting(true, 5f));    // Change sitting position after 5 seconds
        Invoke("AnimStartLooking", 4f);             // Start looking at player after 4 seconds

        Invoke("AnimGetUp", 8f);                    // Get up after 8 seconds
        StartCoroutine(MotionSequence(10f));        // Start the Main Motion Sequence after 10 seconds
    }

    // If lookingActive is true, will look at the player
    void OnAnimatorIK()
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

    private IEnumerator MotionSequence(float time)
    {
        float startTime;
        float changeInTime;

        // Wait for startTime seconds
        yield return new WaitForSeconds(time);

        // Slowly increase speed a bit
        startTime = Time.time;
        changeInTime = 0f;
        while (changeInTime < 1f)
        {
            changeInTime = Time.time - startTime;
            speed = Mathf.Lerp(0f, .4f, changeInTime);
            SendVals();
            yield return null;
        }
        speed = .4f;
        SendVals();

        // Move until a certain position
        while (gameObject.transform.position.z < 4f)
        {
            yield return null;
        }

        // Start turning left as well, for a bit
        startTime = Time.time;
        changeInTime = 0f;
        while (changeInTime < .4f)
        {
            changeInTime = Time.time - startTime;
            lr = Mathf.Lerp(0f, -1f, changeInTime / .4f);
            speed = Mathf.Lerp(.4f, 0f, changeInTime / .4f);
            SendVals();
            yield return null;
        }
        speed = 0f;
        lr = -1f;
        SendVals();

        //Move until a certain position
        while (gameObject.transform.position.x > 16f)
        {
            yield return null;
        }

        // Slow down, coming to a stop
        startTime = Time.time;
        changeInTime = 0f;
        while (changeInTime < .7f)
        {
            changeInTime = Time.time - startTime;
            lr = Mathf.Lerp(-1, 0f, changeInTime / .7f);
            SendVals();
            yield return null;
        }
        speed = 0f;
        lr = 0f;
        SendVals();

        // Turn Right till can walk out of the gap between couches
        startTime = Time.time;
        changeInTime = 0f;
        while (changeInTime < 1f)
        {
            changeInTime = Time.time - startTime;
            speed = Mathf.Lerp(0, .2f, changeInTime / 1f);
            lr = Mathf.Lerp(0, .7f, changeInTime / 1f);
            SendVals();
            yield return null;
        }
        speed = .2f;
        lr = .7f;
        SendVals();

        // Let the model just go right for a bit
        yield return new WaitForSeconds(1f);

        // Walk out of that gap, until a certain location
        startTime = Time.time;
        changeInTime = 0f;
        while (changeInTime < .3f)
        {
            changeInTime = Time.time - startTime;
            speed = Mathf.Lerp(.2f, .7f, changeInTime / .3f);
            lr = Mathf.Lerp(.7f, 0, changeInTime / .3f);
            SendVals();
            yield return null;
        }
        speed = .7f;
        lr = 0f;
        SendVals();

        while (gameObject.transform.position.z < 27)
        {
            yield return null;
        }

        // Move Left till behind the couch
        startTime = Time.time;
        changeInTime = 0f;
        while (changeInTime < 1f)
        {
            changeInTime = Time.time - startTime;
            speed = Mathf.Lerp(.4f, 0f, changeInTime / 1f);
            lr = Mathf.Lerp(0, -1f, changeInTime / 1f);
            SendVals();
            yield return null;
        }
        speed = 0f;
        lr = -1f;
        SendVals();

        while(gameObject.transform.position.x > -5)
        {
            yield return null;
        }

        // Now, just turn
        startTime = Time.time;
        changeInTime = 0f;
        while (changeInTime < .5f)
        {
            changeInTime = Time.time - startTime;
            turnMove = Mathf.Lerp(1f, 0f, changeInTime / .5f);
            SendVals();
            yield return null;
        }
        turnMove = 0f;
        SendVals();

        yield return new WaitForSeconds(.3f);

        //Move till right behind player
        startTime = Time.time;
        changeInTime = 0f;
        while (changeInTime < 1f)
        {
            changeInTime = Time.time - startTime;
            speed = Mathf.Lerp(0f, .7f, changeInTime / 1f);
            lr = Mathf.Lerp(-1f, 0f, changeInTime / 1f);
            SendVals();
            yield return null;
        }
        speed = .7f;
        lr = 0f;
        SendVals();

        while (gameObject.transform.position.z > 12)
        {
            yield return null;
        }

        startTime = Time.time;
        changeInTime = 0f;
        while (changeInTime < 1f)
        {
            changeInTime = Time.time - startTime;
            speed = Mathf.Lerp(0.7f, 0f, changeInTime / 1f);
            SendVals();
            yield return null;
        }
        speed = 0f;
        SendVals();

        // Walk a bit more
        turnMove = 1;

        startTime = Time.time;
        changeInTime = 0f;
        while (changeInTime < 1f)
        {
            changeInTime = Time.time - startTime;
            speed = Mathf.Lerp(0f, .7f, changeInTime / 1f);
            SendVals();
            yield return null;
        }
        speed = .7f;
        SendVals();
        
        // Angle left once at a certain point
        while (gameObject.transform.position.z > -5)
        {
            yield return null;
        }

        startTime = Time.time;
        changeInTime = 0f;
        while (changeInTime < 1f)
        {
            changeInTime = Time.time - startTime;
            speed = Mathf.Lerp(0.7f, 0f, changeInTime / 1f);
            lr = Mathf.Lerp(-.6f, -1f, changeInTime / 1f);
            SendVals();
            yield return null;
        }
        speed = 0f;
        lr = -1f;
        SendVals();

        while (gameObject.transform.position.x < 12)
        {
            yield return null;
        }

        // Now just walk straight until behind the glass, where we'll end
        startTime = Time.time;
        changeInTime = 0f;
        while (changeInTime < 1f)
        {
            changeInTime = Time.time - startTime;
            speed = Mathf.Lerp(0f, 1f, changeInTime / 1f);
            lr = Mathf.Lerp(-1f, 0f, changeInTime / 1f);
            SendVals();
            yield return null;
        }
        speed = 1f;
        lr = 0f;
        SendVals();

        while (gameObject.transform.position.x < 38f)
        {
            yield return null;
        }

        startTime = Time.time;
        changeInTime = 0f;
        while (changeInTime < 1f)
        {
            changeInTime = Time.time - startTime;
            speed = Mathf.Lerp(1f, 0f, changeInTime / 1f);
            SendVals();
            yield return null;
        }

        //END
        MotionSequenceEndCallback();
    }

    void MotionSequenceEndCallback()
    {
        speed = 0f;
        lr = 0f;
        turnMove = 1f;
        SendVals();
    }

    private IEnumerator ChangeSitting(bool sitStraight, float startTime)
    {
        yield return new WaitForSeconds(startTime);
        for (float f = sitStraight ? 0f : 1f; sitStraight ? f >= 1 : f <= 0;)
        {
            animator.SetFloat(sitStyleHash, f);
            f += sitStraight ? Time.deltaTime : -Time.deltaTime;
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

    void SendVals()
    {
        animator.SetFloat(speedHash, speed);
        animator.SetFloat(lrHash, lr);
        animator.SetFloat(turnMoveHash, turnMove);
    }
}
