using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]   // PlayerのUpdateより早い
public class PlayerInputManager : MonoBehaviour
{
    [Header("キーボード")]
    [SerializeField] private bool useKeyboard = true;
    [SerializeField] private float rotatePower = 5f;
    [SerializeField] private Key leftKey = Key.LeftArrow;
    [SerializeField] private Key rightKey = Key.RightArrow;
    [SerializeField] private Key jetKey = Key.Z;
    [SerializeField] private Key jumpKey = Key.Space;

    // 他のクラスから読み取れる入力値
    public Vector3 Direction { get; private set; }
    public float Rotate { get; private set; }
    public bool JetHeld { get; private set; }
    public bool JumpReleased { get; private set; }

    private Player owner;

    private void Awake()
    {
        owner = GetComponent<Player>();
    }

    private void Update()
    {
        // 毎フレーム初期化する
        Direction = Vector3.zero;
        Rotate = 0f;
        JetHeld = false;
        JumpReleased = false;

        ReadKeyboard();
        ReadGamepad();
    }

    private void ReadKeyboard()
    {
        Keyboard keyboard = Keyboard.current;

        if (!useKeyboard || keyboard == null)
            return;

        // 移動
        if (keyboard.aKey.isPressed) Direction = new Vector2(-1f, 0f);
        if (keyboard.dKey.isPressed) Direction = new Vector2(1f, 0f);
        if (keyboard.sKey.isPressed) Direction = new Vector2(0f, -1f);
        if (keyboard.wKey.isPressed) Direction = new Vector2(0f, 1f);

        // 回転
        if (keyboard[leftKey].isPressed)
            Rotate += rotatePower;
        if (keyboard[rightKey].isPressed)
            Rotate -= rotatePower;

        // 噴射
        JetHeld = keyboard[jetKey].isPressed;

        // ジャンプ
        JumpReleased = keyboard[jumpKey].wasReleasedThisFrame;
    }

    private void ReadGamepad()
    {
        if (owner == null) return;

        int gamepadIndex = (int)owner.Id;

        if (gamepadIndex < 0 || gamepadIndex >= Gamepad.all.Count)
            return;

        Gamepad gamepad = Gamepad.all[gamepadIndex];

        float stickX = gamepad.leftStick.ReadValue().x;

        // 移動　十字キー
        Vector2 dpad = gamepad.dpad.ReadValue();
        Direction = Vector3.ClampMagnitude(Direction + new Vector3(dpad.x, dpad.y, 0f), 1f);

        // 回転　スティック
        if (Mathf.Abs(stickX) > 0.1f)
            Rotate = -stickX * rotatePower;

        // 噴射　RT
        JetHeld |= gamepad.rightTrigger.isPressed;

        // ジャンプ　X
        JumpReleased |= gamepad.buttonSouth.wasReleasedThisFrame;
    }
}