using UnityEngine;
using UnityEngine.UI;

namespace NRO.UI.Views
{
    /// <summary>
    /// Quản lý hệ thống hạt linh khí huyền ảo bay lơ lửng trong cảnh giới Tiên Môn.
    /// </summary>
    public class GatewayQiParticles
    {
        private const int ParticleCount = 28;
        private RectTransform[] _qiParticles;
        private Vector2[] _qiVelocities;
        private float[] _qiAlphas;
        private Image[] _qiImages;

        public void Initialize(Transform parent)
        {
            var rootGo = new GameObject("QiParticlesRoot", typeof(RectTransform));
            rootGo.transform.SetParent(parent, false);
            var rootRt = rootGo.GetComponent<RectTransform>();
            rootRt.anchorMin = Vector2.zero;
            rootRt.anchorMax = Vector2.one;
            rootRt.sizeDelta = Vector2.zero;

            _qiParticles = new RectTransform[ParticleCount];
            _qiVelocities = new Vector2[ParticleCount];
            _qiAlphas = new float[ParticleCount];
            _qiImages = new Image[ParticleCount];

            Sprite circleSprite = ThemeTokens.CreateCircleSprite(16, Color.white);

            for (int i = 0; i < ParticleCount; i++)
            {
                var pGo = new GameObject($"Qi_{i}", typeof(RectTransform), typeof(Image));
                pGo.transform.SetParent(rootGo.transform, false);

                var rt = pGo.GetComponent<RectTransform>();
                float size = UnityEngine.Random.Range(3f, 7f);
                rt.sizeDelta = new Vector2(size, size);
                rt.anchoredPosition = new Vector2(
                    UnityEngine.Random.Range(-550f, 550f),
                    UnityEngine.Random.Range(-350f, 350f)
                );

                var img = pGo.GetComponent<Image>();
                img.sprite = circleSprite;
                img.raycastTarget = false;

                // Xen kẽ màu Ngọc Bích và Hoàng Kim
                Color c = (i % 2 == 0) ? ThemeTokens.CelestialCyan : ThemeTokens.AncientGoldBright;
                float alpha = UnityEngine.Random.Range(0.25f, 0.75f);
                c.a = alpha;
                img.color = c;

                _qiParticles[i] = rt;
                _qiVelocities[i] = new Vector2(
                    UnityEngine.Random.Range(-12f, 12f),
                    UnityEngine.Random.Range(20f, 65f)
                );
                _qiAlphas[i] = alpha;
                _qiImages[i] = img;
            }
        }

        public void UpdateParticles(float dt)
        {
            if (_qiParticles == null) return;

            for (int i = 0; i < _qiParticles.Length; i++)
            {
                var rt = _qiParticles[i];
                if (rt == null) continue;

                Vector2 pos = rt.anchoredPosition;
                pos += _qiVelocities[i] * dt;
                pos.x += Mathf.Sin(Time.time + i) * 0.4f;

                if (pos.y > 380f)
                {
                    pos.y = -380f;
                    pos.x = UnityEngine.Random.Range(-550f, 550f);
                }

                rt.anchoredPosition = pos;
            }
        }
    }
}
