using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class LensControl : MonoBehaviour
{
    private Volume _Volume;
    private LensDistortion _Lens;
    private float _Parameter;
    private float random;
    float noiseSpeed;
    float speed=1;
    private float _timeInterval=0.02f;
    private float _timeElapsed=0;

    private void Start()
    {
        _Volume = GetComponent<Volume>();
        
        _Lens = null;
        _Volume.profile.TryGet<LensDistortion>(out _Lens);
       
      
    }

    private void Update()
    {
        noiseSpeed=Time.time*speed;
        _Parameter= 8 * (Mathf.PerlinNoise(noiseSpeed, 0) - 0.5f);
        random =Random.Range(0,0.25f);
        
        _timeElapsed += Time.deltaTime;
    
    if (_timeElapsed  > _timeInterval)
    {
        /*-- 一定間隔で実行したい処理 --*/
        _Lens.intensity.value = _Parameter;
        _Lens.xMultiplier.value=(random+ 0.5f);
        _Lens.yMultiplier.value=Mathf.PerlinNoise(noiseSpeed, 0);
        _Lens.center.value=new Vector2(Mathf.Sin(noiseSpeed)*_Parameter*1/5+0.5f,Mathf.Cos(noiseSpeed)*_Parameter*1/6+0.5f);
        _Lens.scale.value=(_Parameter+3)/4+0.5f;

        // 経過時間を元に戻す
        _timeElapsed = 0f;

    }

       
    }
}