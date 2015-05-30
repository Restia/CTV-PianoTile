Shader "Custom/Avatar" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		Pass {	
			Blend SrcAlpha OneMinusSrcAlpha
			GLSLPROGRAM
		
			uniform sampler2D _MainTex;
			uniform vec4 _Color;
			varying vec2 uv;
	
			#ifdef VERTEX
			void main()
			{
				uv = gl_MultiTexCoord0.xy;
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}
			#endif
	 
			#ifdef FRAGMENT
			void main()
			{
				gl_FragColor = texture2D(_MainTex, uv).rgba;
			}
			#endif

			ENDGLSL
		}
	}
	FallBack "Diffuse"
}
