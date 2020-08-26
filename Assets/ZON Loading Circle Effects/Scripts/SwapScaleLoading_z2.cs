using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwapScaleLoading_z2 : MonoBehaviour
{
	public RectTransform _frontIcon;
    public RectTransform _backIcon;

	public Text _loadingText;

	public float _duration = 1;
    public float _delayTime = 0;

	public Color[] _transitiveColors;//Length > 1
    int _fromColorIndex;
    int _toColorIndex = 0;
	Image[] _graphicList;
    float _startTime;

    void Start()
	{        
        _graphicList = _frontIcon.GetComponentsInChildren<Image>(true);

		if (_transitiveColors.Length > 1)
		{
			SetColor(_transitiveColors[0]);
		}

        Reset();

       	_backIcon.localScale = Vector3.zero;
    }

    void SwapIcon(){
        RectTransform temp = _frontIcon;
        _frontIcon = _backIcon;
        _backIcon = temp;

        _backIcon.SetAsFirstSibling();
        
		_graphicList = _frontIcon.GetComponentsInChildren<Image>(true);
    }
     
    void Reset(){
        SwapIcon();

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

        _frontIcon.GetComponent<RectTransform>().localScale = Vector3.zero;
        _backIcon.GetComponent<RectTransform>().localScale = Vector3.one;
        _startTime = Time.time;    	
    }

    void SetColor(Color toColor){
        if(_loadingText != null){
            _loadingText.color = toColor;
        }

		for(int i = 0; i < _graphicList.Length; i++)
		{
			_graphicList[i].color = toColor;
		}
    }

	void Update(){
        float currentTime = Time.time - _startTime;
        if (currentTime <= _duration) {
			float mainIconScale = SimpleTween.Linear(currentTime, 0, 1, _duration);
			_frontIcon.localScale = new Vector3 (mainIconScale, mainIconScale, mainIconScale);

            if (_transitiveColors.Length > 1)
            {
                Color toColor = SimpleTween.Linear(currentTime, _transitiveColors[_fromColorIndex], _transitiveColors[_toColorIndex], _duration);
                SetColor(toColor);
            }
		} else if (currentTime > _duration + _delayTime) {
            Reset();
		}
	}
}