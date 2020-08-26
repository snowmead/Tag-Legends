using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DotRunningLoading_z1 : MonoBehaviour
{
	public DotRunningLoading_z1_Element _mainIcon;

    public int _elementCount = 8;

    public Text _loadingText;

    public float _minScale = 0;
    public float _maxScale = 5;

    public float _duration = 1;

    public Color[] _transitiveColors;//Length > 1

    DotRunningLoading_z1_Element[] _dotArray;

	void Start(){
		if(_elementCount < 2){
			return;
		}

		float oneStepDuration = _duration - _duration/_elementCount;
		float onsStepDelay = oneStepDuration/(_elementCount - 1);
		_mainIcon.StartAnimation(oneStepDuration, _minScale, _maxScale, 0, _transitiveColors, _loadingText);

        _dotArray = new DotRunningLoading_z1_Element[_elementCount];
		_dotArray[0] = _mainIcon;

        for (int i = 1; i < _elementCount; i ++){
			GameObject newItem = Instantiate(_mainIcon.gameObject);
			newItem.transform.SetParent(_mainIcon.transform.parent);
			newItem.transform.position = _mainIcon.transform.position;
            newItem.transform.localScale = _mainIcon.transform.localScale;
            newItem.transform.localEulerAngles = new Vector3(0, 0, -(360.0f/_elementCount) * i);
            DotRunningLoading_z1_Element newItemComponent = newItem.GetComponent<DotRunningLoading_z1_Element>();
			newItemComponent.StartAnimation(oneStepDuration, _minScale, _maxScale, onsStepDelay * i, _transitiveColors);

			_dotArray[i] = newItemComponent;
		}
	}

	void Update(){
		for(int i = 0; i < _dotArray.Length; i++){
			_dotArray[i].UpdateAnimation();
		}
	}
}