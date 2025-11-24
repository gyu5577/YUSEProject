# 코딩 컨벤션 문서

팀원이 충돌 없이 효율적으로 협업하기 위해, **구조**와 **스타일**을 모두 포함하는 다음 규칙을 정의합니다.

이런 코딩 컨벤션 문서는 전부 외워서 사용하기가 어려울 수 있습니다. 그리고 외우더라도 실수할 확률이 높습니다. 이걸 지켰는지 일일이 확인하면서 신경쓰는 것도 비효율적입니다.

따라서, 이 문서를 **최대한 준수**하시길 부탁드리나, 코드 리뷰 과정에서 검토할테니 형식에 매몰되지는 않았으면 합니다.

---

## = 목차 =

* [1. 구조 및 협업 규칙](#1-구조-및-협업-규칙)
* [2. 명명 규칙](#2-명명-규칙)
* [3. 코드 스타일 및 구조 규칙](#3-코드-스타일-및-구조-규칙)
* [4. 스크립트 구조 통일](#4-스크립트-구조-통일)

---

## 1. 구조 및 협업 규칙

*가장 중요합니다. 스크립트 간의 충돌을 방지하고 협업 속도를 높이기 위한 규칙입니다.*

### 1-1. 의존성 관리 (DI): "찾지 말고, 주입 받기"

* 스크립트가 다른 스크립트를 찾아다니게 만들면 안 됩니다. (`FindObjectOfType` 금지!)

* **[원칙]** 스크립트 간의 연결(예: `PlayerManager`가 `InputManager`를 참조)은 **`[SerializeField]`를 사용한 인스펙터 주입**을 원칙으로 합니다.
    * 클래스 초기 기획 때는 직접 참조를 생각했지만, 이 방식이 성능, 안정성, 명확성에서 우월합니다.

    ```csharp
    // PlayerManager.cs (좋은 예 ✅)
    public class PlayerManager : MonoBehaviour
    {
        [Header("연결(Dependencies)")]
        [SerializeField]
        private InputManager inputManager; // <-- 여기에 인스펙터에서 끌어다 놓기
    
        [SerializeField]
        private HUDManager hudManager; // HUDManager도 이렇게 연결
    
        // ... (이하 생략) ...
    }
    ```

### 1-2. 싱글톤의 제한적 사용

* `GameManager`, `AudioManager`처럼 **"씬(Scene)을 넘나들며"** 게임 전체에서 유일해야 하는 핵심 관리자에게만 싱글톤(`Instance`) 사용을 허용합니다.
* `InputManager`, `PlayerManager`, `SpawnManager` 등 씬 내부에 종속된 매니저들은 싱글톤으로 만들지 않고 **`[SerializeField]`로 연결**합니다.

### 1-3. 이벤트 기반 통신: "알리지만, 누군지는 모르게"

* 서로 다른 묶음(Package) 간의 통신(예: `PlayerManager` -> `HUDManager`)은 **`event Action` (이벤트/방송)**을 사용합니다.
* **[나쁜 예 ❌]** `PlayerManager`가 `HUDManager`의 함수(`UpdateHpBar`)를 직접 호출하면, 두 클래스가 강하게 결합되어 유지보수가 어려워집니다.
* **[좋은 예 ✅]** `PlayerManager`는 HP가 변경되었음을 "방송"만 합니다. `HUDManager`가 이 방송을 "구독"합니다.
    * `PlayerManager`는 UI가 있는지 알 필요가 없습니다. (낮은 결합도)

    ```csharp
    // PlayerManager.cs (정보 제공자)
    public event Action<float, float> OnHpChanged;
    
    public void TakeDamage(float amount)
    {
        _currentHp -= amount;
        OnHpChanged?.Invoke(_currentHp, _maxHp); // "HP 바뀌었음!" 방송
    }
    
    // HUDManager.cs (정보 구독자)
    [SerializeField] private PlayerManager player;
    [SerializeField] private Slider hpSlider;

    void Start()
    {
        // "Player의 HP가 바뀌면, 내 UpdateHpBar 함수를 실행해줘"
        player.OnHpChanged += UpdateHpBar; 
    }

    private void UpdateHpBar(float current, float max)
    {
        hpSlider.value = current / max;
    }
    ```

### 1-4. `GetComponent`는 `Awake()`에서 사용

* 같은 게임 오브젝트(GameObject)에 붙어있는 컴포넌트(`Rigidbody`, `Animator` 등)를 가져올 때는 `Awake()`에서 미리 찾아 변수에 저장해 둡니다. `Update()`에서 매번 `GetComponent`를 호출하지 않습니다.

    ```csharp
    private Rigidbody2D _rb;

    private void Awake() 
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    ```

---

## 2. 명명 규칙

* **클래스명, 파일명:** `PascalCase` (예: `PlayerManager`, `SpawnManager`)
    * 우선 SDS에 명시된 이름을 따릅니다.
    * 스크립트 파일명과 클래스명은 반드시 일치해야 합니다.

* **Private/Protected 필드:** `_` + `camelCase`
    * `private int _count;`

* **`[SerializeField]` 필드:** `_` 없는 `camelCase`
    * `[SerializeField] private string name;`
    * `[SerializeField] private Slider hpSlider;`

* **Public 프로퍼티:** `PascalCase`
    * `public int Count { get; set; }`

* **매개변수, 로컬 변수:** `camelCase`
    * `public int GetValue(List<int> list) { }`
    * `private void Awake() { int count = 1; }`

* **메서드 (Public/Private):** `PascalCase`
    * `public void TakeDamage(float amount)`
    * `private void Die()`

* **이름 원칙:**
    * 역할이 잘 드러나도록 축약어(예: `_sr`) 대신 풀네임(예: `_spriteRenderer`)을 씁니다.
    * 변수는 명사형 (`_index`), 함수는 동사형 (`GetCount`)으로 짓습니다.

* **"매직 넘버" 금지:** 코드에 의미를 알 수 없는 숫자나 문자열을 직접 쓰지 않습니다.
    * **(수정 가능한 값):** `[SerializeField]`를 사용하여 인스펙터에서 관리합니다.
    * **(수정 불가능한 값):** `const` (상수)로 정의합니다.

    ```csharp
    [SerializeField]
    private float moveSpeed = 5.0f; // <-- 인스펙터에서 수정 가능
    
    private const string HORIZONTAL = "Horizontal"; // <-- 상수
    ```

---

## 3. 코드 스타일 및 구조 규칙

### 3-1. 줄 바꿈 규칙
* 키워드나 함수, 클래스를 정의하는 등 새 괄호(`{`)를 열 때는 줄바꿈을 합니다.

    ```csharp
    private void FuncMethod() {
        // NO!
    }

    private void FuncMethod()
    {
        // OK!
    }
    ```

### 3-2. 접근 제한자 명시
* `void Awake()` (X) -> `private void Awake()` (O)
* 모든 필드와 메서드에 `public`, `private`, `protected`를 명시합니다.

### 3-3. 필드와 프로퍼티
* `public int count;` (X) -> `public int Count { get; set; }` (O)
* 필드는 `private`으로, 외부 접근이 필요하면 `public` 프로퍼티로 관리합니다.

    ```csharp
    public float Hp {
        get => _hp;
        private set { // 프로퍼티는 퍼블릭이지만, 외부에서 Hp를 직접 수정(set)할 수 없도록 한 예시
            _hp = value;
            OnHpChanged?.Invoke(_hp, _maxHp);
        }
    }
    private float _hp;
    ```

### 3-4. 인터페이스
* 인터페이스의 이름은 형용사구로 정의하고, 앞에 `I` 접두사를 사용합니다.
* `public interface IAttackable { }`
* `public interface IDamageable { }`

### 3-5. 이벤트 명명
* 이벤트나 콜백의 경우, 어떤 상황에 관한 것인지 명시해줍니다.
* **이벤트 정의 (과거형):** `public event Action DoorOpened;`
* **발생 메서드 (On[Event]):** `private void OnDoorOpened() { DoorOpened?.Invoke(); }`

### 3-6. 주석
* 메서드의 주석은 `/// <summary>`를 이용합니다.
    ```csharp
    /// <summary>
    /// 플레이어에게 데미지를 적용하고, HP 변경 이벤트를 호출합니다.
    /// </summary>
    /// <param name="amount">받은 데미지 양</param>
    public void TakeDamage(float amount) { }
    ```

---

## 4. 스크립트 구조 통일

* `#region` 지시문을 통해 비슷한 속성의 코드들을 구분하여 가독성을 높입니다.
* 모든 스크립트는 가급적 다음 순서를 따릅니다.

```csharp
using System; // 1. Using 문
using UnityEngine;

/// <summary>
/// (2. 클래스 헤더)
/// </summary>
public class PlayerManager : MonoBehaviour
{
    #region Events
    // 3. 이벤트
    public event Action<float, float> OnHpChanged;
    #endregion

    #region Properties
    // 4. 프로퍼티
    public float Hp { get => _hp; }
    #endregion
    
    #region Serialized Fields
    // 5. [SerializeField] (인스펙터 변수)
    [Header("Dependencies")]
    [SerializeField] private InputManager inputManager;
    [Header("Stats")]
    [SerializeField] private PlayerStats stats;
    #endregion

    #region Private Fields
    // 6. Private 변수
    private float _hp;
    private Rigidbody2D _rb;
    #endregion

    #region Unity LifeCycle
    // 7. Unity 생명주기 메서드 (Awake, Start, Update, FixedUpdate)
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _hp = stats.Hp;
    }
    
    private void FixedUpdate()
    {
        // ...
    }
    #endregion

    #region Public Methods
    // 8. Public 메서드 (외부 호출용)
    public void TakeDamage(float amount)
    {
        // ...
    }
    #endregion

    #region Private Methods
    // 9. Private 메서드 (내부 로직용)
    private void Die()
    {
        // ...
    }
    #endregion
}
```