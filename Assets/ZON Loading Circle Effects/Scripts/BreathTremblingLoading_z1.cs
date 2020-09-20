using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathTremblingLoading_z1 : MonoBehaviour
{
	public Image _frontIcon;
    public Image _backIcon;

    public Text _loadingText;

    public float _minScale = 0.8f;
    public float _maxScale = 1;

	public float _expiratoryDuration = 0.5f;
    public float _inhaleDuration = 0.8f;

    public Color[] _transitiveColors;//Length > 1
    Image[] _graphicList;
    int _fromColorIndex;
    int _toColorIndex = 0;

    float _startTime;    
    float _backIconStartAlpha;

    void Start()
    {
        _graphicList = GetComponentsInChildren<Image>(true);
        _backIconStartAlpha = _backIcon.color.a;

        if (_transitiveColors.Length > 1)
        {
            SetColor(_transitiveColors[0]);
        }

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
            if(_graphicList[i] == _backIcon)
            {
                Color backIconColor = toColor;
                backIconColor.a = _backIcon.color.a;
                _graphicList[i].color = backIconColor;
            }
            else
            {
                _graphicList[i].color = toColor;
            }
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

        Color mainIconColor = _backIcon.color;
        mainIconColor.a = _backIconStartAlpha;
        _backIcon.color = mainIconColor;

        _backIcon.enabled = true;
        _backIcon.transform.localScale = new Vector3 (_maxScale, _maxScale, _maxScale);
        _frontIcon.transform.localScale = new Vector3 (_maxScale, _maxScale, _maxScale);
        _startTime = Time.time;
    }

	void Update(){
        float currentTime = Time.time - _startTime;
        if (currentTime <= _expiratoryDuration + _inhaleDuration) {            
            float mainIconScale;
            if(currentTime <= _expiratoryDuration){
                mainIconScale = SimpleTween.EaseOutQuat(currentTime, _maxScale, _minScale, _expiratoryDuration);

                Color mainIconColor = _backIcon.color;
                mainIconColor.a = SimpleTween.EaseOutQuat(currentTime, _backIconStartAlpha, 0, _expiratoryDuration);
                _backIcon.color = mainIconColor;
            }
            else{
                _backIcon.enabled = false;
                mainIconScale = SimpleTween.Linear(currentTime - _expiratoryDuration, _minScale, _maxScale, _inhaleDuration);

                if (_transitiveColors.Length > 1)
                {
                    Color toColor = SimpleTween.Linear(currentTime - _expiratoryDuration, _transitiveColors[_fromColorIndex], _transitiveColors[_toColorIndex], _inhaleDuration);
                    SetColor(toColor);
                }
            }

			_frontIcon.transform.localScale = new Vector3 (mainIconScale, mainIconScale, mainIconScale);
		} else {
            Reset();
		}
	}

    void SwapIcon(){
        Image temp = _frontIcon;
        _frontIcon = _backIcon;
        _backIcon = temp;

        _backIcon.transform.SetAsFirstSibling();
    }
}