Shader "Jacob/Toon" {
   Properties {
      _Color ("Diffuse Color", Color) = (1,1,1,1) 
      _ShadowThreshold ("Shadow Threshold", Range(0.0,1.0)) = 0.5
      _UnlitBrightness("Unlit Brightness", Range(0.0,1.0)) = 0.5
      _HighlightThreshold ("Highlight Threshold", Range(0.0,1.0)) = 0.0
      _SpecColor ("Highlight Color", Color) = (1,1,1,1) 
      
      _OutlineColor ("Outline Color", Color) = (0,0,0,1)
      _OutlineThickness ("Outline Thickness", Range(0,1)) = 0.01
      _InnerDetailOutlineThickness ("Inner Detail Outline Thickness", Range(0,1)) = 0.005

   }
   SubShader {
   
   		CGINCLUDE
   		
     	#include "UnityCG.cginc"

       // User-specified properties
         uniform float4 _Color; 

         uniform float _ShadowThreshold;
   		 uniform float _UnlitBrightness;
         
         uniform float _HighlightThreshold;
	     uniform float4 _SpecColor; 
         
   		uniform float4 _OutlineColor;

        struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posWorld : TEXCOORD0;
            float3 normalDir : TEXCOORD1;
         };
         
                  vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = _Object2World;
            float4x4 modelMatrixInverse = _World2Object; 
               // multiplication with unity_Scale.w is unnecessary 
               // because we normalize transformed vectors
 
            output.posWorld = mul(modelMatrix, input.vertex);
            output.normalDir = normalize(
               mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float3 normalDirection = normalize(input.normalDir);
 
            float3 viewDirection = normalize(
               _WorldSpaceCameraPos - input.posWorld.xyz);

 			float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
 
 			float lightIncidenceFactor = -dot(normalDirection, lightDirection)*0.5f + 0.5f;
 		
 			int inShadow = _ShadowThreshold < lightIncidenceFactor;
 			int inHighlight = lightIncidenceFactor < _HighlightThreshold;
 
            float3 fragmentColor = 
            	inHighlight * _SpecColor.rgb +
            	!inHighlight * _Color.rgb * ( inShadow * _UnlitBrightness +
            				   				  1.0f     * !inShadow);
 
            return float4(fragmentColor, 1.0);
         }
 
	    struct outlineInput {
	    	float4 vertex : POSITION;
	        float3 normal : NORMAL;
	    };
	         struct outlineOutput {
	            float4 pos : SV_POSITION;
	            float4 color : COLOR;
	         };

			outlineOutput outlineVertImpl(outlineInput i, float thickness) {
				// just make a copy of incoming vertex data but scaled according to normal direction
				outlineOutput o;
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
			 
				float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, i.normal);
				float2 offset = TransformViewToProjection(norm.xy);
			 
				o.pos.xy += offset * o.pos.z * thickness;
				o.color = _OutlineColor;
				return o;
			}
		
			half4 outlineFrag(outlineOutput i) :COLOR {
				return i.color;
			}

   		ENDCG
   
   
     	Pass {
   			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Back
			ZWrite Off
			ZTest Always
			ColorMask RGB // alpha not used
			//
 
			// you can choose what kind of blending mode you want for the outline
			Blend SrcAlpha OneMinusSrcAlpha // Normal
			// Blend One One // Additive
			//Blend One OneMinusDstColor // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend DstColor SrcColor // 2x Multiplicative
   	
   		CGPROGRAM
   	
			#pragma vertex outlineVert 
         	#pragma fragment outlineFrag
         	
 		    uniform float _OutlineThickness;

         	outlineOutput outlineVert( outlineInput i ){
         		return outlineVertImpl( i, _OutlineThickness );
         	}
 
			ENDCG
   	}

      Pass {      
      	Name "BASE"
         Tags { "LightMode" = "ForwardBase" } 
         	Cull Back
         	ZWrite On
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
            // pass for ambient light and first light source
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         ENDCG
      }
      
     	Pass {
   			Name "INNER_OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Front
			ZWrite On
			ZTest Less
			ColorMask RGB // alpha not used
			Offset 4, 4
 
			// you can choose what kind of blending mode you want for the outline
			Blend SrcAlpha OneMinusSrcAlpha // Normal
			// Blend One One // Additive
			//Blend One OneMinusDstColor // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend DstColor SrcColor // 2x Multiplicative
   	
   		CGPROGRAM
   	
			#pragma vertex outlineVert 
         	#pragma fragment outlineFrag
 
	 	    uniform float _InnerDetailOutlineThickness;

			outlineOutput outlineVert( outlineInput i ){
				return outlineVertImpl( i, _InnerDetailOutlineThickness );
			}
 
			ENDCG
   	}
   
	} 

   // The definition of a fallback shader should be commented out 
   // during development:
   // Fallback "Specular"
}
