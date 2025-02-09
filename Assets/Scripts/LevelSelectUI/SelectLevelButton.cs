using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectLevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AudioSource audioSource;  // Arrastra el AudioSource desde el Inspector
    [SerializeField] private AudioClip hoverSound;     // Arrastra el sonido desde el Inspector
    [SerializeField] private Color hoverColor;          // Color al pasar el ratón
    private Color originalColor;
    private Image buttonImage;

    private Outline outline;

    void Start()
    {
        buttonImage = GetComponent<Image>();  // Obtiene la imagen del botón
        originalColor = buttonImage.color;    // Guarda el color original

        outline = GetComponent<Outline>();
        if(outline != null)
            outline.enabled = false;            // Hides the button oultine at the beginning
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Reproduce el sonido
        if (audioSource != null && hoverSound != null)        
            audioSource.PlayOneShot(hoverSound);

        // Cambia el color del botón
        if (buttonImage != null)
            buttonImage.color = hoverColor;

        // Shows the button oultine when hover the mouse
        if (outline != null)
            outline.enabled = true;            
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Restaura el color original
        if (buttonImage != null)
            buttonImage.color = originalColor;

        // Hides the button oultine when exit the mouse
        if (outline != null)
            outline.enabled = false;
    }
}