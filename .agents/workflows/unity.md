---
description: usada para produzir código em c# para ser usado na unity
---

# Workflow de Geração de Código Unity com Antigravity

## 1. Contexto Inicial do Projeto

### 1.1 Informações Obrigatórias no Primeiro Prompt
```
[CONTEXTO DO PROJETO]
- Nome do Projeto: [nome]
- Gênero: [RPG/FPS/Platformer/Puzzle]
- Versão Unity: [ex: 2022.3 LTS]
- Pipeline: [URP/HDRP/Built-in]
- Arquitetura: [Singleton/State Machine/ECS/MVP]
- Dependências: [DOTween/UniRx/Zenject/Odin]

[ESTADO ATUAL]
- Scripts existentes: [listar]
- Sistemas implementados: [listar]
```

---

## 2. Estrutura de Pastas

```
Assets/Scripts/
├── Core/Managers/       # GameManager, SceneManager, AudioManager
├── Core/Events/         # Event Bus, ScriptableObject Events
├── Gameplay/Player/     # Controllers, Input, States
├── Gameplay/Enemies/    # AI, Behaviors, Spawning
├── Gameplay/Weapons/    # Armas e projéteis
├── UI/                  # Menus, HUD, Components
├── Data/                # ScriptableObjects, Enums, Structs
├── Camera/              # Controles de câmera
└── Utilities/           # Classes estáticas utilitárias
```

---

## 3. Padrões de Código Unity

### 3.1 Estrutura Base de MonoBehaviour
```csharp
using UnityEngine;

namespace [ProjectName].[Categoria]
{
    [RequireComponent(typeof([Componente]))]
    public class NomeDoScript : MonoBehaviour
    {
        #region Constants
        private const string LOG_PREFIX = "[NomeDoScript] ";
        #endregion

        #region Serialized Fields
        [Header("Configuration")]
        [SerializeField] private float _valorConfiguravel = 10f;
        
        [Header("References")]
        [SerializeField] private Transform _reference;
        #endregion

        #region Private Fields
        private bool _isInitialized;
        #endregion

        #region Public Properties
        public bool IsInitialized => _isInitialized;
        #endregion

        #region Unity Lifecycle
        private void Awake() => InitializeReferences();
        private void Start() => Initialize();
        private void OnEnable() => SubscribeEvents();
        private void OnDisable() => UnsubscribeEvents();
        private void OnDestroy() => Cleanup();
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        private void InitializeReferences()
        {
            if (_reference == null) _reference = transform;
        }

        private void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
        }

        private void SubscribeEvents() { /* EventManager.OnEvent += HandleEvent; */ }
        private void UnsubscribeEvents() { /* EventManager.OnEvent -= HandleEvent; */ }
        private void Cleanup() { /* Liberar recursos */ }
        #endregion

        #region Event Handlers
        #endregion

        #region Debug
        [Conditional("UNITY_EDITOR")]
        private void Log(string msg) => Debug.Log(LOG_PREFIX + msg, this);
        #endregion
    }
}
```

### 3.2 Singleton MonoBehaviour
```csharp
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new();
    private static bool _applicationIsQuitting;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting) return null;
            lock (_lock) { return _instance; }
        }
    }

    [SerializeField] private bool _dontDestroyOnLoad = true;

    protected virtual void Awake()
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (_dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this) _applicationIsQuitting = true;
    }
}
```

### 3.3 ScriptableObject de Configuração
```csharp
[CreateAssetMenu(fileName = "NewConfig", menuName = "Project/Config/NewConfig")]
public class ConfigData : ScriptableObject
{
    [SerializeField] private float _value;
    public float Value => _value;
}
```

---

## 4. Convenções de Nomenclatura

| Elemento | Convenção | Exemplo |
|----------|-----------|---------|
| Classes | PascalCase | `PlayerController` |
| Métodos | PascalCase | `GetPlayerPosition()` |
| Variáveis privadas | _camelCase | `_playerSpeed` |
| Propriedades públicas | PascalCase | `PlayerSpeed { get; }` |
| Constantes | SCREAMING_SNAKE | `MAX_HEALTH` |
| Eventos | On + PascalCase | `OnPlayerDeath` |
| Namespaces | Hierárquico | `ProjectName.Gameplay.Player` |

---

## 5. Template de Prompt Universal

```
[REQUISIÇÃO DE SCRIPT]

Nome: [NomeDoScript]
Categoria: [Core/Gameplay/UI/Data]
Herança: [MonoBehaviour/SingletonMonoBehaviour/ScriptableObject]

Requisitos Funcionais:
- [Listar funcionalidades principais]

Parâmetros Serializados:
- [Nome: tipo (padrão: valor)]

Componentes Obrigatórios:
- [ex: Rigidbody, NavMeshAgent, AudioSource]

Dependências:
- [Scripts que devem ser referenciados]

Eventos:
- Entrada: [eventos que escuta]
- Saída: [eventos que dispara]

Integração:
- [Animator/Cinemachine/Input System/UI]

Restrições de Performance:
- [Cache/Pooling/Coroutines]
```

---

## 6. Diretrizes de Performance

### 6.1 Obrigações
```csharp
// CACHE DE COMPONENTES (ERRADO → CORRETO)
// ERRADO: GetComponent<Rigidbody>().velocity = v;   // No Update
// CORRETO: private Rigidbody _rb; void Awake() => _rb = GetComponent<Rigidbody>();

// ANIMATOR HASHES
// ERRADO: animator.SetBool("IsRunning", true);
// CORRETO: private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

// TAGS
// ERRADO: if (other.tag == "Player")
// CORRETO: if (other.CompareTag("Player"))
```

### 6.2 Anti-GC
- Structs para dados pequenos frequentes
- `StringBuilder` para concatenação
- Evitar LINQ em Updates
- Cache de `GetComponentsInChildren`

### 6.3 Object Pooling
```csharp
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _initialSize = 10;
    private Queue<GameObject> _pool = new();

    private void Start()
    {
        for (int i = 0; i < _initialSize; i++)
        {
            var obj = Instantiate(_prefab, transform);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(_prefab);
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}
```

---

## 7. Sistema de Eventos

### 7.1 Event Bus Estático
```csharp
namespace ProjectName.Core.Events
{
    public static class GameEvents
    {
        // Declaração
        public static event Action<GameState> OnGameStateChanged;
        public static event Action<int> OnScoreChanged;
        public static event Action OnPlayerDeath;

        // Invocação
        public static void GameStateChanged(GameState state) => OnGameStateChanged?.Invoke(state);
        public static void ScoreChanged(int score) => OnScoreChanged?.Invoke(score);
        public static void PlayerDeath() => OnPlayerDeath?.Invoke();
    }
}
```

### 7.2 Uso
```csharp
// Subscrição (OnEnable)
private void OnEnable() => GameEvents.OnPlayerDeath += HandleDeath;

// Desubscrição (OnDisable)
private void OnDisable() => GameEvents.OnPlayerDeath -= HandleDeath;

// Disparo
GameEvents.ScoreChanged(100);
```

---

## 8. Checklist de Qualidade

Antes de aceitar o código gerado:

- [ ] Namespace hierárquico correto
- [ ] `SerializeField` para referências do Inspector
- [ ] `RequireComponent` quando há dependências
- [ ] Cache de componentes no Awake
- [ ] Eventos: OnEnable (subscribe) / OnDisable (unsubscribe)
- [ ] Null checks em referências críticas
- [ ] `#region` organizado
- [ ] Constantes para valores mágicos
- [ ] Propriedades para expor privadas
- [ ] Animator hashes cacheados
- [ ] `CompareTag` ao invés de `== "tag"`

---

## 9. Fluxo de Trabalho

```
1. ANÁLISE
   └── Categoria → Dependências → Padrões → Integrações

2. FORMATAÇÃO
   └── Template (seção 5) + Contexto (seção 1) + Performance (seção 6)

3. REVISÃO
   └── Checklist (seção 8) + Padrões (seção 3) + Nomenclatura (seção 4)

4. ITERAÇÃO
   └── Ajustes → Refinamentos → Atualizar contexto
```

---

## 10. Integrações Comuns

### 10.1 Input System (Novo)
```csharp
public class InputReader : ScriptableObject, InputActions.IGameplayActions
{
    public event Action<Vector2> OnMove;
    public event Action OnJump;

    private InputActions _actions;

    public void Enable()
    {
        _actions ??= new InputActions();
        _actions.Gameplay.SetCallbacks(this);
        _actions.Gameplay.Enable();
    }

    public void Disable() => _actions?.Gameplay.Disable();

    void InputActions.IGameplayActions.OnMove(InputAction.CallbackContext ctx)
        => OnMove?.Invoke(ctx.ReadValue<Vector2>());

    void InputActions.IGameplayActions.OnJump(InputAction.CallbackContext ctx)
    { if (ctx.performed) OnJump?.Invoke(); }
}
```

### 10.2 Animator Integration
```csharp
public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int JumpHash = Animator.StringToHash("Jump");

    public void SetSpeed(float speed) => _animator.SetFloat(SpeedHash, speed);
    public void TriggerJump() => _animator.SetTrigger(JumpHash);
}
```

### 10.3 Cinemachine
```csharp
public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _vcam;
    private CinemachineTransposer _transposer;

    private void Awake()
        => _transposer = _vcam.GetCinemachineComponent<CinemachineTransposer>();

    public void SetFollow(Transform target) => _vcam.Follow = target;
    public void SetOffset(Vector3 offset) => _transposer.m_FollowOffset = offset;
}
```

---

## 11. Tratamento de Erros

### 11.1 Null Safety
```csharp
// TryGetComponent Pattern
private void Awake()
{
    if (!TryGetComponent(out _rb))
    {
        Debug.LogError($"{LOG_PREFIX}Rigidbody missing!", this);
        enabled = false;
    }
}

// Guard Clause
private void Process(Transform target)
{
    if (target == null) return;
    // Lógica
}
```

### 11.2 Safe Coroutine
```csharp
private Coroutine _coroutine;

private void StartProcess()
{
    if (_coroutine != null) StopCoroutine(_coroutine);
    _coroutine = StartCoroutine(ProcessRoutine());
}

private IEnumerator ProcessRoutine()
{
    yield return null;
    _coroutine = null;
}
```

---

