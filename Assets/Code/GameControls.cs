using UnityEngine;
using System.Collections;

namespace GoodFish
{
	public class GameControls : MonoBehaviour {


		public GameObject btnStart;
		public GameObject btnFish;
		public GameObject btnBoat;
		public GameObject joyAll;
		public GameObject joyHorizontal;
		public GameObject joyVertical;
		public GameObject btnSwim;

		public void Awake()
		{
			joyAll.SetActive(false);
			joyHorizontal.SetActive(false);
			joyVertical.SetActive(false);
			btnSwim.SetActive(false);
			btnFish.SetActive(false);
			btnBoat.SetActive(false);
		}
		
		public void SetModeSurface(bool val)
		{
			Debug.Log("SetModeSurface"+val);
			if( val )
			{
				joyAll.SetActive(false);
				joyHorizontal.SetActive(true);
				joyVertical.SetActive(true);
				btnSwim.SetActive(false);
				btnFish.SetActive(true);
				btnBoat.SetActive(false);
			}
			else
			{
				joyAll.SetActive(true);
				joyHorizontal.SetActive(false);
				joyVertical.SetActive(false);
				btnSwim.SetActive(true);
				btnFish.SetActive(false);
				btnBoat.SetActive(true);
			}
		}

		public void StartGame()
		{
			btnStart.SetActive(false);
		}
	}
}