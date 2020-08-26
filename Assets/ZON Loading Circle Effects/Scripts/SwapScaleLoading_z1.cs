using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwapScaleLoading_z1 : MonoBehaviour
{
    public RectTransform _backIcon;
    public RectTransform _frontIcon;

	public Text _loadingText;

	public float _minScale = 0;
	public float _maxScale = 1;

    public float _duration = 1;
    
    public Color[] _transitiveColors;//Length > 1
	Image[] _graphicList;
    int _fromColorIndex;
    int _toColorIndex = 0;

    float _startTime;

    void Start()
    {
        Reset();
    }

    void SetColor(Color toColor){
		for(int i = 0; i < _graphicList.Length; i++)
		{
			_graphicList[i].color = toColor;
		}
    }

    void SwapIcon(){
        RectTransform temp = _frontIcon;
        _frontIcon = _backIcon;
        _backIcon = temp;

        _backIcon.SetAsFirstSibling();
        
		_graphicList = _frontIcon.GetComponentsInChildren<Image>(true);
    }

    void Reset() {
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

            SetColor(_transitiveColors[_toColorIndex]);
        }

        _frontIcon.localScale = Vector3.zero;
        _backIcon.localScale = Vector3.one;
        _startTime = Time.time;
    }

    void Update(){
        float currentTime = Time.time - _startTime;
        if (currentTime <= _duration) {
            float mainIconScale = SimpleTween.EaseOutQuat(currentTime, 0, 1, _duration);
			_frontIcon.localScale = new Vector3 (mainIconScale, mainIconScale, mainIconScale);

			mainIconScale = SimpleTween.EaseInQuat(currentTime, 1, 0, _duration);
			_backIcon.localScale = new Vector3 (mainIconScale, mainIconScale, mainIconScale);

            if (_loadingText != null)
            {
                if (_transitiveColors.Length > 1)
                {
                    Color toColor = SimpleTween.Linear(currentTime, _transitiveColors[_fromColorIndex], _transitiveColors[_toColorIndex], _duration);
                    _loadingText.color = toColor;
                }
            }

        } else {
            Reset();
        }
    }
}