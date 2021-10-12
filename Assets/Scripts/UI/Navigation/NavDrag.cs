using UnityEngine;
using UnityEngine.EventSystems;

// Governs the horizontal drag behavior for tabscreen navigation
public class NavDrag : MonoBehaviour, IDragHandler, IEndDragHandler {

	private const float QUICKDRAG_TIME = 0.2f;	// Drags less than this time will count as quick drags
	private const float QUICKDRAG_DIST = 0.2f;	// Quick drags have to move at least this % of screen to count
	private const float LONGDRAG_MIN_X = 0.3f;	// Long drags must start within this % of their side of the screen
	private const float LONGDRAG_DIST = 0.60f;	// Long drags must move at least this % of screen to count

	private NavController _controller;
	private bool _dragging;
	private Vector2 _direction;
	private float _timer;


	void Update() {
		if(_dragging) {
			_timer += Time.deltaTime;
		}
	}

	public void Init(NavController navController) {
		_controller = navController;
		_dragging = false;
		_direction = Vector2.zero;
		_timer = 0;
	}

	public void OnDrag(PointerEventData e) {
		if(!_dragging) {
			_direction = (Mathf.Abs(e.delta.x) >= Mathf.Abs(e.delta.y))? new Vector2(1, 0) : new Vector2(0, 1);
			_dragging = true;
		}
		
		if(_direction.x == 1) {
			_controller.DragScreen(e.delta * _direction);
		}
	}

	public void OnEndDrag(PointerEventData e) {
		if(_direction.x == 1) {
			float deltaX = e.position.x - e.pressPosition.x;
			if(_timer <= QUICKDRAG_TIME && Mathf.Abs(deltaX) > Screen.width * QUICKDRAG_DIST) {
				if(deltaX > 0) {
					_controller.LeftMenu();
				}
				else {
					_controller.RightMenu();
				}
			}
			else if(e.position.x < Screen.width * LONGDRAG_MIN_X && Mathf.Abs(deltaX) > Screen.width * LONGDRAG_DIST) {
				_controller.RightMenu();
			}
			else if(e.position.x > Screen.width * (1f - LONGDRAG_MIN_X) && Mathf.Abs(deltaX) > Screen.width * LONGDRAG_DIST) {
				_controller.LeftMenu();
			}
			else {
				_controller.ResetMenu();
			}
		}
		
		_dragging = false;
		_direction = Vector2.zero;
		_timer = 0;
	}
}
