using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PuffLoading_Element : MonoBehaviour
{
	float _startTime;

	float _appearDuration;
	float _disappearDuration;

	float _minScale;
	float _maxScale;

	bool _isSwicthToDisapear = false;

	Image[] _graphicList;

	public delegate void OnFinishDisappear();
	public OnFinishDisappear onFinishDisappear;

	public void StartAnimation(float appearDuration, float disappearDuration, float minScale, float maxScale){
		_graphicList = GetComponentsInChildren<Image>(true);

		_isSwicthToDisapear = false;

		GetComponent<RectTransform> ().localScale = new Vector3 (minScale, minScale, minScale);

		Color mainIconColor = GetComponent<Image> ().color;
		mainIconColor.a = 0;
		GetComponent<Image> ().color = mainIconColor;

		_appearDuration = appearDuration;
		_disappearDuration = disappearDuration;

		_minScale = minScale;
		_maxScale = maxScale;

		_startTime = Time.time;
	}

	void Update(){
		float currentTime = Time.time - _startTime;
		if (currentTime <= _appearDuration + _disappearDuration) {
			float mainIconScale = SimpleTween.EaseOutQuat (currentTime, _minScale, _maxScale, _appearDuration + _disappearDuration);
			GetComponent<RectTransform> ().localScale = new Vector3 (mainIconScale, mainIconScale, mainIconScale);

			Color mainIconColor = GetComponent<Image> ().color;
			if (currentTime <= _appearDuration) {
				mainIconColor.a = SimpleTween.EaseOutQuat (currentTime, 0, 1, _appearDuration);
			} else {
				if (!_isSwicthToDisapear) {
					_isSwicthToDisapear = true;
					if (onFinishDisappear != null) {
						onFinishDisappear ();
					}
				}
				currentTime -= _appearDuration;
				mainIconColor.a = SimpleTween.EaseOutQuat (currentTime, 1, 0, _disappearDuration);
			}

			GetComponent<Image> ().color = mainIconColor;
		} else {
			Destroy(gameObject);
		}
	}

	public void SetColor(Color toColor){
		toColor.a = 0;

		for(int i = 0; i < _graphicList.Length; i++)
		{
			_graphicList[i].color = toColor;
		}
	}
}