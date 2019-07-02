Shader "Hidden/OverDrawShader"
{
    Properties
    {
        _Color("Colour", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
        }

        Pass
        {
            ZTest Always
            ZWrite Off
            Blend One One
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            half4 _OverDrawColour;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OverDrawColour;
            }
            ENDCG
        }
    }
}
