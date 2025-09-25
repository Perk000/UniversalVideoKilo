Shader "Silver Tau/NSR - Screen Recorder/Web Camera/FlipRotateShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlipVertical ("Flip Vertical", Int) = 0
        _FlipHorizontal ("Flip Horizontal", Int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            int _FlipVertical;
            int _FlipHorizontal;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                if (_FlipHorizontal == 1)
                {
                    uv.x = 1.0 - uv.x;
                }
                if (_FlipVertical == 1)
                {
                    uv.y = 1.0 - uv.y;
                }
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
