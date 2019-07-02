Shader "Shaders102/ImageEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DisplacementTex ("Displacement Texture", 2D) = "white" {}
        _Magnitude ("Magnitude", Range(0, 0.1)) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _DisplacementTex;
            float _Magnitude;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 disp = tex2D(_DisplacementTex, i.uv + _Time.x * 2).xy;
                disp = (disp * 2 - 1) * _Magnitude;
            
                fixed4 col = tex2D(_MainTex, i.uv + disp);
                
                return col;
                
                //return tex2D(_DisplacementTex, i.uv + timeDisp);
            }
            ENDCG
        }
    }
}
