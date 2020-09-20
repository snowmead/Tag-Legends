using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SwapScaleLoading_z4 : MonoBehaviour
{
	public RectTransform _iconContainer;
	public RectTransform _icon1;
	public RectTransform _icon2;

    public Text _loadingText;

	public float _minScale;
	public float _maxScale;

	public float _duration = 1;

    public Color[] _transitiveColors;//Length > 1
    Image[] _graphicList;
    int _fromColorIndex;
    int _toColorIndex = 0;

	float _startTime;

	void Start()    {
        _graphicList = GetComponentsInChildren<Image>(true);

        if (_transitiveColors.Length > 1){
            SetColor(_transitiveColors[0]);            
        }

		Reset();
	}

    void SetColor(Color toColor){
        if(_loadingText != null){
            _loadingText.color = toColor;
        }

        for (int i = 0; i < _graphicList.Length; i++)
        {
            _graphicList[i].color = toColor;
        }
    }

	void Reset(){	
        //Get transitive color index
        if(_transitiveColors.Length > 1){
            _fromColorIndex = _toColorIndex;
            _toColorIndex ++;

            if(_toColorIndex >= _transitiveColors.Length){
                _toColorIndex = 0;
            }
        }
        	
		_startTime = Time.time;
	}

	void Update(){
        float currentTime = Time.time - _startTime;

        if(currentTime > _duration){
        	Reset();
        	currentTime = 0;
        }

		Vector3 angle = _iconContainer.localEulerAngles;

		angle.z = SimpleTween.Linear(currentTime, 0, -360, _duration);
		_iconContainer.localEulerAngles = angle;

		float scale = SimpleTween.Linear(currentTime, _minScale, _maxScale * 2, _duration);

		if (scale > _maxScale) {
			scale =  _maxScale * 2 - scale;
		}

		_icon1.localScale = new Vector3 (scale, scale, scale);
		_icon2.localScale = new Vector3 (_maxScale - scale, _maxScale - scale, _maxScale - scale);		

		if(_transitiveColors.Length > 1){
            Color toColor = SimpleTween.Linear(currentTime, _transitiveColors[_fromColorIndex], _transitiveColors[_toColorIndex], _duration);
            SetColor(toColor);
        }
	}
}