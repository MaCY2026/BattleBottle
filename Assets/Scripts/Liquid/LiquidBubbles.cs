using UnityEngine;

// LiquidVolume に泡を出す位置には依存している。LateUpdateの処理順番はLiquidVolume→LiquidBubbles
[DefaultExecutionOrder(100)]
[RequireComponent(typeof(ParticleSystem))]
public class LiquidBubbles : MonoBehaviour
{
    public LiquidVolume Liquid;
    [SerializeField] private Wobble wobble;

    [SerializeField, Min(0f)] private float BubblesPerSecond = 20f;
    [SerializeField, Min(0f)] private float RiseSpeed = 0.25f;
    // 水面に到達する手前で泡を消す距離。
    [SerializeField, Range(0f, 1f)] private float inwardScaleFactor = 0.05f;
    [SerializeField, Min(0f)] private float surfaceGenerateDistance = 0.13f;

    ParticleSystem particles;
    float pendingBubbles;

    // バッファを取得
    ParticleSystem.Particle[] bubbleBuffer;

    void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        bubbleBuffer = new ParticleSystem.Particle[particles.main.maxParticles];

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

        GenerateBubble();
        RemoveBubbleOutofRange();
    }

    void GenerateBubble()
    {
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


        var liquidPos = Liquid.transform.position;
        // 泡を生成する
        for (int i = 0; i < count; i++)
        {
            // Liquidから泡の発生位置を取得する。
            if (!Liquid.TryGetBubblePosition(out Vector3 bubblePosition)) break;

            // 水面と近すぎる位置なら、この泡の生成を見送る
            if (bubblePosition.y > Liquid.Level + liquidPos.y - surfaceGenerateDistance)
                continue;

            // 泡を生成するときに使う設定を作る
            var settings = new ParticleSystem.EmitParams
            {
                position = bubblePosition + (liquidPos - bubblePosition) * inwardScaleFactor,
                velocity = Vector3.up * RiseSpeed,
                applyShapeToPosition = false
            };

            // 設定した位置と速度で泡を1個生成する
            particles.Emit(settings, 1);
        }
    }

    void RemoveBubbleOutofRange()
    {
        int count = particles.GetParticles(bubbleBuffer);
        for (int i = 0; i < count; i++)
        {
            // 水面上の基準点
            Vector3 surfacePoint = Liquid.PivotPoint;

            // 水面の単位法線ベクトルを取得する
            Vector3 normal = wobble.SurfaceNormal;

            // 水面上の点から泡に向かうベクトルを求める
            Vector3 toBubble = bubbleBuffer[i].position - surfacePoint;

            // 内積で、泡と水面の符号付き距離を求める
            // 正：水面より上、0：水面上、負：水面より下
            float distanceToLevel = Vector3.Dot(toBubble, normal);

            // 水面に到達する泡を消す
            if (distanceToLevel >= 0)
            {
                bubbleBuffer[i].remainingLifetime = 0f;
            }

            Vector3 localPosition = Liquid.transform.InverseTransformPoint(bubbleBuffer[i].position);

            float distanceToPivotX = Mathf.Abs(localPosition.x);

            // 容器の外に到達する時泡を消す
            if (IsOutsideBottle(bubbleBuffer[i].position))
            {
                bubbleBuffer[i].remainingLifetime = 0f;
            }
        }
        particles.SetParticles(bubbleBuffer, count);
    }

    [SerializeField] private float bottomY = -0.057f;
    [SerializeField] private float shoulderY = 0.06f;
    [SerializeField] private float topY = 0.088f;

    [SerializeField, Min(0f)]
    private float bodyHalfWidth = 0.025f;

    [SerializeField, Min(0f)]
    private float mouthHalfWidth = 0.01f;

    private bool IsOutsideBottle(Vector3 worldPosition)
    {
        Vector3 localPosition = Liquid.transform.InverseTransformPoint(worldPosition);

        float x = Mathf.Abs(localPosition.x);
        float y = localPosition.y;

        float slope = (mouthHalfWidth - bodyHalfWidth) / (topY - shoulderY);

        float intercept = bodyHalfWidth - slope * shoulderY;

        return
            x > bodyHalfWidth ||           // 左右
            x > slope * y + intercept ||   // 傾き
            y < bottomY ||                 // 底
            y > topY;                      // 口
    }
}