using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Handles tab and screen navigation
public class NavController : MonoBehaviour {

	private const float TAB_MULT_WIDTH = 1.2f;	// Width multiplier for selected tab
	private const float TAB_MULT_HEIGHT = 1.1f;	// Height multiplier for selected tab
	private const float ELASTICITY = 0.01f;		// Elasticity of screen lerping

	public RectTransform TabScreensParentRT;	// Reference to parent TabScreen transform set in Unity Inspector
	public RectTransform TabsParentRT;			// Reference to parent Tab transform set in Unity Inspector
	public CanvasScaler MainCanvas;				// Reference to Canvas since Screen.width will break resolution scaling

	private int _numTabs;						// Number of tabs
	private List<LerpController> _tabs;			// Built List of tab LerpControllers
	private List<LerpController> _tabScreens;	// Built List of tabScreen LerpControllers
	private int _selectedTabIndex = -1;			// The currently selected tab index

	private float _selectedTabWidth;	// Width of selected tab
	private float _unselectedTabWidth;	// Width of unselected tab
	private float _defaultTabHeight;	// Height of unselected tab
	private float _selectedTabHeight;	// Height of selected tab


	// Public Methods -------------------------------------------------------------------------- //
	// Initialize the tabs and calculate/set lerp variables
	public void Initialize() {
		if(TabsParentRT.childCount != TabScreensParentRT.childCount) {
			throw new System.Exception("Tab and TabScreen transforms have a different number of children, NavController will not work");
		}
		_numTabs = TabsParentRT.childCount;

		_tabs = new List<LerpController>(_numTabs);
		for(int i = 0; i < _numTabs; i++) {
			_tabs.Add(TabsParentRT.GetChild(i).gameObject.AddComponent<LerpController>().Init(8));
		}

		_tabScreens = new List<LerpController>(_numTabs);
		for(int i = 0; i < _numTabs; i++) {
			_tabScreens.Add(TabScreensParentRT.GetChild(i).gameObject.AddComponent<LerpController>().Init(20));
		}
		foreach(LerpController screen in _tabScreens) {
			screen.gameObject.AddComponent<NavDrag>().Init(this);
		}

		_selectedTabWidth = _tabs[0].rectTransform.sizeDelta.x * TAB_MULT_WIDTH;
		_unselectedTabWidth = _tabs[0].rectTransform.sizeDelta.x - ((_selectedTabWidth - _tabs[0].rectTransform.sizeDelta.x) / (_numTabs - 1));
		_defaultTabHeight = _tabs[0].rectTransform.sizeDelta.y;
		_selectedTabHeight = _defaultTabHeight * TAB_MULT_HEIGHT;

		SetTab(1, true);
	}

	// Called from the Button events in Unity Inspector, set for each Tab
	public void SelectTab(int index) {
		if(_selectedTabIndex != index) {
			SetTab(index);
		}
	}

	// Called from the NavDrag component on a GameObject
	public void DragScreen(Vector2 delta) {
		float xElastic = 0;
		if(_tabScreens[0].rectTransform.anchoredPosition.x > 0) {
			xElastic = delta.x * _tabScreens[0].rectTransform.anchoredPosition.x / (_tabScreens[0].rectTransform.anchoredPosition.x + (_tabScreens[0].rectTransform.sizeDelta.x * ELASTICITY));
		}
		if(_tabScreens[_numTabs - 1].rectTransform.anchoredPosition.x < 0) {
			xElastic = delta.x * _tabScreens[_numTabs - 1].rectTransform.anchoredPosition.x / (_tabScreens[_numTabs - 1].rectTransform.anchoredPosition.x - (_tabScreens[0].rectTransform.sizeDelta.x * ELASTICITY));
		}

		float targetX = 0;
		foreach(LerpController screen in _tabScreens) {
			targetX = screen.TargetPos.x + delta.x - xElastic;

			screen.SetTargetPosition(new Vector3(targetX, screen.rectTransform.anchoredPosition.y, 0));
		}
	}

	public void LeftMenu() {
		SetTab(Mathf.Max(_selectedTabIndex - 1, 0));
	}
	public void RightMenu() {
		SetTab(Mathf.Min(_selectedTabIndex + 1, _numTabs - 1));
	}
	public void ResetMenu() {
		SetTab(_selectedTabIndex);
	}

	private void SetTab(int index, bool init = false) {
		_selectedTabIndex = index;
		for(int i = 0; i < _numTabs; i++ ) {
			Vector2 targetSize = new Vector2(
					(index == i)? _selectedTabWidth : _unselectedTabWidth,
					(index == i)? _selectedTabHeight : _defaultTabHeight
			);
			float targetX;
			if(i < index) {
				targetX = (i + 0.5f) * _unselectedTabWidth;
			}
			else if(i == index) {
				targetX = i * _unselectedTabWidth + 0.5f * _selectedTabWidth;
			}
			else {
				targetX = index * _unselectedTabWidth + _selectedTabWidth + (i - index - 0.5f) * _unselectedTabWidth;  
			}
			targetX -= MainCanvas.referenceResolution.x / 2f;
			Vector3 targetPos = new Vector3(targetX, _tabs[i].rectTransform.anchoredPosition.y, 0);

			if(init) {
				_tabs[i].SetSize(targetSize);
				_tabs[i].SetPosition(targetPos);
			}
			else {
				_tabs[i].SetTargetSize(targetSize);
				_tabs[i].SetTargetPosition(targetPos);
			}
			
			targetPos = new Vector3((i - index) * MainCanvas.referenceResolution.x, _tabScreens[i].rectTransform.anchoredPosition.y, 0);
			if(init) {
				_tabScreens[i].SetPosition(targetPos);
			}
			else {
				_tabScreens[i].SetTargetPosition(targetPos);
			}
		}
	}
}
