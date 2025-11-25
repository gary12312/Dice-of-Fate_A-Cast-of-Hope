Shader "Unlit/VertexColorShader"
{
    Properties
    {
        // ћожно добавить свойства, но дл€ нашей задачи они не об€зательны
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL; // »спользуем нормаль дл€ определени€ грани
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed3 color : COLOR0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                // –аскрашиваем грани в зависимости от направлени€ нормали
                o.color = v.normal * 0.5 + 0.5; // ѕреобразуем из диапазона (-1,1) в (0,1)
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ¬озвращаем цвет, полученный из вершинного шейдера
                return fixed4(i.color, 1.0);
            }
            ENDCG
        }
    }
}
