Shader "Unlit/DiceShader"
{
    Properties
    {
        _MainTex ("Dice Faces Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
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
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float faceIndex : TEXCOORD1; // Передаем индекс грани
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _BaseColor;

            // Функция, которая преобразует нормаль в индекс грани (0-5)
            float GetFaceIndex(float3 normal)
            {
                float3 absNormal = abs(normal);
                float majorAxis = max(absNormal.x, max(absNormal.y, absNormal.z));

                // Определяем, какая грань "смотрит" на камеру сильнее всего
                if (majorAxis == absNormal.y) return (normal.y > 0) ? 0 : 5; // Top: 0, Bottom: 5
                else if (majorAxis == absNormal.x) return (normal.x > 0) ? 2 : 3; // Right: 2, Left: 3
                else return (normal.z > 0) ? 1 : 4; // Front: 1, Back: 4
                // Важно: Нумерация может не соответствовать стандартной игре.
                // Это всего лишь пример. Вам нужно настроить сопоставление под свою текстуру.
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.faceIndex = GetFaceIndex(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Рассчитываем UV координату для конкретной грани
                // Предполагаем, что 6 граней расположены в один ряд (1x6)
                float2 diceUV = float2((i.uv.x / 6.0) + (i.faceIndex / 6.0), i.uv.y);

                // Считываем цвет из текстуры
                fixed4 texColor = tex2D(_MainTex, diceUV);
                // Накладываем базовый цвет
                return texColor * _BaseColor;
            }
            ENDCG
        }
    }
}
