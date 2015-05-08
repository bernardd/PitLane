using System;
using ICities;
using ColossalFramework.UI;
using UnityEngine;
using System.Reflection;

namespace PitLane
{
	public class PitLaneUI
	{
		public PitLaneUI ()
		{
		}

		UITabstrip publicTransportTabstrip;
		UITabContainer publicTransportContainer;
		UIButton pitLaneButton;
		UIPanel pitLanePanel;

		public bool isVisible { get; private set; }

		bool Initialize() {
			Debug.Log("Initializing UI");

			if (UIUtils.Instance == null) return false;

			UIPanel publicTransportPanel = UIUtils.Instance.FindComponent<UIPanel>("PublicTransportPanel", null, UIUtils.FindOptions.NameContains);
			if (publicTransportPanel == null || !publicTransportPanel.gameObject.activeInHierarchy) return false;

			publicTransportTabstrip = UIUtils.Instance.FindComponent<UITabstrip> ("GroupToolstrip", publicTransportPanel, UIUtils.FindOptions.NameContains);
			publicTransportContainer = UIUtils.Instance.FindComponent<UITabContainer> ("GTSContainer", publicTransportPanel, UIUtils.FindOptions.NameContains);


			pitLaneButton = UIUtils.Instance.FindComponent<UIButton>("PitLaneButton");
			if (pitLaneButton != null) {
				DestroyView();
			}

			CreateView();
			if (pitLaneButton == null) return false; 
	
			// Discovery only:
			DiscoverUI(publicTransportPanel, 0);
			//

			return true;
		}		

		void DiscoverUI(UIComponent c, int level) {
			string stars = "";
			for (int i = 0; i < level; i++)
				stars += "*";
			Debug.Log (stars + "Name: " + c.name + " : " + c.GetType());
			if (c is UIButton) {
				UIButton b = (UIButton)c;
				Debug.Log (stars + "--Button sprites: " + b.normalBgSprite + " " + b.normalFgSprite);
				Debug.Log (stars + "         atlas: " + b.atlas.name);
				Debug.Log (stars + "         other: " + b.relativePosition + " / " + b.absolutePosition);
				Debug.Log (stars + "         other: " + b.width + " / " + b.height);
			}
			foreach (UIComponent d in c.components) {
				DiscoverUI (d, level +1);
			}
		}

		void CreateView() {
			Debug.Log("Creating view");

			pitLaneButton = publicTransportTabstrip.AddUIComponent<PitLaneButton>();

			UIPanel busPanel = UIUtils.Instance.FindComponent<UIPanel> ("PublicTransportBusPanel", null, UIUtils.FindOptions.NameContains);

			pitLanePanel = publicTransportContainer.AddUIComponent<UIPanel> ();
			pitLanePanel.autoFitChildrenHorizontally = true;
			pitLanePanel.autoLayout = true;
			pitLanePanel.autoLayoutPadding = new RectOffset (0, 0, 20, 0);

			//UIScrollablePanel scrollPanel = pitLanePanel.AddUIComponent<UIScrollablePanel> ();
			//UIScrollbar scrollBar = pitLanePanel.AddUIComponent<UIScrollbar> ();

			UIButton pitButton = pitLanePanel.AddUIComponent<UIButton> ();
			UIButton courseButton = pitLanePanel.AddUIComponent<UIButton> ();

			pitButton.atlas = courseButton.atlas = UIView.GetAView ().defaultAtlas;

			pitButton.name = "PitButton";
			pitButton.normalFgSprite = pitButton.pressedFgSprite = "ThumbnailTrophy";
			pitButton.eventClick += PitButtonClickHandler;
			pitButton.autoSize = true;

			courseButton.name = "CourseButton";
			courseButton.normalFgSprite = courseButton.pressedFgSprite = "ThumbnailJunctionsClover"; // Route icon
			courseButton.eventClick += CourseButtonClickHandler;
			courseButton.autoSize = true;
			courseButton.relativePosition = new Vector3 (0, -20);
		}

		void PitButtonClickHandler(UIComponent button, UIMouseEventParameter evt)
		{
			Debug.Log ("Pit Button!");
		}

		void CourseButtonClickHandler(UIComponent button, UIMouseEventParameter evt)
		{
			Debug.Log ("Course Button!");
		}

		public void DestroyView() {
	/*		if (pitLaneButton != null) {
				UIView.Destroy(pitLaneButton);
				pitLaneButton = null;
			}
			*/ // Hopefully cleaned up by parents?
		}

		bool initialized {
			get { return pitLaneButton != null; }
		}

		public void Show() {
			if (!initialized) {
				if (!Initialize()) return;
			}

			Debug.Log("Showing UI");
			isVisible = true;
		}
	}

	public class PitLaneButton : UIButton {
		PitLaneButton() {
			name = "PitLaneButton";
			tooltip = "Pit Lane";

			string[] spriteNames = {
				"PitLaneButtonBg", 
				"PitLaneButtonBgHovered", 
				"PitLaneButtonBgPressed", 
				"PitLaneIcon",
				"PitLaneIconPressed", 
			};

			atlas = UIUtils.CreateTextureAtlas("buttons.png", "PitLaneUI", atlas.material, 60, 25, spriteNames);

			normalBgSprite = "PitLaneButtonBg";
			disabledBgSprite = "PitLaneButtonBg";
			hoveredBgSprite = "PitLaneButtonBgHovered";
			pressedBgSprite = "PitLaneButtonBgPressed";
			focusedBgSprite = "PitLaneButtonBgPressed";
			normalFgSprite = "PitLaneIcon";
			pressedFgSprite = "PitLaneIconPressed";

			width = 60;
		}



	}

}

