Shader "Custom/MenuItemBg" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Foreground Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		Pass {	
			Blend SrcAlpha OneMinusSrcAlpha
			GLSLPROGRAM
		
			uniform sampler2D _MainTex;
			uniform vec4 _Color;
	
			#ifdef VERTEX
			void main()
			{
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}
			#endif
	 
			#ifdef FRAGMENT
			void main()
			{
				gl_FragColor = _Color;
			}
			#endif

			ENDGLSL
		}
	}
	FallBack "Diffuse"
}
