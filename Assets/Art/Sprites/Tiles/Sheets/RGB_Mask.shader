Shader "Custom/RGB_Mask"
{
    Properties
    {
        _Color1 ("Color", Color) = (1,1,1,1)
        _Color2 ("Color", Color) = (1,1,1,1)
        _Color3 ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MaskTex ("Mask (RGB)", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump"{}
        _Metallic ("Metallic",2D) ="white" {}

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex,_MaskTex,_Metallic,_Normal;
		fixed3 _Color1, _Color2, _Color3;


        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MaskTex;
            float2 uv_Metallic;
            float2 uv_Normal;
        };

        
        

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			float3 mask = tex2D(_MaskTex, IN.uv_MainTex);
			float cmask = min(1.0, mask.r+mask.g+mask.b);
			float3 nro = tex2D(_Metallic, IN.uv_Metallic);
			float4 n = tex2D(_Normal, IN.uv_Normal);

			c.rgb = c.rgb*(1 - cmask) + (_Color1*mask.r) + (_Color2*mask.g) + (_Color3*mask.b);

            o.Albedo = c.rgb;
			o.Normal = UnpackNormal(n);
            //o.Metallic = nro.r;
            //o.Smoothness = nro.g;
			o.Occlusion = nro.b;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
