Shader "SupGames/Mobile/PostProcess"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "" {}
		_MaskTex("Base (RGB)", 2D) = "" {}
		_BlurTex("Base (RGB)", 2D) = "" {}
		_Amount("Amount of Color Gradint", float) = 1.0
		_ScrRes ("Screen Resolution", Vector) = (0.,0.,0.,0.)	
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	struct v2fb {
		half4 pos : POSITION;
		half2 uv  : TEXCOORD0;
		half4 uv1 : TEXCOORD1;
		half4 uv2 : TEXCOORD2;
	};
	struct v2fbu {
		half4 pos : POSITION;
		half2 uv  : TEXCOORD0;
		half4 uv1 : TEXCOORD1;
		half4 uv2 : TEXCOORD2;
		half4 uv3 : TEXCOORD3;
		half4 uv4 : TEXCOORD4;
	};
	struct v2f {
		half4 pos : POSITION;
		half2 uv  : TEXCOORD0;
	};

	sampler2D _MainTex;
	sampler2D _LutTex;
	sampler2D _MaskTex;
	sampler2D _BlurTex;
	uniform half _Amount;
	uniform half _BloomThreshold;
	uniform half _BloomAmount;
	uniform half4 _ScrRes;
	v2fb vertBlur(appdata_img v)
	{
		v2fb o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		o.uv1.xy = v.texcoord.xy + fixed2(-_ScrRes.x, -_ScrRes.y);
		o.uv1.zw = v.texcoord.xy + fixed2(-_ScrRes.x, _ScrRes.y);
		o.uv2.xy = v.texcoord.xy + fixed2(_ScrRes.x, -_ScrRes.y);
		o.uv2.zw = v.texcoord.xy + fixed2(_ScrRes.x, _ScrRes.y);
		return o;
	}
	v2fbu vertBlur3(appdata_img v)
	{
		v2fbu o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		o.uv1.xy = v.texcoord.xy + half2(_ScrRes.x, _ScrRes.y);
		o.uv1.zw = v.texcoord.xy + half2(-_ScrRes.x, _ScrRes.y);
		o.uv2.xy = v.texcoord.xy + half2(-_ScrRes.x, -_ScrRes.y);
		o.uv2.zw = v.texcoord.xy + half2(_ScrRes.x, -_ScrRes.y);
		o.uv3.xy = v.texcoord.xy + half2(0.0h, 2.0h*_ScrRes.y);
		o.uv3.zw = v.texcoord.xy + half2(0.0h, -2.0h*_ScrRes.y);
		o.uv4.xy = v.texcoord.xy + half2(2.0h*_ScrRes.x, 0.0h);
		o.uv4.zw = v.texcoord.xy + half2(-2.0h*_ScrRes.x, 0.0h);
		return o;
	}

	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv =  v.texcoord.xy;	
		return o;
	} 

	fixed2 GetUV(fixed4 c)
	{
		half b = floor(c.b * 256.0h);
		half by = floor(b *0.0625h);
		half bx = floor(b - by * 16.0h);
		half2 uv = c.rg *0.0586h + 0.001953h + half2(bx, by) *0.0625h;
		return uv;
	}
	fixed4 fragBlur2(v2fb i) : COLOR 
	{
		fixed4 result = tex2D(_MainTex, i.uv);
		result += tex2D(_MainTex, i.uv1.xy);
		result += tex2D(_MainTex, i.uv1.zw);
		result += tex2D(_MainTex, i.uv2.xy);
		result += tex2D(_MainTex, i.uv2.zw);
		return result * 0.2h;
	}

	#define oneSix     0.1666666h
	#define oneThree   0.3333333h
	fixed4 fragBlur3(v2fbu i) : COLOR
	{
		fixed4 result = oneThree*tex2D(_MainTex, i.uv1.xy);
		result += oneThree*tex2D(_MainTex, i.uv1.zw);
		result += oneThree*tex2D(_MainTex, i.uv2.xy);
		result += oneThree*tex2D(_MainTex, i.uv2.zw);
		result += oneSix*tex2D(_MainTex, i.uv3.xy);
		result += oneSix*tex2D(_MainTex, i.uv3.zw);
		result += oneSix*tex2D(_MainTex, i.uv4.xy);
		result += oneSix*tex2D(_MainTex, i.uv4.zw);
		return result *0.5h;
	}
	fixed4 frag(v2f i) : COLOR
	{
		fixed4 c = tex2D(_MainTex, i.uv);
		fixed4 bl = tex2D(_BlurTex, i.uv);
		fixed4 m = tex2D(_MaskTex, i.uv);
		fixed4 lc= lerp(c,tex2D(_LutTex, GetUV(c)),_Amount);
		fixed4 lb= lerp(bl,tex2D(_LutTex, GetUV(bl)),_Amount);
		return lerp(lc, lb , m.r)+ max(lb-_BloomThreshold,0.0h)*_BloomAmount;
	}
	fixed4 fragLut(v2f i) : COLOR 
	{
		fixed4 c = tex2D(_MainTex, i.uv);
		fixed4 lc= tex2D(_LutTex, GetUV(c));
		return lerp(c,lc,_Amount);
	}
	fixed4 fragBlurOnly(v2f i) : COLOR
	{
		fixed4 c = tex2D(_MainTex, i.uv);
		fixed4 bl = tex2D(_BlurTex, i.uv);
		fixed4 m = tex2D(_MaskTex, i.uv);
		return lerp(c,bl,m.r)+ max(bl-_BloomThreshold,0.0h)*_BloomAmount;
	}
	ENDCG 
		
	Subshader 
	{
		Pass 
		{
		  ZTest Always Cull Off ZWrite Off
		  Fog { Mode off }      

	      CGPROGRAM
	      #pragma vertex vert
	      #pragma fragment frag
	      #pragma fragmentoption ARB_precision_hint_fastest
		  #pragma target 3.0
	      ENDCG
	  	}
	  	Pass 
		{
		  ZTest Always Cull Off ZWrite Off
		  Fog { Mode off }      
	      CGPROGRAM
	      #pragma vertex vert
	      #pragma fragment fragLut
	      #pragma fragmentoption ARB_precision_hint_fastest
		  #pragma target 3.0
	      ENDCG
	  	}
		Pass
		{
		  ZTest Always Cull Off ZWrite Off
		  Fog { Mode off }
		  CGPROGRAM
		  #pragma vertex vertBlur
		  #pragma fragment fragBlur2
		  #pragma fragmentoption ARB_precision_hint_fastest
		  #pragma target 3.0
		  ENDCG
		}
		Pass
		{
		  ZTest Always Cull Off ZWrite Off
		  Fog { Mode off }
		  CGPROGRAM
		  #pragma vertex vertBlur3
		  #pragma fragment fragBlur3
		  #pragma fragmentoption ARB_precision_hint_fastest
		  #pragma target 3.0
		  ENDCG
		}
		Pass 
		{
		  ZTest Always Cull Off ZWrite Off
		  Fog { Mode off }      
	      CGPROGRAM
	      #pragma vertex vert
	      #pragma fragment fragBlurOnly
	      #pragma fragmentoption ARB_precision_hint_fastest
		  #pragma target 3.0
	      ENDCG
	  	}
	}

	Fallback off
	}