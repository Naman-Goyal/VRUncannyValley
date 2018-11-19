using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkScript : MonoBehaviour {

    Animator animator;

    private readonly int blinkHash = Animator.StringToHash("Blink");

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        Invoke("Blink", RandomTime());
    }

    void Blink()
    {
        animator.SetTrigger(blinkHash);
        Invoke("Blink", RandomTime());
    }

    float RandomTime()
    {
        return Random.Range(3f, 6f);
    }
}
