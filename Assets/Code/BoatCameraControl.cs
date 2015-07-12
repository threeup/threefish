using UnityEngine;
using System.Collections;
using BasicCommon;
using DG.Tweening;

namespace GoodFish
{
	public class BoatCameraControl : ChaseCameraControl {

		public Boat boat;
		
		private bool isUnderwater = false;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () 
		{
			float deltaTime = Time.deltaTime;
			SetUnderwater( boat != null && boat.hook != null && boat.hook.IsAlive );
			sequenceLock.Tick(deltaTime);
		}

		public void SetUnderwater(bool val)
		{
			if( isUnderwater == val )
			{
				return;
			}

			isUnderwater = val;
			Vector3 pos = this.transform.position;
			if( val )
			{
				pos.y = -8;
			}
			else
			{
				pos.y = 3;
			}
			float duration = 0.15f;
			sequenceLock = new BasicTimer(duration, false);
			mySequence = DOTween.Sequence();
            mySequence.Append(transform.DOMove(pos, duration));
		}

		public void SetActiveFocus(bool val, Actor focalActor)
		{
			Boat focalBoat = focalActor as Boat;
			boat = focalBoat;
			if( val )
			{
				thisCamera.rect = focusRect;
				thisCamera.depth = 0;
			}
			else
			{
				thisCamera.rect = pipRect;
				thisCamera.depth = 1;
			}
		}
	}
}