using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] public string openDrawerText = "[E] to open drawer";
    [SerializeField] public string closeDrawerText = "[E] to close drawer";
    public Animator animator;
    private bool isOpened;

    public string InteractionText
    {
        get
        {
            return isOpened ? closeDrawerText : openDrawerText;
        }
    }

    public void Interact()
    {
        isOpened = !isOpened;
        animator.SetBool("isOpened", isOpened);
    }

    
}
