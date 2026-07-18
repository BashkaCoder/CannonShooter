using UnityEngine;

namespace Feedback
{
    [RequireComponent(typeof(Renderer))]
    public class WallDecalPainter : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Shader _stampShader;
        [SerializeField] private int _textureSize;

        private RenderTexture _decalTexture;
        private Material _wallMaterialInstance;
        private Material _stampMaterial;
        private Vector3 _decalAxis;

        private static readonly int DecalTexId = Shader.PropertyToID("_DecalTex");
        private static readonly int DecalAxisId = Shader.PropertyToID("_DecalAxis");
        private static readonly int PrevTexId = Shader.PropertyToID("_PrevTex");
        private static readonly int StampTexId = Shader.PropertyToID("_StampTex");
        private static readonly int StampUvId = Shader.PropertyToID("_StampUV");
        private static readonly int StampUvSizeId = Shader.PropertyToID("_StampUvSize");

        private void Awake()
        {
            _wallMaterialInstance = new Material(_renderer.sharedMaterial);
            _renderer.sharedMaterial = _wallMaterialInstance;
            _decalAxis = CalculateBroadFaceAxis();

            _decalTexture = new RenderTexture(_textureSize, _textureSize, 0, RenderTextureFormat.ARGB32)
            {
                name = name + "_Decals",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            _decalTexture.Create();

            _stampMaterial = new Material(_stampShader);
            _wallMaterialInstance.SetTexture(DecalTexId, _decalTexture);
            _wallMaterialInstance.SetVector(DecalAxisId, _decalAxis);
            Clear();
        }

        public void Paint(RaycastHit hit, Texture2D stamp, float stampSize)
        {
            if (!IsBroadFaceHit(hit))
            {
                return;
            }

            var temporary = RenderTexture.GetTemporary(_decalTexture.width, _decalTexture.height,
                0, _decalTexture.format);

            Graphics.Blit(_decalTexture, temporary);

            _stampMaterial.SetTexture(PrevTexId, temporary);
            _stampMaterial.SetTexture(StampTexId, stamp);
            _stampMaterial.SetVector(StampUvId, CalculateBroadFaceUv(hit.point));
            _stampMaterial.SetVector(StampUvSizeId, CalculateBroadFaceUvSize(stampSize));

            Graphics.Blit(temporary, _decalTexture, _stampMaterial);
            RenderTexture.ReleaseTemporary(temporary);
        }

        private Vector3 CalculateBroadFaceAxis()
        {
            var scale = transform.lossyScale;

            if (scale.y <= scale.x && scale.y <= scale.z)
            {
                return Vector3.up;
            }

            return scale.x <= scale.z ? Vector3.right : Vector3.forward;
        }

        private bool IsBroadFaceHit(RaycastHit hit)
        {
            var localNormal = transform.InverseTransformDirection(hit.normal).normalized;
            return Mathf.Abs(Vector3.Dot(localNormal, _decalAxis)) >= 0.75f;
        }

        private Vector2 CalculateBroadFaceUv(Vector3 worldPoint)
        {
            var localPoint = transform.InverseTransformPoint(worldPoint);

            Vector2 uv;
            if (_decalAxis == Vector3.up)
            {
                uv = new Vector2(localPoint.x + 0.5f, localPoint.z + 0.5f);
            }
            else if (_decalAxis == Vector3.right)
            {
                uv = new Vector2(localPoint.z + 0.5f, localPoint.y + 0.5f);
            }
            else
            {
                uv = new Vector2(localPoint.x + 0.5f, localPoint.y + 0.5f);
            }

            return new Vector2(Mathf.Clamp01(uv.x), Mathf.Clamp01(uv.y));
        }

        private Vector2 CalculateBroadFaceUvSize(float worldSize)
        {
            var scale = transform.lossyScale;

            if (_decalAxis == Vector3.up)
            {
                return new Vector2(worldSize / scale.x, worldSize / scale.z);
            }

            return _decalAxis == Vector3.right ? 
                new Vector2(worldSize / scale.z, worldSize / scale.y) :
                new Vector2(worldSize / scale.x, worldSize / scale.y);
        }

        private void Clear()
        {
            var active = RenderTexture.active;
            RenderTexture.active = _decalTexture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = active;
        }

        private void OnDestroy()
        {
            Destroy(_wallMaterialInstance);
            Destroy(_stampMaterial);
            _decalTexture.Release();
            Destroy(_decalTexture);
        }
    }
}