using UnityEngine;

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

        var main = particles.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = particles.emission;
        emission.enabled = false;
    }

    void LateUpdate()
    {
        if (Liquid == null || !Liquid.isActiveAndEnabled) return;

        pendingBubbles += BubblesPerSecond * Time.deltaTime;

        int count = Mathf.FloorToInt(pendingBubbles);
        pendingBubbles -= count;

        for (int i = 0; i < count; i++)
        {
            if (!Liquid.TryGetBubblePosition(out Vector3 position))
            {
                break;
            }

            var settings = new ParticleSystem.EmitParams
            {
                position = position,
                velocity = Vector3.up * RiseSpeed,

                applyShapeToPosition = false
            };

            particles.Emit(settings, 1);
        }
    }
}