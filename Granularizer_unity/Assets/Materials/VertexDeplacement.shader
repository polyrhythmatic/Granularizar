  Shader "Noise/Vertex Deformation" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
	  _BumpMap("Bumpmap", 2D) = "bump" {}
      _Speed ("Speed", Range(0, 1)) = 1
      _Scale ("Scale", Range(0, 1)) = 1
      _Seed ("Seed", Float) = 0
      _Albedo ("Abledo", Color) = (1, 1, 1, 1)
      _Emission ("Emission", Color) = (0, 0, 0, 0)
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert vertex:vert
      //perlin noise stuff
         
	  float hash( float n )
	  {
	    return frac(sin(n)*43758.5453);
	  }
		 
		float noise( float3 x )
		{
		    // The noise function returns a value in the range -1.0f -> 1.0f
		 
		    float3 p = floor(x);
		    float3 f = frac(x);
		 
		    f       = f*f*(3.0-2.0*f);
		    float n = p.x + p.y*57.0 + 113.0*p.z;
		 
		    return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
		                   lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
		               lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
		                   lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
		}
		
		float fractal(float3 P) {
			float f = 0., s = 1.;
			for (int i = 0 ; i < 9 ; i++) {
				f += noise(s * P) / s;
				s *= 2.;
				P = float3(.866 * P.x + .5 * P.z, P.y + 100., -.5 * P.x + .866 * P.z);
			}
			return f;
		}
		float turbulence(float3 P) {
			float f = 0., s = 1.;
			for (int i = 0 ; i < 9 ; i++) {
				f += abs(noise(s * P)) / s;
				s *= 2.;
				P = float3(.866 * P.x + .5 * P.z, P.y + 100., -.5 * P.x + .866 * P.z);
			}
			return f;
		}
		
		//end noise
      struct Input {
          float2 uv_MainTex;
		  float2 uv_BumpMap;
		  float3 worldRefl;
      };
      float _Speed;
      float _Scale;
      float _Seed;
      fixed4 _Albedo;
      fixed4 _Emission;
      void vert (inout appdata_full v) {
      	  float timex = _Seed + _Time.y * _Speed + 0.1365143;
      	  float timey = _Seed + _Time.y * _Speed + 1.21688;
      	  float timez = _Seed + _Time.y * _Speed + 2.5564;
          v.vertex.x += _Scale * noise(float3(timex + v.vertex.x, timex + v.vertex.y, timex + v.vertex.z));//v.normal * _Amount;
          v.vertex.y += _Scale * noise(float3(timey + v.vertex.x, timey + v.vertex.y, timey + v.vertex.z));
          v.vertex.z += _Scale * noise(float3(timez + v.vertex.x, timez + v.vertex.y, timez + v.vertex.z));
      }
      sampler2D _MainTex;
	  sampler2D _BumpMap;
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = _Albedo.rgb;
          o.Emission = _Emission.rgb;
		  o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }