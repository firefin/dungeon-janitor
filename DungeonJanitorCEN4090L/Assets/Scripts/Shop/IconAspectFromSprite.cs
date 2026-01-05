using UnityEngine;
using UnityEngine.UI;

public class IconAspectFromSprite : MonoBehaviour
{
    [SerializeField] Image img;                 
    [SerializeField] AspectRatioFitter arf;     

    void Reset()
    {
        if (!img) img = GetComponent<Image>();
        if (!arf) arf = GetComponent<AspectRatioFitter>();
    }

    void OnEnable() => Apply();
#if UNITY_EDITOR
    void OnValidate() => Apply();
#endif

    void Apply()
    {
        if (!img || !arf || !img.sprite) return;
        var r = img.sprite.rect;
        if (r.height > 0f)
        {
            arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            arf.aspectRatio = r.width / r.height;
        }
    }
}
