Shader "Hidden/NNCam/CompositorTransparent"
{
    Properties
    {
        _CameraFeed ("Camera Feed", 2D) = "white" {}
        _Mask       ("Mask",        2D) = "white" {}
        _PrevTex    ("Prev Tex",    2D) = "black" {}
        _Threshold  ("Threshold", Range(0,1)) = 0.5
        _Feather    ("Feather",   Range(0,0.5)) = 0.08
        _Hold       ("Temporal Hold", Range(0,0.98)) = 0.85   // 0=no hold, 0.85 is nice
        _DilatePx   ("Dilate (px)", Range(0,3)) = 1.0         // grow mask by N texels
    }

    SubShader
    {
        // We write exact RGBA into an off-screen RT
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _CameraFeed;
            sampler2D _Mask;
            sampler2D _PrevTex;
            float _Threshold, _Feather, _Hold, _DilatePx;
            float4 _Mask_TexelSize; // x=1/width, y=1/height

            struct appdata {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            float dilatedMask(float2 uv)
            {
                float2 o = float2(_Mask_TexelSize.x * _DilatePx, _Mask_TexelSize.y * _DilatePx);
                float m  = tex2D(_Mask, uv).r;
                // 8-neighbour max for a small dilate
                m = max(m, tex2D(_Mask, uv + float2( o.x,  0   )).r);
                m = max(m, tex2D(_Mask, uv + float2(-o.x,  0   )).r);
                m = max(m, tex2D(_Mask, uv + float2( 0,    o.y )).r);
                m = max(m, tex2D(_Mask, uv + float2( 0,   -o.y )).r);
                m = max(m, tex2D(_Mask, uv + float2( o.x,  o.y )).r);
                m = max(m, tex2D(_Mask, uv + float2(-o.x,  o.y )).r);
                m = max(m, tex2D(_Mask, uv + float2( o.x, -o.y )).r);
                m = max(m, tex2D(_Mask, uv + float2(-o.x, -o.y )).r);
                return m;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float m     = dilatedMask(i.uv);
                // soft edge
                float aCurr = saturate(smoothstep(_Threshold - _Feather, _Threshold + _Feather, m));

                // temporal hold: keep some of last frame’s alpha
                float aPrev = tex2D(_PrevTex, i.uv).a;
                float a     = max(aCurr, aPrev * _Hold);

                fixed4 cam  = tex2D(_CameraFeed, i.uv);
                cam.rgb *= a;  // premultiply
                cam.a    = a;
                return cam;
            }
            ENDCG
        }
    }
}
