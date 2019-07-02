Shader "Glow/GlowImageEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
                float4 vertex : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _GlowPrePassTex;
            sampler2D _GlowBlurredTex;
            
            float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv0);
                fixed4 prepass = tex2D(_GlowPrePassTex, i.uv0);
                fixed4 blurred = tex2D(_GlowBlurredTex, i.uv0);
                fixed4 glow = max(0, blurred - prepass);
                //return col;
                
                return col + glow * _Intensity;
            }
            ENDCG
        }
    }
}
