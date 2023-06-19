Shader "Hidden/Custom/MultiScreen"
{
    HLSLINCLUDE

    //Std.hlslにはポストエフェクトで共通に利用される
    //中身にはデフォルトの頂点シェーダーVertDefaultやデフォルトの構造体VaryingaDefaultが定義されている
    //これによりFrag関数以外はテンプレート処理になっている？
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

    float _DivisionNumber; //分割数
    float2 _HueShift; //グリッドごとの色相変化速度
    float _Blend; //色のブレンド率

    //ユーザ関数
    float3 hsvToRgb(float3 c)
    {
        float4 K =float4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
        float3 p = abs(frac(c.xxx + K.xyz)*6.0 - K.www);
        return c.z * lerp(K.xxx, saturate(p-K.xxx), c.y);
    }

    //ポストエフェクト効果の処理はこのFrag関数の中に実装
    float4 Frag(VaryingsDefault i):SV_Target
    {
        //UVに_DivisionNumberを乗算して、小さい画面がたくさん並ぶ「マルチスクリーン」の効果を加える
        float2 uv = i.texcoord * _DivisionNumber;

        //テクスチャを参照
        float4 color = SAMPLE_TEXTURE2D(_MainTex,  sampler_MainTex, frac(uv));

        //輝度を求める（グレースケール化）
        float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));

        //どのグリッドに属すかの数値を計算
        float2 grid = floor(uv);

        //グリッドごとのカラフルな色を生成する
        float3 palette = hsvToRgb(float3(dot(grid, _HueShift), 1.0, 1.0));

        //グリッドごとのカラフルな色を設定したブレンド率で合成
        color.rgb = lerp(color.rgb, luminance * palette, _Blend);

        return color;
    }

    ENDHLSL

    Subshader
    {
        Cull Off ZWrite Off Ztest ALways
        Pass{
            HLSLPROGRAM
            #pragma vertex VertDefault;
            #pragma fragment Frag;
            ENDHLSL
        }
    }
}