// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/InvCircleShadow" {
	Properties
	{
		Radius("radius", float) = 0.01
		Center("center", vector) = (0.0, 0.0, 0.0, 0.0)
	}
    SubShader {
		Tags { "Queue"="Overlay" "RenderType"="Transparent" }
		ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			float Radius;
			vector Center;
            struct vertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };

            struct fragmentInput{
                float4 position : SV_POSITION;
                float4 texcoord0 : TEXCOORD0;
            };

            fragmentInput vert(vertexInput i){
                fragmentInput o;
                o.position = UnityObjectToClipPos (i.vertex);
                o.texcoord0 = i.texcoord0;
                return o;
            }

            fixed4 frag(fragmentInput i) : SV_Target {
                fixed4 color;

				float2 v = Center.xy + i.texcoord0.xy - float2(0.5, 0.5);
				if(dot(v,v) < Radius){
					color = fixed4(0.0,0.0,0.0,0.0);
				}else{
					color = fixed4(0.0,0.0,0.0,0.65);
				}

                return color;
            }
            ENDCG
        }
    }
}
