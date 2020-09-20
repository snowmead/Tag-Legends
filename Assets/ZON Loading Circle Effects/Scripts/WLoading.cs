using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WLoading : MonoBehaviour
{
    public Transform _firstElement;

    public WLoading_Element _elementTemplate;    
    public int _elementCount = 6;

    public Text _loadingText;

    public float _runAngle = -280;

    public float _runDuration = 0.5f;
    public float _walkDuration = 1.5f;

    public float _elementFollowSpeed = 7f;
	public float _angleBetween2Elements = 0;//Leave 0 to get default

    public Color[] _transitiveColors;//Length > 1
    Image[] _graphicList;
    int _fromColorIndex;
    int _toColorIndex = 0;

	WLoading_Element[] _elements;

    float _finishRunAngle = 0;
    float _startTime;

    void Start()
    {
        _graphicList = GetComponentsInChildren<Image>(true);

        if (_transitiveColors.Length > 1)
        {
            SetColor(_transitiveColors[0]);
        }

        if(_elementCount > 1){
			_elements = new WLoading_Element[_elementCount - 1];			
        }

		float signOfAngle = Mathf.Sign (_runAngle);

		float angleBetween2Elements;
		if (_angleBetween2Elements > 0) {
			angleBetween2Elements = _angleBetween2Elements;
		} else {
			angleBetween2Elements = (360 - _runAngle * signOfAngle)/(_elementCount);
		}

        Transform tempTarget = _firstElement;

		for(int i = 1; i < _elementCount; i ++){
			GameObject newItem = Instantiate(_elementTemplate.gameObject);
			newItem.transform.SetParent(_elementTemplate.transform.parent);
			newItem.transform.position = _elementTemplate.transform.position;
            newItem.transform.localScale = _elementTemplate.transform.localScale;
            newItem.transform.localEulerAngles = new Vector3(0, 0, (-signOfAngle) * angleBetween2Elements * i);
            WLoading_Element newItemComponent = newItem.GetComponent<WLoading_Element>();
			newItemComponent.ResetAnimation(tempTarget, (-signOfAngle) * angleBetween2Elements, _elementFollowSpeed);

	        if (_transitiveColors.Length > 1)
	        {
	            newItemComponent.SetColor(_transitiveColors[0]);
	        }

			_elements[i - 1] = newItemComponent;
			tempTarget = newItemComponent.transform;
		}

		_elementTemplate.gameObject.SetActive(false);

        Reset();
    }

    void SetColor(Color toColor)
    {
        if (_loadingText != null)
        {
            _loadingText.color = toColor;
        }

        for(int i = 0; i < _graphicList.Length; i++)
        {
            _graphicList[i].color = toColor;
        }
    }

    void Reset()
    {
        //Get transitive color index
        if (_transitiveColors.Length > 1)
        {
            _fromColorIndex = _toColorIndex;
            _toColorIndex++;

            if (_toColorIndex >= _transitiveColors.Length)
            {
                _toColorIndex = 0;
            }
        }

        _startTime = Time.time;
    }

    void Update(){
		float signOfAngle = Mathf.Sign (_runAngle);

        float currentTime = Time.time - _startTime;

        if(currentTime > _runDuration + _walkDuration)
        {
            Reset();
            currentTime = 0;
        }        

        if (currentTime <= _runDuration)
        {
			_finishRunAngle = SimpleTween.EaseOutQuat (currentTime, 0, _runAngle, _runDuration);			
            Vector3 angle = _firstElement.transform.localEulerAngles;
            angle.z = _finishRunAngle;
			_firstElement.transform.localEulerAngles = angle;

            if (_transitiveColors.Length > 1)
            {
                Color toColor = SimpleTween.Linear(currentTime, _transitiveColors[_fromColorIndex], _transitiveColors[_toColorIndex], _runDuration);
                SetColor(toColor);
            }
        }
        else{
        	currentTime -= _runDuration;

            if(currentTime < _walkDuration){				
                _firstElement.localEulerAngles = new Vector3(0, 0, SimpleTween.Linear(currentTime, _finishRunAngle, signOfAngle * 360, _walkDuration));
            } 
        }

        for (int i = 0; i < _elements.Length; i++) {
            if (_transitiveColors.Length > 1)
            {
            	_elements[i].SetTargetColor(_transitiveColors[_toColorIndex]);	
            }

            _elements[i].UpdateAnimation();
        }        
    }
}