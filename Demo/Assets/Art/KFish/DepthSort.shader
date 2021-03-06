﻿Shader "Custom/DepthSort" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader {
        //Tags { "RenderType"="Overlay" } 
        Tags { "Queue"="Overlay-20" }        //我们的
        Zwrite On        //通知unity重写物体的渲染深度顺序。
		ZTest Off
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    } 
    FallBack "Diffuse"
}