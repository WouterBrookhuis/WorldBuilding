Shader "Custom/Terrain"
{
    Properties
    {
        _Color ("Flat Color", Color) = (1,1,1,1)
		_VertColor ("Vertical Color", Color) = (1,1,1,1)
        _MainTex ("Flat (RGB)", 2D) = "white" {}
		_VertTex ("Vertical (RGB)", 2D) = "white" {}
		_BlendStart ("Blend Start", Range(0, 1)) = 0.3
		_BlendEnd("Blend End", Range(0, 1)) = 0.5
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _VertTex;

        struct Input
        {
            float2 uv_MainTex;
			float3 worldNormal;
        };

		half _BlendStart;
		half _BlendEnd;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _VertColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert(in appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
			float3 up = { 0, 1.0, 0 };
			float normalDot = dot(up, IN.worldNormal);
            fixed4 c = 
				normalDot < _BlendStart ? tex2D(_VertTex, IN.uv_MainTex) * _VertColor :
				normalDot > _BlendEnd ? tex2D(_MainTex, IN.uv_MainTex) * _Color :
				lerp(tex2D(_MainTex, IN.uv_MainTex) * _Color, tex2D(_VertTex, IN.uv_MainTex) * _VertColor, 1 - (normalDot - _BlendStart) / (_BlendEnd - _BlendStart));
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
