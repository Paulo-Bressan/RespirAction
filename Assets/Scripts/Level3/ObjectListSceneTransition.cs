using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RespirAction.Level3
{
    /// <summary>
    /// Componente que monitora uma lista de GameObjects. Assim que todos da lista
    /// estiverem desativados, carrega uma nova cena informada pelo Inspector.
    /// Respeita o padrão de Responsabilidade Única (uma única função: transição por objetos).
    /// </summary>
    public class ObjectListSceneTransition : MonoBehaviour
    {
        [Header("Configuração de Gatilho")]
        [Tooltip("Lista de objetos que o script irá monitorar. Quando todos estiverem desativados, a transição começa.")]
        [SerializeField] private List<GameObject> _objectsToMonitor = new List<GameObject>();

        [Header("Configuração de Cena")]
        [Tooltip("Nome exato da próxima cena a ser carregada.")]
        [SerializeField] private string _nextSceneName;

        [Tooltip("Tempo de espera (em segundos) após os objetos sumirem antes de carregar a cena.")]
        [SerializeField] private float _delayBeforeTransition = 1f;

        private bool _isTransitioning = false;
        private Coroutine _monitorCoroutine;

        private void OnEnable()
        {
            StartMonitoring();
        }

        private void OnDisable()
        {
            if (_monitorCoroutine != null)
            {
                StopCoroutine(_monitorCoroutine);
                _monitorCoroutine = null;
            }
        }

        private void StartMonitoring()
        {
            if (_monitorCoroutine != null) StopCoroutine(_monitorCoroutine);
            _monitorCoroutine = StartCoroutine(MonitorObjectsRoutine());
        }

        private IEnumerator MonitorObjectsRoutine()
        {
            // Verificação de segurança
            if (_objectsToMonitor == null || _objectsToMonitor.Count == 0)
            {
                Debug.LogWarning("[ObjectListSceneTransition] A lista de objetos para monitorar está vazia. O script não fará nada.", this);
                yield break;
            }

            if (string.IsNullOrEmpty(_nextSceneName))
            {
                Debug.LogWarning("[ObjectListSceneTransition] O nome da próxima cena está vazio! Verifique o Inspector.", this);
                yield break;
            }

            while (!_isTransitioning)
            {
                bool allDisabled = true;
                
                // Checa o status de todos os objetos da lista
                for (int i = 0; i < _objectsToMonitor.Count; i++)
                {
                    // Ignoramos refs nulas na lista, mas paramos se houver pelo menos um ativo
                    if (_objectsToMonitor[i] != null && _objectsToMonitor[i].activeInHierarchy)
                    {
                        allDisabled = false;
                        break; 
                    }
                }

                // Se todos (os não nulos) estiverem desativados...
                if (allDisabled)
                {
                    _isTransitioning = true;
                    StartCoroutine(TransitionRoutine());
                }

                // Espera um pouco antes de checar de novo (Otimização de performance: melhor que usar o Update todo frame)
                yield return new WaitForSeconds(0.2f);
            }
        }

        private IEnumerator TransitionRoutine()
        {
            // Delay opcional para dar tempo do jogador processar o último item
            if (_delayBeforeTransition > 0f)
            {
                yield return new WaitForSeconds(_delayBeforeTransition);
            }

            // Carrega a cena
            SceneManager.LoadScene(_nextSceneName);
        }
    }
}
