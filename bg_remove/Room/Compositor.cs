using UnityEngine;

namespace NNCam
{
    public sealed class Compositor : MonoBehaviour
    {
        [SerializeField] WebcamInput _input = null;
        [SerializeField, Range(0.01f, 0.99f)] float _threshold = .55f;
        [SerializeField, Range(0f, 0.5f)]     float _feather   = 0.10f;
        [SerializeField, Range(0f, 0.98f)]    float _hold      = 0.85f;
        [SerializeField, Range(0, 3)]         int   _dilatePx  = 1;

        [SerializeField] ResourceSet _resources = null;
        [SerializeField] Shader _shader = null;

        public RenderTexture OutputTexture => _rt;

        SegmentationFilter _filter;
        Material _material;
        RenderTexture _rt, _prev;

        void Start()
        {
            _filter   = new SegmentationFilter(_resources);
            _material = new Material(_shader);

            GetComponent<Camera>().clearFlags      = CameraClearFlags.SolidColor;
            GetComponent<Camera>().backgroundColor = new Color(0,0,0,0);
        }

        void OnDestroy()
        {
            if (_filter   != null) _filter.Dispose();
            if (_material != null) Destroy(_material);
            ReleaseRT(ref _rt);
            ReleaseRT(ref _prev);
        }

        void Update()
        {
            if (_input == null || _input.Texture == null) return;
            _filter.ProcessImage(_input.Texture);

            // Ensure our RTs match input resolution (fixes blocky look)
            var src = _input.Texture as RenderTexture;
            if (src != null)
            {
                EnsureRT(ref _rt,   src.width, src.height);
                EnsureRT(ref _prev, src.width, src.height);
            }
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            if (_input == null || _input.Texture == null || _filter == null || _filter.MaskTexture == null || _rt == null || _prev == null)
            {
                Graphics.Blit(src, dst);
                return;
            }

            _material.SetTexture("_CameraFeed", _input.Texture);
            _material.SetTexture("_Mask",       _filter.MaskTexture);
            _material.SetTexture("_PrevTex",    _prev);
            _material.SetFloat  ("_Threshold",  _threshold);
            _material.SetFloat  ("_Feather",    _feather);
            _material.SetFloat  ("_Hold",       _hold);
            _material.SetFloat  ("_DilatePx",   _dilatePx);

            // Clear target to transparent every frame
            var prevActive = RenderTexture.active;
            RenderTexture.active = _rt;
            GL.Clear(true, true, new Color(0,0,0,0));
            RenderTexture.active = prevActive;

            // Compose into _rt (shader writes exact RGBA)
            Graphics.Blit(null, _rt, _material, 0);

            // Copy current output to _prev for the next frame (for temporal hold)
            Graphics.Blit(_rt, _prev);

            // Preview to screen
            Graphics.Blit(_rt, dst);
        }

        void EnsureRT(ref RenderTexture rt, int w, int h)
        {
            if (rt != null && rt.width == w && rt.height == h) return;
            ReleaseRT(ref rt);
            rt = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32) { useMipMap = false };
            rt.Create();
        }

        void ReleaseRT(ref RenderTexture rt)
        {
            if (rt == null) return;
            rt.Release();
            rt = null;
        }
    }
}
