using UnityEngine;

namespace NNCam
{
    public sealed class WebcamInput : MonoBehaviour
    {
        [SerializeField] string _deviceName = "";

        WebCamTexture _webcam;
        RenderTexture _buffer;

        public Texture Texture => _buffer;

        void Start()
        {
            _webcam = new WebCamTexture(_deviceName);
            _webcam.Play();
        }

        void OnDestroy()
        {
            if (_webcam != null) _webcam.Stop();
            if (_buffer  != null) Destroy(_buffer);
        }

        void Update()
        {
            if (_webcam == null || !_webcam.didUpdateThisFrame) return;

            // Wait until webcam actually reports a sane size (not 0 or 16)
            int w = _webcam.width;
            int h = _webcam.height;
            if (w >= 160 && h >= 120) // threshold avoids the 16x16 startup dummy
            {
                if (_buffer == null || _buffer.width != w || _buffer.height != h)
                {
                    if (_buffer != null) Destroy(_buffer);
                    _buffer = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
                    _buffer.Create();
                    Debug.Log($"WebcamInput: allocated {_buffer.width}x{_buffer.height} ARGB32");
                }

                var vflip  = _webcam.videoVerticallyMirrored;
                var scale  = new Vector2(1, vflip ? -1 : 1);
                var offset = new Vector2(0, vflip ?  1 : 0);

                Graphics.Blit(_webcam, _buffer, scale, offset);
            }
        }
    }
}
