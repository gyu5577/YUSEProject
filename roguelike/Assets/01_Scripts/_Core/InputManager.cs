using System;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    #region Events (컨벤션 1-3)
    /// <summary>
    /// (S1, B-1.a) 이동 벡터 값이 변경될 때마다 호출됩니다.
    /// </summary>
    public event Action<Vector2> OnMovementInput;
    
    /// <summary>
    /// (S1, A-3.a) 일시정지 키(ESC)가 눌렸을 때 호출됩니다.
    /// </summary>
    public event Action OnPausePressed;
    
    /// <summary>
    /// 아이템 슬롯(1, 2, 3) 키가 눌렸을 때 호출됩니다.
    /// </summary>
    public event Action<int> GetItemUseInput;
    #endregion

    #region Private Consts
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const string JUMP = "Jump";
    private const string MOUSE_X = "Mouse X";
    private const string MOUSE_Y = "Mouse Y";
    #endregion

    #region Serialized Fields
    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;
    [SerializeField] private float mouseXInput;
    [SerializeField] private float mouseYInput;
    [SerializeField] private bool jumpInput;
    [SerializeField] private bool pauseInput;
    [SerializeField] private int useItemInput;
    #endregion

    #region Properties
    public float HorizontalInputValue => horizontalInput;
    public float VerticalInputValue => verticalInput;
    public float MouseXValue => mouseXInput;
    public float MouseYValue => mouseYInput;
    public bool JumpTriggered => jumpInput;
    public bool PausePressed => pauseInput;
    public int UseItemTriggered => useItemInput;
    #endregion

    #region Unity LifeCycle
    private void Awake()
    {
        Init();
    }

    private void Start()
    {
    }

    private void Update()
    {
        ProcessInput();
    }
    #endregion

    #region Public Methods
    public void Init()
    {
        horizontalInput = 0f;
        verticalInput = 0f;
        mouseXInput = 0f;
        mouseYInput = 0f;
        pauseInput = false;
        useItemInput = 0;
    }
    
    public void ProcessInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        jumpInput = Input.GetButtonDown(JUMP);
        mouseXInput = Input.GetAxis(MOUSE_X);
        mouseYInput = Input.GetAxis(MOUSE_Y);
        pauseInput = Input.GetKeyDown(KeyCode.Escape);
       
        // Item키 입력 상태 저장
        if (Input.GetKeyDown(KeyCode.Alpha1))
            useItemInput = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            useItemInput = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            useItemInput = 3;
        else
            useItemInput = 0;
        // --- 이벤트 방송 ---
        
        // 1. 이동 방송 (매 프레임)
        OnMovementInput?.Invoke(new Vector2(horizontalInput, verticalInput));
        
        // 2. 일시정지 방송 (키가 눌렸을 때만)
        if (pauseInput)
        {
            OnPausePressed?.Invoke();
        }
        // 3. ---------------------------
        if (useItemInput != 0)
        {
            GetItemUseInput?.Invoke(useItemInput);
        }
    }

    public bool IsKeyPressed(KeyCode keyCode) => Input.GetKey(keyCode);
    public bool IsKeyDown(KeyCode keyCode) => Input.GetKeyDown(keyCode);
    public Vector2 GetMouseCoord() => Input.mousePosition;
    public bool IsMouseButtonPressed(int button) => Input.GetMouseButtonDown(button);
    public bool IsMouseButtonDown(int button) => Input.GetMouseButtonDown(button);
    #endregion
}