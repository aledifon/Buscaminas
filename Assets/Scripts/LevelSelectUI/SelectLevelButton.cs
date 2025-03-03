using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectLevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AudioSource audioSource;  // Arrastra el AudioSource desde el Inspector
    [SerializeField] private AudioClip hoverSound;     // Arrastra el sonido desde el Inspector
    [SerializeField] private Color hoverColor;          // Color al pasar el rat�n
    private Color originalColor;
    private Image buttonImage;

    private Outline outline;

    #region UnityAPI
    void Start()
    {
        buttonImage = GetComponent<Image>();  // Obtiene la imagen del bot�n
        originalColor = buttonImage.color;    // Guarda el color original

        outline = GetComponent<Outline>();
        if(outline != null)
            outline.enabled = false;            // Hides the button oultine at the beginning
    }
    #endregion

    #region PublicMethods
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Reproduce el sonido
        if (audioSource != null && hoverSound != null)        
            audioSource.PlayOneShot(hoverSound);

        HighlightButton();
    }

    public void OnPointerExit(PointerEventData eventData)
    {                
        FadeButton();
    }
    public void HighlightButton()
    {
        SetHoverColorButton();
        ShowsOutlinedButton();
    }
    public void FadeButton()
    {
        SetOriginalColorButton();
        HidesOutlinedButton();
    }
    #endregion

    #region PrivateMethods
    private void SetHoverColorButton()
    {
        // Cambia el color del bot�n
        if (buttonImage != null)
            buttonImage.color = hoverColor;
    }
    private void SetOriginalColorButton()
    {
        // Restaura el color original
        if (buttonImage != null)
            buttonImage.color = originalColor;
    }
    private void ShowsOutlinedButton()
    {
        // Shows the button oultine when hover the mouse
        if (outline != null)
            outline.enabled = true;
    }
    private void HidesOutlinedButton()
    {
        // Hides the button oultine when exit the mouse
        if (outline != null)
            outline.enabled = false;
    }
    #endregion
}