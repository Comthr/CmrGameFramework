using UnityEngine;
using UnityEngine.UI;

namespace CmrGame.UI
{
    [RequireComponent(typeof(RawImage))]
    public class ProceduralBackground : MonoBehaviour
    {
        public int width = 512;
        public int height = 512;
        public Color baseColor = new Color(0.05f, 0.05f, 0.08f, 1f); // 深空灰蓝
        public Color highlightColor = new Color(0.1f, 0.1f, 0.2f, 0.5f); // 微微的亮光
        public float noiseScale = 5f;
        public float vignetteStrength = 0.8f;

        private Texture2D m_Texture;
        private RawImage m_RawImage;

        void Start()
        {
            m_RawImage = GetComponent<RawImage>();
            GenerateTexture();
        }

        [ContextMenu("Regenerate")]
        public void GenerateTexture()
        {
            m_Texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 1. UV 坐标 (0~1)
                    float u = (float)x / width;
                    float v = (float)y / height;

                    // 2. 柏林噪声 (模拟烟雾/流动感)
                    // 我们加上偏移让它稍微不对称
                    float noise = Mathf.PerlinNoise(u * noiseScale, v * noiseScale);

                    // 3. 混合颜色
                    Color finalColor = Color.Lerp(baseColor, highlightColor, noise * 0.5f);

                    // 4. 暗角 (Vignette) - 让四周变暗，突出中间
                    float distFromCenter = Vector2.Distance(new Vector2(u, v), new Vector2(0.5f, 0.5f));
                    float vignette = Mathf.Clamp01(1.0f - distFromCenter * vignetteStrength);

                    // 更加平滑的暗角
                    vignette = Mathf.SmoothStep(0f, 1f, vignette);

                    finalColor *= vignette;
                    finalColor.a = 1f;

                    pixels[y * width + x] = finalColor;
                }
            }

            m_Texture.SetPixels(pixels);
            m_Texture.Apply();
            m_RawImage.texture = m_Texture;
        }
    }
}