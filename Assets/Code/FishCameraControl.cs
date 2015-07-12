﻿using UnityEngine;
using System.Collections;
using BasicCommon;
using DG.Tweening;

namespace GoodFish
{
	public class FishCameraControl : ChaseCameraControl {

		public Fish fish;

		private bool isFacingPositive;


		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () 
		{
			float deltaTime = Time.deltaTime;
			SetFacingPositive( fish != null && fish.motor.MotorForward.z > 0);
			sequenceLock.Tick(deltaTime);
		}

		void LateUpdate()
		{
			if( target == null )
			{
				return;
			}
			if( !LockInput )
			{
				this.transform.position = DesiredCameraPosition(target.position);
			}
			Vector3 targetPosition = target.position;
			targetPosition.x = 0;
			this.transform.LookAt(targetPosition);
		}

		public void SetFacingPositive(bool val)
		{
			if( isFacingPositive == val )
			{
				return;
			}
			isFacingPositive = val;
			if( target == null )
			{
				return;
			}
			float duration = 0.5f;
			sequenceLock = new BasicTimer(duration, false);
			mySequence = DOTween.Sequence();
            mySequence.Append(transform.DOMove(DesiredCameraPosition(target.position), duration));
		}

		Vector3 DesiredCameraPosition(Vector3 targetPos)
		{
			Vector3 pos = targetPos;
			if( isFacingPositive )
			{
				pos.z -= 10;
			}
			else
			{
				pos.z += 10;
			}
			pos.x += 10;
			return pos;
		}


		public void SetActiveFocus(bool val, Actor focalActor)
		{
			Fish focalFish = focalActor as Fish;
			fish = focalFish;
			if( fish != null )
			{
				target = fish.transform;
			}
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