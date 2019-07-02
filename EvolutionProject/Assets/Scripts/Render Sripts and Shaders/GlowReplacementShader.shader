Shader "Hidden/GlowReplace"
{
    Properties
    {
        _GlowColour("Glow Colour", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Glowable"="True"
        }

        Pass
        {
            Blend SrcAlpha One
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            fixed4 _GlowColour;

            fixed4 frag (v2f i) : SV_Target
            {
                return _GlowColour;
            }
            ENDCG
        }
    }
}
