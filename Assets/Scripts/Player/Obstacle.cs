using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	public class Obstacle : MonoBehaviour
	{
		private Animation _animation;

		public void PlayAnimation()
		{
			if (_animation == null)
			{
				_animation = GetComponentInChildren<Animation>();
			}
			if (_animation == null)
			{
				return;
			}
			_animation.Play();
		}
	}
}
