using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AudioSource audioSource;  // Arrastra el AudioSource desde el Inspector
    [SerializeField] private AudioClip hoverSound;     // Arrastra el sonido desde el Inspector

    private Image buttonImage;                          // Button Image Component
    private Color imageColor;                           // Button Image Color               

    void Start()
    {
        buttonImage = GetComponent<Image>();        // Gets the button image component
        imageColor = buttonImage.color;     // Gets the original button color        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Play the sound Fx
        if (audioSource != null && hoverSound != null)        
            audioSource.PlayOneShot(hoverSound);

        // Set the button image color as visible
        if (buttonImage != null)        
            buttonImage.color = new Color(buttonImage.color.r,buttonImage.color.g, buttonImage.color.b, 1f);        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Set the button image color as transparent
        if (buttonImage != null)
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0f);       
    }
}