using UnityEngine;

public class GlassHighlight : MonoBehaviour
{
    private Material originalMaterial;
    public Material highlightMaterial;
    private Renderer glassRenderer;

    //public GlassController glassController;

    void Start()
    {
        glassRenderer = GetComponentInChildren<Renderer>();
        originalMaterial = glassRenderer.material;
    }

    void OnMouseEnter()
    {
        if (!GetComponent<GlassController>().isDragging)
        {
            glassRenderer.material = highlightMaterial;
        }
    }

    void OnMouseExit()
    {
        glassRenderer.material = originalMaterial;
    }
}