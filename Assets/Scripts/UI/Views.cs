using UnityEngine;
using System.Threading.Tasks;

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
			GameView.gameObject.SetActive(false);
			StartView.gameObject.SetActive(true);
		}

		public void ShowGame()
		{
			StartView.gameObject.SetActive(false);
			GameView.gameObject.SetActive(true);
			ToggleDead(false);
		}

		public void ToggleDead(bool isShow)
		{
			GameView.ToggleDead(isShow);
		}
	}
}
