using UnityEngine;

public class Wobble : MonoBehaviour
{
    Renderer rend;
    MaterialPropertyBlock properties;
    static readonly int WobbleXId = Shader.PropertyToID("_WobbleX");
    static readonly int WobbleZId = Shader.PropertyToID("_WobbleZ");
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;
    public float MaxWobble = 0.02f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        properties = new MaterialPropertyBlock();
        lastPos = transform.position;
        lastRot = transform.eulerAngles;
    }
    private void Update()
    {
        if (rend == null || Time.deltaTime <= 0f) return;
        time += Time.deltaTime;

        // 揺れの強さを少しずつ減らす
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        // Sin で左右に揺らす
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX =
            wobbleAmountToAddX * Mathf.Sin(pulse * time)
            + wobbleAmountToAddZ * 0.4f
            * Mathf.Sin(pulse * time * 1.3f + 1f);

        wobbleAmountZ =
            wobbleAmountToAddZ * Mathf.Sin(pulse * time)
            + wobbleAmountToAddX * 0.4f
            * Mathf.Sin(pulse * time * 1.1f + 2f);

        // 計算した揺れをシェーダーへ送る
        rend.GetPropertyBlock(properties);
        properties.SetFloat(WobbleXId, wobbleAmountX);
        properties.SetFloat(WobbleZId, wobbleAmountZ);
        rend.SetPropertyBlock(properties);

        // 瓶がどれだけ動いたかを調べる
        velocity = (lastPos - transform.position) / Time.deltaTime;
        Vector3 rotation = transform.eulerAngles;
        angularVelocity = new Vector3(
            Mathf.DeltaAngle(lastRot.x, rotation.x),
            Mathf.DeltaAngle(lastRot.y, rotation.y),
            Mathf.DeltaAngle(lastRot.z, rotation.z));

        // 動きに応じて揺れを追加する
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        // 今の位置と回転を保存する
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }

    public Vector3 SurfaceNormal
    {
        get
        {
            return new Vector3(
                wobbleAmountX,
                1f,
                -wobbleAmountZ
            ).normalized;
        }
    }

}
