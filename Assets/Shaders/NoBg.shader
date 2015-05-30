Shader "Custom/NoBg" {
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
			varying vec2 uv;
	
			#ifdef VERTEX
			void main()
			{
				uv = gl_MultiTexCoord0.xy;
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}
			#endif
	 
			#ifdef FRAGMENT

			float func(vec2 point, vec2 center)
			{
				return length(point - center);
			}

			void main()
			{
				vec2 center = vec2(0.5, 0.5);
				float d = func(uv, center);

				float dfdx = func(vec2(uv.x + 0.05, uv.y), center) - d;
				float dfdy = func(vec2(uv.x, uv.y + 0.05), center) - d;

				float delta = abs(dfdx) + abs(dfdy);

				float r = 1.0 - smoothstep(0.5 - delta, 0.5, d);
				gl_FragColor = _Color * r;
			}
			#endif

			ENDGLSL
		}
	}
	FallBack "Diffuse"
}
