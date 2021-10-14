using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleController : MonoBehaviour {

	public CanvasScaler Canvas;

	public RectTransform LevelScreen;
	public void SetMainMenuUI() {
		// ------------------------------------------------------- //
		// Set LevelScreen UI Components
		// y = 0 is top of Tabs bar, building upwards
		RectTransform scrollMask = LevelScreen.Find("Viewport").Find("Mask").GetComponent<RectTransform>();
		RectTransform playButton = LevelScreen.Find("Play").GetComponent<RectTransform>();
		Transform scrollContent = scrollMask.Find("Content");

		// First, scale scrollmask by % of canvas ref width vs screen size (width since CanvasScaler set to scale by width) - for tablet support
		scrollMask.localScale = new Vector2(1f, (1f + (Canvas.referenceResolution.x / Screen.width)) / 2f);
		// Rescale child transform to counteract above rescaling
		scrollContent.GetComponent<RectTransform>().localScale = new Vector2((1f + (Canvas.referenceResolution.x / Screen.width)) / 2f, 1f);

		// Size of blank space above & below scrollview (each, not total)
		float vertScrollPadding = (LevelScreen.ScaledSize(0).y - scrollMask.ScaledSize(0).y) / 2f;

		// Play button scaled by % of canvas ref width vs screen size
		playButton.localScale = Vector2.one * (1f + (Canvas.referenceResolution.x / Screen.width)) / 2f;
		// Play button goes either bottom of scroll view minus spacing, or half of bottom padding - whichever is higher
		playButton.anchoredPosition = new Vector2(0, Mathf.Max((vertScrollPadding / 2f) + (playButton.ScaledSize(0).y / 2f), vertScrollPadding - 75f));

		// Scroll content is scrolled to start at bottom (tier 1)
		scrollContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -vertScrollPadding);

		// Space tiers up the content screen evenly
		float startY = (LevelScreen.ScaledSize(0).y / 2f) / scrollContent.GetComponent<RectTransform>().RecursiveScale(1).y;
		float incY = scrollContent.GetChild(0).GetComponent<RectTransform>().ScaledSize(1).y;
		float spacing = 200f;
		for(int tier = 0; tier < scrollContent.childCount; tier++) {
			scrollContent.GetChild(tier).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, startY + (tier * (incY + spacing)));
		}

		// Tier info & Multiplier shown above scroll view, inverse from play button. Y position anchored at top of screen for less math
		RectTransform tierInfo = LevelScreen.Find("Title").GetComponent<RectTransform>();
		tierInfo.anchoredPosition = new Vector2(0, -playButton.anchoredPosition.y);

		
	}
}