using UnityEngine;
using System.Collections;

namespace GoodFish
{
	public class SpriteBoss : MonoBehaviour {

		public static SpriteBoss Instance;

		public GameObject HUDPrototype;

		public Texture NoHealth;
		public Texture LowHealth;
		public Texture MedHealth;
		public Texture FullHealth;

		public Texture NoFood;
		public Texture LowFood;
		public Texture MedFood;
		public Texture FullFood;	

		public Texture GoFood;
		public Texture GoFish;
		public Texture GoFear;
		public Texture GoTired;	
		public Texture GoHappy;	

		void Awake()
		{
			Instance = this;
		}	
	}
}