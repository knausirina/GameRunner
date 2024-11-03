using Level;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerController : MonoBehaviour, IPlayerController
	{
		[SerializeField] private Transform _colliderTrasform;

		private const float Speed = 5.0f;
		private const float LeneOffset = 1.0f;
		private const float AnimationSpeedRation = 0.6f;
		private const float SpeedRation = 0.05986977f;

		private int DeadAnimationHash = Animator.StringToHash("Dead");
		private int RunAnimationHash = Animator.StringToHash("runStart");
		private int MovingAnimationHash = Animator.StringToHash("Moving");
		private int JumingAnimationHash = Animator.StringToHash("Jumping");
		private int JumginSpeedHash = Animator.StringToHash("JumpSpeed");
		private int HitAnimationHash = Animator.StringToHash("Hit");

		private Animator _animator;

		private int _currentRoad = 1;
		private bool _isPlaying = false;
		private Vector3 _initPosition;

		private bool _jumping = false;
		private float _jumpLength = 3f;
		private float _jumpHeight = 1.2f;
		private float _jumpStart;
		private float _distance = 0;

		private void Awake()
		{
			_animator = GetComponentInChildren<Animator>();
			_initPosition = transform.position;
		}

		public void Start()
		{
			gameObject.SetActive(true);

			_currentRoad = 1;

			transform.position = _initPosition;
			_colliderTrasform.localPosition = Vector3.zero;

			_animator.enabled = true;
			_animator.SetBool(DeadAnimationHash, false);
			_animator.Play(RunAnimationHash);
			_animator.SetBool(MovingAnimationHash, true);

			_isPlaying = true;
		}

		public void Stop()
		{
            gameObject.SetActive(false);
            _isPlaying = false;

			_animator.enabled = false;
		}

		public void Die()
		{
			_animator.SetTrigger(HitAnimationHash);
			_animator.SetBool(DeadAnimationHash, true);

            _isPlaying = false;
           
        }

		public void Hit()
		{
			_animator.SetBool(MovingAnimationHash, false);
			_animator.SetTrigger(HitAnimationHash);

            _isPlaying = false;
		}

		public void Pause()
		{
			_isPlaying = false;
		}

		public void Resume()
		{
			_isPlaying = true;
		}

		private void Update()
		{
			if (!_isPlaying)
				return;

			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				ChangeRoad(MovementDirection.Left);
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				ChangeRoad(MovementDirection.Right);
			}
			else if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				Jump();
			}

			float verticalPosition = 0;
			if (_jumping)
			{
                var correctJumpLength = _jumpLength * (1.0f + SpeedRation);
                var ratio = (_distance - _jumpStart) / correctJumpLength;
				if (ratio >= 1.0f)
				{
					_jumping = false;
					_animator.SetBool(JumingAnimationHash, false);
				}
				else
				{
					verticalPosition = Mathf.Sin(ratio * Mathf.PI) * _jumpHeight;
				}
			}
		
			var oldPos = transform.position;
			transform.position = Vector3.MoveTowards(oldPos, new Vector3(oldPos.x, oldPos.y, oldPos.z + 10), Speed * Time.deltaTime);

			_colliderTrasform.localPosition = Vector3.MoveTowards(_colliderTrasform.localPosition, new Vector3((_currentRoad - 1) * LeneOffset, verticalPosition, _colliderTrasform.localPosition.z), Speed * Time.deltaTime);

			_distance += Speed * Time.deltaTime;
		}

		private void Jump()
		{
			if (_jumping)
			{
				return;
			}

			var correctJumpLength = _jumpLength * (1.0f + SpeedRation);
			_jumpStart = _distance;

			_animator.SetFloat(JumginSpeedHash, AnimationSpeedRation * (Speed / correctJumpLength));
			_animator.SetBool(JumingAnimationHash, true);
			_jumping = true;
		}

		private void ChangeRoad(MovementDirection direction)
		{
			int road = _currentRoad + (direction == MovementDirection.Left ? -1 : 0) + (direction == MovementDirection.Right ? 1 : 0);
			if (road < 0 || road > 2)
			{
				return;
			}

			_currentRoad = road;
		}
	}
}