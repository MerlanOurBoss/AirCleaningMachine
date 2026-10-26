using TMPro;
using UnityEngine;

public class SborDisplayUI : MonoBehaviour
{
    [System.Serializable]
    private class Channel
    {
        public TextMeshProUGUI label;
        public string numberFormat = "0.";
        public string suffix = "";
        [HideInInspector] public float current;
        [HideInInspector] public float target;
    }

    [SerializeField] private Channel[] channels;
    [SerializeField] private float lerpSpeed = 4f;

    /// <summary>Плавно направить значение канала к target (аналог Mathf.Lerp(displayValueN, target, speed*dt) из оригинала).</summary>
    public void SetTarget(int channelIndex, float target, float speedOverride = -1f)
    {
        channels[channelIndex].target = target;
        if (speedOverride > 0f)
            _speedOverride[channelIndex] = speedOverride;
    }

    /// <summary>Мгновенно обнулить все каналы — аналог блока displayValue..5 = 0f в StopColumnProcess().</summary>
    public void ResetAll()
    {
        foreach (var c in channels)
        {
            c.current = 0f;
            c.target = 0f;
            if (c.label != null) c.label.text = c.current.ToString(c.numberFormat) + c.suffix;
        }
    }

    private float[] _speedOverride;

    private void Awake()
    {
        _speedOverride = new float[channels.Length];
        for (int i = 0; i < _speedOverride.Length; i++) _speedOverride[i] = -1f;
    }

    private void Update()
    {
        for (int i = 0; i < channels.Length; i++)
        {
            var c = channels[i];
            float speed = _speedOverride[i] > 0f ? _speedOverride[i] : lerpSpeed;
            c.current = Mathf.Lerp(c.current, c.target, speed * Time.deltaTime);
            if (c.label != null)
                c.label.text = c.current.ToString(c.numberFormat) + c.suffix;
        }
    }
}