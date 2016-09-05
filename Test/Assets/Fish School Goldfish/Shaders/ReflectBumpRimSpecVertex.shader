Shader "Unluck Software/ReflectBumpRimSpecVertex" {
Properties {
	_Color2 ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
	_MainTex ("Base (RGB) RefStrGloss (A)", 2D) = "white" {}
	_Cube ("Reflection Cubemap", Cube) = "" { TexGen CubeReflect }
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
    _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 400
	
CGPROGRAM
#pragma surface surf BlinnPhong
#pragma target 3.0
//input limit (8) exceeded, shader uses 9
//#pragma exclude_renderers d3d11_9x

sampler2D _MainTex;
sampler2D _BumpMap;
samplerCUBE _Cube;
fixed4 _Color;
fixed4 _Color2;
fixed4 _ReflectColor;
half _Shininess;
float4 _RimColor;
float _RimPower;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float3 worldRefl;
	 float4 color : COLOR;
	 float3 viewDir;
	INTERNAL_DATA
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

	o.Albedo = tex.rgb * IN.color.rgb *_Color2;
	
	o.Gloss = tex.a;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
	
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	
	float3 worldRefl = WorldReflectionVector (IN, o.Normal);
	fixed4 reflcol = texCUBE (_Cube, worldRefl);
	reflcol *= tex.a;
		half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
	o.Emission = reflcol.rgb * _ReflectColor.rgb+(_RimColor.rgb * pow (rim, _RimPower));

	//o.Alpha = reflcol.a * _ReflectColor.a;
}
ENDCG
}

FallBack "Specular"
}
