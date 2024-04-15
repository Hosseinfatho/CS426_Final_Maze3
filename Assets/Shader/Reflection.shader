Shader "Custom/ReflectiveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        
        struct Input
        {
            float2 uv_MainTex;
        };
        
        sampler2D _MainTex;
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Metallic = 1.0;
            o.Smoothness = 1.0;
        }
        ENDCG
    }
    FallBack "Standard"
}