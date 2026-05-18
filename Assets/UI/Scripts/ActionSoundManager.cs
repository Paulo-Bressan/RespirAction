using UnityEngine;

namespace RespirAction.Audio
{
    public class ActionSoundManager : MonoBehaviour
    {
        #region Constants
        private const string LOG_PREFIX = "[ActionSoundManager] ";
        #endregion

        #region Serialized Fields
        [Header("Configuração de Referências")]
        [Tooltip("Referência ao script de movimentação. Se vazio, tentará achar na cena.")]
        [SerializeField] private PlayerMovement _playerMovement;
        
        [Tooltip("AudioSource para sons rápidos (SFX). Será criado se vazio.")]
        [SerializeField] private AudioSource _sfxSource;
        
        [Tooltip("AudioSource para sons em loop. Será criado se vazio.")]
        [SerializeField] private AudioSource _loopSource;

        [Header("Sons de Ação")]
        [SerializeField] private AudioClip _interactionSound;
        [SerializeField] private AudioClip _movementSound;
        [SerializeField] private AudioClip _jumpSound;
        [SerializeField] private AudioClip _fallSound;
        #endregion

        #region Private Fields
        private bool _isInitialized;
        
        // Variáveis para detectar apenas a mudança de estado (trigger)
        private bool _wasMoving;
        private bool _wasInteracting;
        private bool _wasJumping;
        private bool _wasFalling;
        #endregion

        #region Public Properties
        public bool IsInitialized => _isInitialized;
        #endregion

        #region Unity Lifecycle
        private void Awake() => InitializeReferences();
        private void Start() => Initialize();
        private void Update() => HandleSounds();
        #endregion

        #region Private Methods
        private void InitializeReferences()
        {
            // Auto-associa ou cria AudioSources se não existirem
            if (_sfxSource == null) 
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
            }
            
            if (_loopSource == null) 
            {
                _loopSource = gameObject.AddComponent<AudioSource>();
                _loopSource.loop = true;
                _loopSource.playOnAwake = false;
            }
        }

        private void Initialize()
        {
            if (_isInitialized) return;

            if (_playerMovement == null)
            {
                _playerMovement = FindFirstObjectByType<PlayerMovement>();
                if (_playerMovement == null)
                {
                    Debug.LogWarning(LOG_PREFIX + "PlayerMovement não encontrado na cena!", this);
                    return;
                }
            }

            _isInitialized = true;
        }

        private void HandleSounds()
        {
            if (!_isInitialized || _playerMovement == null) return;

            bool isMoving = _playerMovement.IsMoving;
            bool isInteracting = _playerMovement.IsInteracting;
            bool isJumping = _playerMovement.IsJumping;
            bool isFalling = _playerMovement.IsFalling;
            bool isGrounded = _playerMovement.IsGrounded;

            // 1. Som de Movimento (Loop)
            // Toca apenas se estiver se movendo, no chão e não interagindo.
            if (isMoving && isGrounded && !isInteracting)
            {
                if (!_loopSource.isPlaying && _movementSound != null)
                {
                    _loopSource.clip = _movementSound;
                    _loopSource.Play();
                }
            }
            else
            {
                if (_loopSource.isPlaying && _loopSource.clip == _movementSound)
                {
                    _loopSource.Stop();
                }
            }

            // 2. Som de Interação (Trigger)
            if (isInteracting && !_wasInteracting)
            {
                PlaySFX(_interactionSound);
            }

            // 3. Som de Pulo (Trigger)
            if (isJumping && !_wasJumping)
            {
                PlaySFX(_jumpSound);
            }

            // 4. Som de Queda (Trigger)
            if (isFalling && !_wasFalling)
            {
                PlaySFX(_fallSound); 
            }

            // Atualização de estados anteriores para detecção no próximo frame
            _wasMoving = isMoving;
            _wasInteracting = isInteracting;
            _wasJumping = isJumping;
            _wasFalling = isFalling;
        }

        private void PlaySFX(AudioClip clip)
        {
            if (clip != null && _sfxSource != null)
            {
                _sfxSource.PlayOneShot(clip);
            }
        }
        #endregion
        
        #region Debug
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void Log(string msg) => Debug.Log(LOG_PREFIX + msg, this);
        #endregion
    }
}
