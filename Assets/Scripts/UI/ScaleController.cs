using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleController : MonoBehaviour {

	public RectTransform LevelScreen;
	public RectTransform LevelScrollMask;
	public RectTransform PlayButton;

	public void SetMainMenuUI() {
		PlayButton.anchoredPosition = new Vector2(0, (LevelScreen.rect.size.y - LevelScrollMask.rect.size.y) / 4f);
	}
}