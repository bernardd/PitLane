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

		public bool isVisible { get; private set; }

		bool Initialize() {
			Debug.Log("Initializing UI");

			if (UIUtils.Instance == null) return false;

			UIPanel publicTransportPanel = UIUtils.Instance.FindComponent<UIPanel>("PublicTransportPanel", null, UIUtils.FindOptions.NameContains);
			if (publicTransportPanel == null || !publicTransportPanel.gameObject.activeInHierarchy) return false;

			publicTransportTabstrip = UIUtils.Instance.FindComponent<UITabstrip> ("GroupToolstrip", publicTransportPanel, UIUtils.FindOptions.NameContains);
			publicTransportContainer = UIUtils.Instance.FindComponent<UITabContainer> ("GTSContainer", publicTransportPanel, UIUtils.FindOptions.NameContains);

			// Discovery only:
			foreach (UIComponent c in publicTransportPanel.components) {
				Debug.Log ("Name: " + c.name + " " + c.GetType());
				foreach (UIComponent d in c.components) {
					Debug.Log ("***Name: " + d.name + " " + d.GetType());
				}
			}
			//

			pitLaneButton = UIUtils.Instance.FindComponent<UIButton>("PitLaneButton");
			if (pitLaneButton != null) {
				DestroyView();
			}

			CreateView();
			if (pitLaneButton == null) return false; 

			return true;
		}		

		void CreateView() {
			Debug.Log("Creating view");

			pitLaneButton = publicTransportTabstrip.AddUIComponent<PitLaneButton>();
		}

		public void DestroyView() {
			if (pitLaneButton != null) {
				UIView.Destroy(pitLaneButton);
				pitLaneButton = null;
			}
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
			tooltip = "Pit Lane management";

			string[] spriteNames = {
				"PitLaneButtonBg", 
				"PitLaneButtonBgHovered", 
				"PitLaneButtonBgPressed", 
				//		"CrossingsIcon", 
				//		"CrossingsIconPressed", 
			};

			atlas = CreateTextureAtlas("buttons.png", "PitLaneUI", atlas.material, 64, 25, spriteNames);

			normalBgSprite = "PitLaneButtonBg";
			disabledBgSprite = "PitLaneButtonBg";
			hoveredBgSprite = "PitLaneButtonBgHovered";
			pressedBgSprite = "PitLaneButtonBgPressed";
			focusedBgSprite = "PitLaneButtonBgPressed";

			relativePosition = new Vector3(323, -25);
			width = 64;
		}

		UITextureAtlas CreateTextureAtlas(string textureFile, string atlasName, Material baseMaterial, int spriteWidth, int spriteHeight, string[] spriteNames) {

			Texture2D tex = new Texture2D(spriteWidth * spriteNames.Length, spriteHeight, TextureFormat.ARGB32, false);
			tex.filterMode = FilterMode.Bilinear;
			{ // LoadTexture
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
				System.IO.Stream textureStream = assembly.GetManifestResourceStream("PitLane." + textureFile);

				byte[] buf = new byte[textureStream.Length];  //declare arraysize
				textureStream.Read(buf, 0, buf.Length); // read from stream to byte array

				tex.LoadImage(buf);

				tex.Apply(true, true);
			}
			UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();

			{ // Setup atlas
				Material material = (Material)Material.Instantiate(baseMaterial);
				material.mainTexture = tex;

				atlas.material = material;
				atlas.name = atlasName;
			}

			// Add sprites
			for (int i = 0; i < spriteNames.Length; ++i) {
				float uw = 1.0f / spriteNames.Length;

				var spriteInfo = new UITextureAtlas.SpriteInfo() {
					name = spriteNames[i],
					texture = tex,
					region = new Rect(i * uw, 0, uw, 1),
				};

				atlas.AddSprite(spriteInfo);
			}

			return atlas;
		}

	}

}

