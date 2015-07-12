using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BasicCommon
{
	public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public enum AxisOption
		{
			// Options for which axes to use
			Both, // Use both
			OnlyHorizontal, // Only horizontal
			OnlyVertical // Only vertical
		}

		public int MovementRange = 100;
		public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
		public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
		public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

		Vector3 m_StartPos;
		bool m_UseX; // Toggle for using the x axis
		bool m_UseY; // Toggle for using the Y axis
		float m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
		float m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input



		public UnityEngine.UI.Slider.SliderEvent onHorizontalChanged;
		public UnityEngine.UI.Slider.SliderEvent onVerticalChanged;

        void Start()
        {

            m_StartPos = transform.position;
			m_UseX = axesToUse != AxisOption.OnlyVertical;
			m_UseY = axesToUse != AxisOption.OnlyHorizontal;
        }

		void UpdateVirtualAxes(Vector3 value)
		{
			var delta = m_StartPos - value;
			delta.y = -delta.y;
			delta /= MovementRange;
			if (m_UseX)
			{
				onHorizontalChanged.Invoke( -delta.x );
			}

			if (m_UseY)
			{
				onVerticalChanged.Invoke( delta.y );
			}

			
		}


		public void OnDrag(PointerEventData data)
		{
			Vector3 newPos = Vector3.zero;

			if (m_UseX)
			{
				int delta = (int)(data.position.x - m_StartPos.x);
				delta = Mathf.Clamp(delta, - MovementRange, MovementRange);
				newPos.x = delta;
			}

			if (m_UseY)
			{
				int delta = (int)(data.position.y - m_StartPos.y);
				delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
				newPos.y = delta;
			}
			transform.position = new Vector3(m_StartPos.x + newPos.x, m_StartPos.y + newPos.y, m_StartPos.z + newPos.z);
			UpdateVirtualAxes(transform.position);
		}


		public void OnPointerUp(PointerEventData data)
		{
			transform.position = m_StartPos;
			UpdateVirtualAxes(m_StartPos);
		}

		public void OnPointerDown(PointerEventData data) { }
	}
}