Shader "Unlit/CoveredBoardShader"
{
    Properties
    {
        _FieldTexture ("Field", 2D) = "white" {}
        _Smoothing ("Smoothing Factor", Float) = 0.75
        _Color ("Color", Color) = (1.0, 0, 0, 1.0)
        _BoardSize ("Board Size", int) = 10
        _AddingCircle ("Adding Circle", int) = 0
        _DraggingCirclePosition ("Dragging Circle Position", Vector) = (0.0, 0.0, 0.0, 0.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members worldPosition)
#pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 worldPosition : NORMAL;
            };

            sampler2D _FieldTexture;
            float _Smoothing;
            float4 _Color;
            int _BoardSize;
            int _AddingCircle;
            float4 _DraggingCirclePosition;

            v2f vert (appdata v)
            {
                v2f o;
                // If we wanted to do all the sdf stuff in world space:
                //o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            float sdfCircle(float2 p, float2 center, float radius)
            {
                return length(center - p) - radius;
            }
            
            float opSmoothUnion( float d1, float d2, float k )
            {
                float h = max(k-abs(d1-d2),0.0);
                return min(d1, d2) - h*h*0.25/k;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(1.0, 0.0, 0.0, 1.0);
                
                float2 fieldCoord = float2(
                    floor(i.worldPosition.x + _BoardSize * 0.5),
                    floor(i.worldPosition.z + _BoardSize * 0.5)
                );
                float2 normFieldCoord = fieldCoord / _BoardSize;
                float increment = 1.0 / _BoardSize;
                float2 p = i.worldPosition.xz + float2(_BoardSize * 0.5, _BoardSize * 0.5);
                float radius = 0.49;
                float2 correction = float2(radius, radius);
                
                float distances[10] = {
                        sdfCircle(p, fieldCoord + float2(-1.0, -1.0) + correction, radius),
                        sdfCircle(p, fieldCoord + float2(0.0, -1.0) + correction, radius),
                        sdfCircle(p, fieldCoord + float2(1.0, -1.0) + correction, radius),
                        sdfCircle(p, fieldCoord + float2(-1.0, 0.0) + correction, radius),
                        sdfCircle(p, fieldCoord + float2(0.0, 0.0) + correction, radius),
                        sdfCircle(p, fieldCoord + float2(1.0, 0.0) + correction, radius),
                        sdfCircle(p, fieldCoord + float2(-1.0, 1.0) + correction, radius),
                        sdfCircle(p, fieldCoord + float2(0.0, 1.0) + correction, radius),
                        sdfCircle(p, fieldCoord + float2(1.0, 1.0) + correction, radius),
                        sdfCircle(p, _DraggingCirclePosition.xz, radius)
                };
                float fields[10] = {
                        tex2D(_FieldTexture, float2(normFieldCoord.x - increment, normFieldCoord.y - increment)).r,
                        tex2D(_FieldTexture, float2(normFieldCoord.x, normFieldCoord.y - increment)).r,
                        tex2D(_FieldTexture, float2(normFieldCoord.x + increment, normFieldCoord.y - increment)).r,
                        tex2D(_FieldTexture, float2(normFieldCoord.x - increment, normFieldCoord.y)).r,
                        tex2D(_FieldTexture, float2(normFieldCoord.x, normFieldCoord.y)).r,
                        tex2D(_FieldTexture, float2(normFieldCoord.x + increment, normFieldCoord.y)).r,
                        tex2D(_FieldTexture, float2(normFieldCoord.x - increment, normFieldCoord.y + increment)).r,
                        tex2D(_FieldTexture, float2(normFieldCoord.x, normFieldCoord.y + increment)).r,
                        tex2D(_FieldTexture, float2(normFieldCoord.x + increment, normFieldCoord.y + increment)).r,
                        (float)_AddingCircle
                };
                float distance = 1000.0;
                for (int i = 0; i < 10; i++) {
                    if (fields[i] == 0) {
                        continue;
                    }
                    distance = opSmoothUnion(distance, distances[i], _Smoothing);
                }
                
                if (distance > 0.01) {
                   discard;
                }
                return _Color;
            }
            ENDCG
        }
    }
}
