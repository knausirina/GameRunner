using UnityEngine;

namespace UI
{
	public class Views : MonoBehaviour
	{
		public StartView StartView;
		public GameView GameView;

		private void Awake()
		{
			ShowStart();
		}

		public void ShowStart()
		{
			StartView.Show();
			GameView.Hide();
		}

		public void ShowGame()
		{
			StartView.Hide();
			GameView.Show();
		}

		public void ShowDead()
		{
			GameView.ToggleDead(true);
		}
	}
}
