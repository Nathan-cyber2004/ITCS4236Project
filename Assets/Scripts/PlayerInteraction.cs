using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float range = 5f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private TMP_Text interactionText;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

  void Update()
  {
    HandleRayCast();
  }

  private void HandleRayCast()
    {
        RaycastHit hit;
        if (!Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, range, layerMask))
        {
            return;
        }
        if (!hit.collider.TryGetComponent(out IInteractable interactable))
        {
            interactionText.text = "";
            return;
        }
        interactionText.text = interactable.InteractionText;
        if (Input.GetKeyDown(KeyCode.E))
        {
            interactable.Interact();
        }
    }
}
