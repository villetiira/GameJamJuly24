using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactableName;
    public float interactTime = 2f;
    public string interactTooltip;
    public AudioSource audioSource;
    public AudioClip interactClip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void Interact(GameObject player)
    {
        Debug.Log("Interacting with a " + interactableName);
    }

    public AudioClip GetInteractClip()
    {
        return interactClip;
        
    }
}
