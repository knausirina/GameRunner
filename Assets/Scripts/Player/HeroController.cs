using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Level;
using Zenject;

namespace Player
{
	public class HeroController : MonoBehaviour, IHeroController
	{
		public Transform colliderTrasform;

		private const float SPEED = 5.0f;
		private const float LANE_OFFSET = 1.0f;
		private const float ANIMATION_SPEED_RATIO = 0.6f;
		private const float SPEED_RATIO = 0.05986977f;

		private int DEAD = Animator.StringToHash("Dead");
		private int RUN = Animator.StringToHash("runStart");
		private int MOVING = Animator.StringToHash("Moving");
		private int JUMPING = Animator.StringToHash("Jumping");
		private int JUMPINGSPEED = Animator.StringToHash("JumpSpeed");
		private int HIT = Animator.StringToHash("Hit");

		private Animator _animator;

		private int _currentRoad = 1;
		private bool _isPlaying = false;
		private Vector3 _initPosition;

		private bool _jumping = false;
		private float _jumpLength = 3f;
		private float _jumpHeight = 1.2f;
		private float _jumpStart;
		private float _distance = 0;

		[Inject]
		private LevelController _levelController;

		/*
		[Inject]
		private void Construct(LevelController levelController)
		{
			_levelController = levelController;
		}
		*/

		private void Awake()
		{
			_animator = GetComponentInChildren<Animator>();
			_initPosition = transform.position;
		}

		public void Run()
		{
			_levelController.SetPlayer(transform);

			_currentRoad = 1;

			transform.position = _initPosition;
			colliderTrasform.localPosition = Vector3.zero;

			_animator.enabled = true;
			_animator.SetBool(DEAD, false);
			_animator.Play(RUN);
			_animator.SetBool(MOVING, true);

			_isPlaying = true;

			_levelController.Build();
		}

		public void Stop()
		{
			_isPlaying = false;

			_animator.enabled = false;
			_levelController.Stop();
		}

		public void Die()
		{
			_isPlaying = false;
			_levelController.Stop();

			_animator.SetTrigger(HIT);
			_animator.SetBool(DEAD, true);
		}

		public void Hit()
		{
			_animator.SetBool(MOVING, false);
			_animator.SetTrigger(HIT);

			_levelController.Stop();
			_isPlaying = false;
		}

		public void Pause()
		{
			_isPlaying = false;
		}

		public void Resume()
		{
			_isPlaying = true;
			_levelController.Resume();
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
				float correctJumpLength = _jumpLength * (1.0f + SPEED_RATIO);
				float ratio = (_distance - _jumpStart) / correctJumpLength;
				if (ratio >= 1.0f)
				{
					_jumping = false;
					_animator.SetBool(JUMPING, false);
				}
				else
				{
					verticalPosition = Mathf.Sin(ratio * Mathf.PI) * _jumpHeight;
				}
			}
		
			var oldPos = transform.position;
			transform.position = Vector3.MoveTowards(oldPos, new Vector3(oldPos.x, oldPos.y, oldPos.z + 10), SPEED * Time.deltaTime);

			colliderTrasform.localPosition = Vector3.MoveTowards(colliderTrasform.localPosition, new Vector3((_currentRoad - 1) * LANE_OFFSET, verticalPosition, colliderTrasform.localPosition.z), SPEED * Time.deltaTime);

			_distance += SPEED * Time.deltaTime;
		}

		private void Jump()
		{
			if (_jumping)
			{
				return;
			}

			float correctJumpLength = _jumpLength * (1.0f + SPEED_RATIO);
			_jumpStart = _distance;

			_animator.SetFloat(JUMPINGSPEED, ANIMATION_SPEED_RATIO * (SPEED / correctJumpLength));
			_animator.SetBool(JUMPING, true);
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