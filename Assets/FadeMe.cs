using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMe : MonoBehaviour {
    private Animator myAnimator;

    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    public void DoFadeOut()
    {
        myAnimator.SetTrigger(Animator.StringToHash("DoFadeOut"));
    }
    public void DoFadeIn()
    {
        myAnimator.SetTrigger(Animator.StringToHash("DoFadeIn"));
    }
}
