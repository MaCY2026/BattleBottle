using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{
    [SerializeField] private Player owner;
    [SerializeField] private Renderer waterRenderer;

    [SerializeField]
    private Color player1Color = new Color(0.15f, 0.55f, 1f, 1f);

    [SerializeField]
    private Color player2Color = new Color(1f, 0.45f, 0.1f, 1f);

    [SerializeField, Range(0f, 1f)]
    private float surfaceBrightness = 0.65f;

    private void Start()
    {
        if (owner == null)
            owner = GetComponentInParent<Player>();

        if (owner == null || waterRenderer == null)
        {
            Debug.LogWarning(
                "PlayerAppearance：Player Renderer。",
                this
            );
            return;
        }

        Color color = owner.Id == PlayerId.Player1
            ? player1Color
            : player2Color;

        var block = new MaterialPropertyBlock();

        waterRenderer.GetPropertyBlock(block);

        block.SetColor("_Side", color);
        Color surfaceColor = Color.Lerp(color, Color.white, surfaceBrightness);
        surfaceColor.a = color.a;
        block.SetColor("_Top", surfaceColor);

        waterRenderer.SetPropertyBlock(block);
    }
}
