using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RespirAction.Level3
{
    public class ImmersiveSceneTransition : MonoBehaviour
    {
        #region Constants
        private const string LOG_PREFIX = "[ImmersiveSceneTransition] ";
        #endregion

        #region Serialized Fields
        [Header("Triggers")]
        [Tooltip("Lista de objetos que o script irá monitorar. Quando todos estiverem desativados, a transição começa.")]
        [SerializeField] private List<GameObject> _objectsToMonitor = new();

        [Header("Zoom Configuration")]
        [Tooltip("A câmera que realizará o zoom. Se deixado vazio, usará a Camera.main automaticamente.")]
        [SerializeField] private Camera _transitionCamera;
        
        [Tooltip("Ponto de referência (Transform) na cena atual para onde a câmera deve dar zoom.")]
        [SerializeField] private Transform _zoomTarget;
        
        [Tooltip("Tempo em segundos que leva para completar o efeito de zoom.")]
        [SerializeField] private float _zoomDuration = 2f;
        
        [Tooltip("Tamanho alvo da lente no final do zoom (afeta Orthographic Size no 2D ou Field Of View no 3D).")]
        [SerializeField] private float _targetZoomValue = 2f;
        
        [Tooltip("Curva de suavização da transição do zoom.")]
        [SerializeField] private AnimationCurve _zoomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Scene Configuration")]
        [Tooltip("Nome exato da próxima cena a ser carregada.")]
        [SerializeField] private string _nextSceneName;
        #endregion

        #region Private Fields
        private bool _isInitialized;
        private bool _isTransitioning;
        private Coroutine _monitorCoroutine;
        private Coroutine _transitionCoroutine;
        private float _initialZoomValue;
        private Vector3 _initialCameraPosition;
        #endregion

        #region Unity Lifecycle
        private void Awake() => InitializeReferences();
        private void Start() => Initialize();
        private void OnEnable() => StartMonitoring();
        private void OnDisable() => Cleanup();
        #endregion

        #region Private Methods
        private void InitializeReferences()
        {
            if (_transitionCamera == null)
            {
                _transitionCamera = Camera.main;
            }

            if (_transitionCamera != null)
            {
                _initialZoomValue = _transitionCamera.orthographic ? _transitionCamera.orthographicSize : _transitionCamera.fieldOfView;
                _initialCameraPosition = _transitionCamera.transform.position;
            }
            else
            {
                Log("Nenhuma câmera encontrada para a transição.");
            }
        }

        private void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
        }

        private void StartMonitoring()
        {
            if (_monitorCoroutine != null) StopCoroutine(_monitorCoroutine);
            _monitorCoroutine = StartCoroutine(MonitorObjectsRoutine());
        }

        private IEnumerator MonitorObjectsRoutine()
        {
            // Espera até que a inicialização esteja concluída
            yield return new WaitUntil(() => _isInitialized);

            if (_objectsToMonitor == null || _objectsToMonitor.Count == 0)
            {
                Log("Lista de objetos para monitorar está vazia.");
                yield break;
            }

            while (!_isTransitioning)
            {
                bool allDisabled = true;
                
                // Checa todos os objetos da lista
                for (int i = 0; i < _objectsToMonitor.Count; i++)
                {
                    if (_objectsToMonitor[i] != null && _objectsToMonitor[i].activeInHierarchy)
                    {
                        allDisabled = false;
                        break;
                    }
                }

                if (allDisabled)
                {
                    StartTransition();
                }

                // Verifica periodicamente (0.2s) em vez de todo frame para economizar recursos (Anti-GC)
                yield return new WaitForSeconds(0.2f);
            }
        }

        private void StartTransition()
        {
            if (_isTransitioning) return;
            _isTransitioning = true;
            
            if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = StartCoroutine(TransitionRoutine());
        }

        private IEnumerator TransitionRoutine()
        {
            if (_transitionCamera == null || _zoomTarget == null || string.IsNullOrEmpty(_nextSceneName))
            {
                Debug.LogWarning($"{LOG_PREFIX}Configuração incompleta! (Falta Camera, ZoomTarget ou NextSceneName). Carregando próxima cena diretamente.", this);
                LoadNextScene();
                yield break;
            }

            float elapsedTime = 0f;
            Vector3 targetPosition = _zoomTarget.position;
            
            // Mantém o Z inicial da câmera (offset) para não atravessar os colliders do jogo em 2D/3D
            targetPosition.z = _initialCameraPosition.z;

            while (elapsedTime < _zoomDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _zoomDuration);
                float curveValue = _zoomCurve.Evaluate(t);

                // Interpola a Posição da Câmera
                _transitionCamera.transform.position = Vector3.Lerp(_initialCameraPosition, targetPosition, curveValue);

                // Interpola o Zoom da Câmera (Lida com câmeras 2D e 3D de forma dinâmica)
                if (_transitionCamera.orthographic)
                {
                    _transitionCamera.orthographicSize = Mathf.Lerp(_initialZoomValue, _targetZoomValue, curveValue);
                }
                else
                {
                    _transitionCamera.fieldOfView = Mathf.Lerp(_initialZoomValue, _targetZoomValue, curveValue);
                }

                yield return null;
            }

            // Garante os valores finais absolutos para evitar problemas de arredondamento
            _transitionCamera.transform.position = targetPosition;
            if (_transitionCamera.orthographic) _transitionCamera.orthographicSize = _targetZoomValue;
            else _transitionCamera.fieldOfView = _targetZoomValue;

            LoadNextScene();
        }

        private void LoadNextScene()
        {
            if (!string.IsNullOrEmpty(_nextSceneName))
            {
                SceneManager.LoadScene(_nextSceneName);
            }
            else
            {
                Debug.LogError($"{LOG_PREFIX}Nome da próxima cena está vazio!", this);
            }
        }

        private void Cleanup()
        {
            if (_monitorCoroutine != null)
            {
                StopCoroutine(_monitorCoroutine);
                _monitorCoroutine = null;
            }
            
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = null;
            }
        }
        #endregion

        #region Debug
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void Log(string msg) => Debug.Log(LOG_PREFIX + msg, this);
        #endregion
    }
}
