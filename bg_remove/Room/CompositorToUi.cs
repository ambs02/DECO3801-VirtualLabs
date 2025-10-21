using UnityEngine;
using UnityEngine.UI;

public class CompositorToUI : MonoBehaviour
{
    [SerializeField] NNCam.Compositor compositor;
    [SerializeField] RawImage targetImage;

    void Start()
    {
        if (compositor != null && targetImage != null)
        {
            targetImage.texture = compositor.OutputTexture;
        }
    }
}
