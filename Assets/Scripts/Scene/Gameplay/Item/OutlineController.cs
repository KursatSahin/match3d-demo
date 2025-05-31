using UnityEngine;

public class OutlineController : MonoBehaviour
{
    [SerializeField] private Color highlightColor = new Color(1f, 0.92f, 0.016f, 1f); // Sarı
    [SerializeField] private float highlightIntensity = 1.5f; // Parlaklık miktarı
    
    private Renderer[] renderers;
    private MaterialPropertyBlock propertyBlock;
    private bool isHighlightActive = false;
    
    private void Awake()
    {
        // Tüm renderer'ları al
        renderers = GetComponentsInChildren<Renderer>();
        
        // Material property block oluştur
        propertyBlock = new MaterialPropertyBlock();
    }
    
    public void EnableOutline()
    {
        if (isHighlightActive) return;
        
        foreach (var renderer in renderers)
        {
            // Her renderer için property block al
            renderer.GetPropertyBlock(propertyBlock);
            
            // Emisyon değerlerini ayarla
            propertyBlock.SetColor("_EmissionColor", highlightColor * highlightIntensity);
            
            // Emisyon keyword'ünü etkinleştir
            // Not: Property block ile shader keyword set edemiyoruz, bu yüzden _EmissionColor'u ayarlamak yeterli
            
            // Property block'u renderer'a uygula
            renderer.SetPropertyBlock(propertyBlock);
        }
        
        isHighlightActive = true;
    }
    
    public void DisableOutline()
    {
        if (!isHighlightActive) return;
        
        foreach (var renderer in renderers)
        {
            // Her renderer için property block al
            renderer.GetPropertyBlock(propertyBlock);
            
            // Emisyon değerlerini sıfırla
            propertyBlock.SetColor("_EmissionColor", Color.black);
            
            // Property block'u renderer'a uygula
            renderer.SetPropertyBlock(propertyBlock);
        }
        
        isHighlightActive = false;
    }
    
    public void SetOutlineColor(Color color)
    {
        highlightColor = color;
        
        if (isHighlightActive)
        {
            foreach (var renderer in renderers)
            {
                // Her renderer için property block al
                renderer.GetPropertyBlock(propertyBlock);
                
                // Emisyon rengini güncelle
                propertyBlock.SetColor("_EmissionColor", highlightColor * highlightIntensity);
                
                // Property block'u renderer'a uygula
                renderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
    
    public void SetHighlightIntensity(float intensity)
    {
        highlightIntensity = intensity;
        
        if (isHighlightActive)
        {
            foreach (var renderer in renderers)
            {
                // Her renderer için property block al
                renderer.GetPropertyBlock(propertyBlock);
                
                // Emisyon yoğunluğunu güncelle
                propertyBlock.SetColor("_EmissionColor", highlightColor * highlightIntensity);
                
                // Property block'u renderer'a uygula
                renderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
    
    public void ToggleOutline()
    {
        if (isHighlightActive)
            DisableOutline();
        else
            EnableOutline();
    }
}