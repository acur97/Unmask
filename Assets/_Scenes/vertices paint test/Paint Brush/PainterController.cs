using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class PaintBrushConfig
{
    public float radius = 32f;
    public LayerMask mask = -5;
}

public class PainterController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private PintableObject pintableObject;

    [Space]
    [SerializeField] private PaintBrushConfig brushConfig;

    private bool supportsAsyncGPUReadback;

    private Ray ray;
    private readonly RaycastHit[] result = new RaycastHit[1];

    private Vector2 uv;
    private int width;
    private int height;
    private int centerX;
    private int centerY;
    private int radius;
    private int px;
    private int py;
    private float distance;
    private Color color = Color.black;

    [Space]
    public float progress;

    private void Awake()
    {
        supportsAsyncGPUReadback = SystemInfo.supportsAsyncGPUReadback;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.RaycastNonAlloc(ray, result, float.PositiveInfinity, brushConfig.mask) == 0)
                return;

            uv = result[0].textureCoord;

            //pasar info a configuracion global
            width = pintableObject.runtimeAlbedo.width;
            height = pintableObject.runtimeAlbedo.height;

            centerX = Mathf.RoundToInt(uv.x * width);
            centerY = Mathf.RoundToInt(uv.y * height);

            radius = Mathf.RoundToInt(brushConfig.radius);

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    px = centerX + x;
                    py = centerY + y;

                    if (px < 0 || py < 0 || px >= width || py >= height)
                        continue;

                    distance = Mathf.Sqrt(x * x + y * y);

                    if (distance > radius)
                        continue;

                    //color.r = distance.Remap(0, radius, 0, 1);

                    pintableObject.runtimeAlbedo.SetPixel(px, py, color);
                }
            }

            pintableObject.runtimeAlbedo.Apply(false);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ReadGeneralColor();
        }
    }

    [ContextMenu("Read General Color")]
    public void ReadGeneralColor()
    {
        pintableObject.runtimeAlbedo.Apply(true);

        if (supportsAsyncGPUReadback)
        {
            AsyncGPUReadback.Request(pintableObject.runtimeAlbedo, pintableObject.runtimeAlbedo.mipmapCount - 1, OnCompleteReadback);
        }
        else
        {
            progress = 1f - pintableObject.runtimeAlbedo.GetPixel(0, 0, pintableObject.runtimeAlbedo.mipmapCount - 1).r;
            Debug.Log($"Progreso: {progress:P2}");
        }

    }

    private void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
            return;

        progress = 1f - (request.GetData<byte>()[0] / 255f);
        Debug.Log($"Progreso: {progress:P2}");
    }
}