using UnityEngine;
using BasicCommon;
using System.Collections;

namespace GoodFish
{
	public class Hook : Actor 
	{
		public GameObject chain;

		protected override void Update()
		{
			float chainLength = -Mathf.Min(-1, this.transform.position.y);
			chain.transform.localScale = new Vector3(1f, 1f, chainLength/2f);
			chain.transform.localPosition = new Vector3(0f, 0f, chainLength/2f);
			base.Update();
		}
	}

}