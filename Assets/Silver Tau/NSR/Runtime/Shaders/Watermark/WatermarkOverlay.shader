Shader "Silver Tau/NSR - Screen Recorder/Watermark/WatermarkOverlay" {
    Properties {
        _MainTex ("Base Texture", 2D) = "white" {}
        _WatermarkTex ("Watermark Texture", 2D) = "white" {}
        _WatermarkOpacity ("Watermark Opacity", Range(0,1)) = 0.5
        _WatermarkPos ("Watermark Position", Vector) = (0.8, 0.1, 0, 0)
        _WatermarkSize ("Watermark Size", Vector) = (0.2, 0.2, 0, 0)
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            sampler2D _WatermarkTex;
            float _WatermarkOpacity;
            float4 _WatermarkPos;
            float4 _WatermarkSize;
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                fixed4 baseColor = tex2D(_MainTex, i.uv);
                float2 wmUV = (i.uv - _WatermarkPos.xy) / _WatermarkSize.xy;
                fixed4 wmColor = tex2D(_WatermarkTex, wmUV);
                if (wmUV.x < 0 || wmUV.x > 1 || wmUV.y < 0 || wmUV.y > 1) {
                    wmColor.a = 0;
                }
                
                fixed4 finalColor = lerp(baseColor, wmColor, wmColor.a * _WatermarkOpacity);
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
