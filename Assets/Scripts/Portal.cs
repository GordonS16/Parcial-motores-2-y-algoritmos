using UnityEngine;

public class Portal : MonoBehaviour
{
    private Animator myAnimator;

    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    public void Open()
    {
        myAnimator.SetTrigger("Open");
    }
}
