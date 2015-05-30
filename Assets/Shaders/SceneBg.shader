Shader "Custom/Background" {
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
			varying vec2 uv; 
			uniform vec4 _Color;
	
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
				vec2 center = vec2(0.5, 0.5);
				vec2 c = (uv - center) / center;

				float r = (1.0 - c.x * c.x) * (1.0 - c.y * c.y);

				gl_FragColor = vec4(_Color.rgb * r, 1.0);
			}
			#endif

			ENDGLSL
		}
	}
	FallBack "Diffuse"
}
