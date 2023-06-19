/*MultiScreenはポストエフェクトの設定を定義するクラス
ポストエフェクト用のシェーダーに必要なプロパティを定義している
*/
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/*カスタムエフェクトを追加するのに必要なファイルは
C#のスクリプト(.cs)とシェーダー(.shader)の2つ
*/

//設定はScritableObjectとして保存されるため[Serializable]属性は必須
[Serializable]
/*PostProcess属性
第2引数：ポストエフェクトの実行タイミングとして「PostProccesEvent.Atack」を指定
第3引数：エフェクトのコンテキストメニュー上の名前
第4引数：SceneView（GameViewじゃないよ！）上で効果を有効にするかどうかのフラグ
*/
[PostProcess(typeof(MultiScreenRenderer), PostProcessEvent.AfterStack, "Custom/MultiScreen", false)]

public sealed class MultiScreen : PostProcessEffectSettings
{
    //クラスのフィールドとしてプロパティを定義（以下構文通りなので適宜調べること）

    [Range(1f, 10f), Tooltip("分割数")]
    public FloatParameter divisionNumber = new FloatParameter{value = 3f};

    [Tooltip("グリッドごとの色相の変化速度")]
    public Vector2Parameter hueShift = new Vector2Parameter{value = new Vector2(0.1f, 0.3f)};

    [Range(0f, 1f), Tooltip("色のブレンド率")]
    public FloatParameter blend = new FloatParameter{value = 0.5f};
}

//ポストエフェクトのレンダリングを実行するためのクラス
//PostProcessEffectRender<T>には色々なメソッドが定義されているので必要に応じてoverrideすること？
public sealed class MultiScreenRenderer : PostProcessEffectRenderer<MultiScreen>
{
    public override void Render(PostProcessRenderContext context)
    {
        //シェーダーにプロパティを渡す
        var sheet = context.propertySheets.Get
                    (Shader.Find("Hidden/Custom/MultiScreen"));
        sheet.properties.SetFloat("_DivisionNumber", settings.divisionNumber);
        sheet.properties.SetVector("_HueShift", settings.hueShift);
        sheet.properties.SetFloat("_Blend", settings.blend);
        
        //最後にBlitFullscreenTriangleを呼び出す
        context.command.BlitFullscreenTriangle
                        (context.source, context.destination, sheet, 0);
    }
}