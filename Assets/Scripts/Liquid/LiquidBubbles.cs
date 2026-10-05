using UnityEngine;

// LiquidVolume に泡を出す位置には依存している。LateUpdateの処理順番はLiquidVolume→LiquidBubbles
[DefaultExecutionOrder(100)]
[RequireComponent(typeof(ParticleSystem))]
public class LiquidBubbles : MonoBehaviour
{
    public LiquidVolume Liquid;

    [Min(0f)] public float BubblesPerSecond = 20f;
    [Min(0f)] public float RiseSpeed = 0.25f;

    ParticleSystem particles;
    float pendingBubbles;

    void Awake()
    {
        particles = GetComponent<ParticleSystem>();

        // 発生位置をワールド座標として扱う
        var main = particles.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        // パーティクルの自動生成を無効にする
        var emission = particles.emission;
        emission.enabled = false;
    }

    void LateUpdate()
    {
        if (Liquid == null || !Liquid.isActiveAndEnabled) return;

        /* 毎回、整数部分を取り出してそのフレームで生成する泡の数にし、残った小数部分はそのまま次のフレームに持ち越す。
        | フレーム | たまった数 | 生成する泡の数 | 次に持ち越す数 |
        | 1      | 0.4      | 0個         | 0.4        |
        | 2      | 0.8      | 0個         | 0.8        |
        | 3      | 1.2      | 1個         | 0.2        |
        | 4      | 0.6      | 0個         | 0.6        |
        | 5      | 1.0      | 1個         | 0          |
        */
        pendingBubbles += BubblesPerSecond * Time.deltaTime;
        // 生成する泡の数
        int count = Mathf.FloorToInt(pendingBubbles);
        pendingBubbles -= count;

        // 泡を生成する
        for (int i = 0; i < count; i++)
        {
            // Liquidから泡の発生位置を取得する。
            if (!Liquid.TryGetBubblePosition(out Vector3 bubblePosition)) break;

            // 泡を生成するときに使う設定を作る
            var settings = new ParticleSystem.EmitParams
            {
                position = bubblePosition,
                velocity = Vector3.up * RiseSpeed,
                applyShapeToPosition = false
            };

            // 設定した位置と速度で泡を1個生成する
            particles.Emit(settings, 1);
        }
    }
}