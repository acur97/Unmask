using UnityEngine;

public class PintableObject : MonoBehaviour
{
    public Texture2D runtimeAlbedo;
    [SerializeField] private Renderer renderer;
    [SerializeField] private Material material;

    private readonly Color32 white = new(255, 255, 255, 255);
    private readonly int _MaskTex = Shader.PropertyToID("_MaskTex");

    private void Awake()
    {
        runtimeAlbedo = new Texture2D(512, 512, TextureFormat.R8, true, false, true);

        byte[] albedoPixels = new byte[runtimeAlbedo.width * runtimeAlbedo.height];

        for (int i = 0; i < albedoPixels.Length; i++)
        {
            albedoPixels[i] = 255;
        }

        runtimeAlbedo.SetPixelData(albedoPixels, 0);
        runtimeAlbedo.Apply(false);

        material.SetTexture(_MaskTex, runtimeAlbedo);
    }
}