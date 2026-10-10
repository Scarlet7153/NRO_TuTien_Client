#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Unity.Profiling;

namespace NRO.Util
{
    /// <summary>
    /// Component do dac hieu nang chuan (P0.3): FPS, 1% Low, FrameTime, GC Alloc va Draw Calls.
    /// Chi hoat dong trong Unity Editor hoac Development Build.
    /// Tu dong gan qua RuntimeInitializeOnLoadMethod, khong can thao tac thu cong tren Scene.
    /// Phim bat/tat OSD: BackQuote (~) hoac F12 (TUYET DOI KHONG DUNG F3 do xung dot MyKeyMap -23).
    /// </summary>
    public class PerfProbe : MonoBehaviour
    {
        private static PerfProbe _instance;
        public static PerfProbe Instance => _instance;

        private ProfilerRecorder _gcRecorder;
        private ProfilerRecorder _drawCallsRecorder;

        private bool _showOsd = true;
        private readonly KeyCode _toggleKey1 = KeyCode.BackQuote; // Phim `~`
        private readonly KeyCode _toggleKey2 = KeyCode.F12;

        // Thong so FPS & FrameTime
        private float _deltaTimeAccumulator;
        private int _frameCountAccumulator;
        private float _currentFps;
        private float _currentFrameTimeMs;
        private float _onePercentLowFps;

        private readonly float[] _recentFrameTimes = new float[120];
        private int _frameTimeIndex;

        // Bo dem GC Alloc & Draw Calls
        private long _lastGcAllocBytes;
        private long _lastDrawCalls;

        // Tich luy phuc vu bao cao Baseline
        private bool _isRecordingBaseline;
        private float _baselineRecordTimer;
        private readonly List<float> _baselineFpsSamples = new List<float>(3600);
        private readonly List<long> _baselineGcSamples = new List<long>(3600);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoInitialize()
        {
            if (_instance != null)
            {
                return;
            }

            var existing = FindObjectOfType<PerfProbe>();
            if (existing != null)
            {
                _instance = existing;
                return;
            }

            var go = new GameObject("[PerfProbe]");
            _instance = go.AddComponent<PerfProbe>();
            DontDestroyOnLoad(go);
        }

        private void OnEnable()
        {
            _gcRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "GC Allocated In Frame");
            _drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        }

        private void OnDisable()
        {
            _gcRecorder.Dispose();
            _drawCallsRecorder.Dispose();
        }

        private void Update()
        {
            // Toggle hien thi OSD
            if (Input.GetKeyDown(_toggleKey1) || Input.GetKeyDown(_toggleKey2))
            {
                _showOsd = !_showOsd;
            }

            float dt = Time.unscaledDeltaTime;
            _recentFrameTimes[_frameTimeIndex] = dt;
            _frameTimeIndex = (_frameTimeIndex + 1) % _recentFrameTimes.Length;

            _deltaTimeAccumulator += dt;
            _frameCountAccumulator++;

            // Cap nhat chi so moi 0.5s mot lan
            if (_deltaTimeAccumulator >= 0.5f)
            {
                _currentFps = _frameCountAccumulator / _deltaTimeAccumulator;
                _currentFrameTimeMs = (_deltaTimeAccumulator / _frameCountAccumulator) * 1000f;

                // Tinh 1% Low FPS
                float[] sorted = (float[])_recentFrameTimes.Clone();
                Array.Sort(sorted);
                int onePercentIndex = Mathf.Max(0, (int)(sorted.Length * 0.99f) - 1);
                float worstDt = sorted[onePercentIndex];
                _onePercentLowFps = worstDt > 0.0001f ? 1f / worstDt : 0f;

                _deltaTimeAccumulator = 0f;
                _frameCountAccumulator = 0;
            }

            if (_gcRecorder.Valid)
            {
                _lastGcAllocBytes = _gcRecorder.LastValue;
            }

            if (_drawCallsRecorder.Valid)
            {
                _lastDrawCalls = _drawCallsRecorder.LastValue;
            }

            if (_isRecordingBaseline)
            {
                _baselineRecordTimer += dt;
                _baselineFpsSamples.Add(dt > 0.0001f ? 1f / dt : 0f);
                _baselineGcSamples.Add(_lastGcAllocBytes);
            }
        }

        private void OnGUI()
        {
            if (!_showOsd)
            {
                return;
            }

            int w = 260;
            int h = 135;
            int x = Screen.width - w - 10;
            int y = 10;

            GUI.color = new Color(0f, 0f, 0f, 0.75f);
            GUI.Box(new Rect(x, y, w, h), GUIContent.none);
            GUI.color = Color.white;

            GUILayout.BeginArea(new Rect(x + 8, y + 6, w - 16, h - 12));

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.3f, 0.9f, 1f) }
            };

            GUIStyle textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                normal = { textColor = Color.white }
            };

            GUILayout.Label("NRO PerfProbe (P0.3 Baseline)", titleStyle);

            string fpsColor = _currentFps >= 50f ? "#55FF55" : (_currentFps >= 30f ? "#FFFF55" : "#FF5555");
            GUILayout.Label($"FPS: <color={fpsColor}>{_currentFps:F1}</color> (1% Low: {_onePercentLowFps:F1}) | {_currentFrameTimeMs:F1} ms", textStyle);

            double gcKb = _lastGcAllocBytes / 1024.0;
            string gcColor = gcKb < 50.0 ? "#55FF55" : "#FF8855";
            GUILayout.Label($"GC Alloc/Frame: <color={gcColor}>{gcKb:F1} KB</color>", textStyle);

            long totalMemMb = GC.GetTotalMemory(false) / (1024 * 1024);
            GUILayout.Label($"Draw Calls: {_lastDrawCalls} | Heap: {totalMemMb} MB", textStyle);

            if (_isRecordingBaseline)
            {
                GUILayout.Label($"<color=#FF5555>Đang ghi ({_baselineRecordTimer:F1}s)</color>", textStyle);
            }
            else
            {
                GUILayout.Label("<color=#AAAAAA>[~] Ẩn/Hiện | F12 Toggle</color>", textStyle);
            }

            GUILayout.EndArea();
        }

        /// <summary>
        /// Bat dau ghi mau baseline trong mot khoang thoi gian cu the (vi du 60 giay).
        /// </summary>
        public void StartBaselineRecording()
        {
            _baselineFpsSamples.Clear();
            _baselineGcSamples.Clear();
            _baselineRecordTimer = 0f;
            _isRecordingBaseline = true;
            Debug.Log("[PerfProbe] Bat dau ghi mau Baseline...");
        }

        /// <summary>
        /// Ket thuc ghi mau va xuat bao cao tom tat.
        /// </summary>
        public string StopBaselineRecording(string scenarioName)
        {
            _isRecordingBaseline = false;
            if (_baselineFpsSamples.Count == 0)
            {
                return "[PerfProbe] Khong co du lieu mau.";
            }

            float avgFps = 0f;
            for (int i = 0; i < _baselineFpsSamples.Count; i++)
            {
                avgFps += _baselineFpsSamples[i];
            }
            avgFps /= _baselineFpsSamples.Count;

            long totalGc = 0;
            for (int i = 0; i < _baselineGcSamples.Count; i++)
            {
                totalGc += _baselineGcSamples[i];
            }
            double avgGcKb = (totalGc / (double)_baselineGcSamples.Count) / 1024.0;

            var sb = new StringBuilder();
            sb.AppendLine($"=== PERF BASELINE REPORT: {scenarioName} ===");
            sb.AppendLine($"Thoi gian do: {_baselineRecordTimer:F1} giay | Tong so frame: {_baselineFpsSamples.Count}");
            sb.AppendLine($"FPS Trung Binh: {avgFps:F2}");
            sb.AppendLine($"GC Alloc Trung Binh / Frame: {avgGcKb:F2} KB");
            sb.AppendLine("============================================");

            string report = sb.ToString();
            Debug.Log(report);
            return report;
        }
    }
}
#else
namespace NRO.Util
{
    // Stub class rong khi bien dich Release Build de tranh ton overhead
    public class PerfProbe : UnityEngine.MonoBehaviour
    {
    }
}
#endif
