using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotRunningLoading_z2 : MonoBehaviour
{
	public DotRunningLoading_z2_Element _mainIcon;

    public int _elementCount = 8;
    public float _minScale = 0;
    public float _maxScale = 5;

    public float _minAlpha = 0;
    public float _maxAlpha = 0.5f;
    
	public float _duration = 1;

    public Color[] _colors;

    DotRunningLoading_z2_Element[] _dotArray;

	void Start(){
		if(_elementCount < 2){
			return;
		}

		float oneStepDuration = _duration - _duration/_elementCount;
		float onsStepDelay = oneStepDuration/_elementCount;
		_mainIcon.StartAnimation(oneStepDuration, _minScale, _maxScale, _minAlpha, _maxAlpha, 0);
		if(_colors.Length > 0){
			Color startColor = _colors [0];
			startColor.a = 0;
			_mainIcon.SetColor(startColor);
		}

		_dotArray = new DotRunningLoading_z2_Element[_elementCount];
		_dotArray[0] = _mainIcon;

		for(int i = 1; i < _elementCount; i ++){
			GameObject newItem = Instantiate(_mainIcon.gameObject);
			newItem.transform.SetParent(_mainIcon.transform.parent);
			newItem.transform.position = _mainIcon.transform.position;
            newItem.transform.localScale = _mainIcon.transform.localScale;
            newItem.transform.localEulerAngles = new Vector3(0, 0, -(360.0f/_elementCount) * i);
            DotRunningLoading_z2_Element newItemComponent = newItem.GetComponent<DotRunningLoading_z2_Element>();
			newItemComponent.StartAnimation(oneStepDuration, _minScale, _maxScale, _minAlpha, _maxAlpha, onsStepDelay * i);

			if(i < _colors.Length){
				Color elementColor = _colors [i];
				elementColor.a = 0;
				newItemComponent.SetColor(elementColor);
			}
			_dotArray[i] = newItemComponent;
		}
	}

	void Update(){
		for(int i = 0; i < _dotArray.Length; i++){
			_dotArray[i].UpdateAnimation();
		}
	}
}