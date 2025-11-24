using UnityEngine;

public class NieblaControl : MonoBehaviour
{
    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = new Color(0.25f, 0.25f, 0.25f);
        RenderSettings.fogStartDistance = 5f;
        RenderSettings.fogEndDistance = 10f;

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.3f, 0.3f, 0.3f);
    }
}
