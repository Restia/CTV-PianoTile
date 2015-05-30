Shader "Custom/PianoTile" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BgColor ("Background Color", Color) = (0.0, 0.0, 0.0, 1.0)
		_Color ("Foreground Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Fill ("Fill Percent", Float) = 0.0
	}
	SubShader {
		Pass {	
			Blend SrcAlpha OneMinusSrcAlpha
			GLSLPROGRAM
		
			uniform sampler2D _MainTex;
			varying vec2 uv; 
			uniform vec4 _BgColor;
			uniform vec4 _Color;
			uniform float _Fill;
	
			#ifdef VERTEX
			void main()
			{
				uv = gl_MultiTexCoord0.xy;
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}
			#endif
	 
			#ifdef FRAGMENT
			#define PI 3.141592653589793238462643383279

			void main()
			{
				vec2 center = vec2(0.5, 0.5);
				vec2 c = (uv - center) / center;
				vec4 color = _Color;

				// float t = step(abs(c.x), 1.0 * _Fill / 100.0) * step(abs(c.y), 1.0 * _Fill / 100.0);
				float t = _Fill / 100.0;
				color = color * t * (1.0 - c.x*c.x) * (1.0 - c.y*c.y);

				vec4 out_color = _BgColor + color;

				gl_FragColor = out_color;
			}
			#endif

			ENDGLSL
		}
	}
	FallBack "Diffuse"
}
