using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeoutController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetFade(bool fade)
    {
        animator.SetBool("Fadeout On", fade);
    }
}
