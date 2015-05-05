using System;
using ICities;
using ColossalFramework.UI;
using UnityEngine;
using System.Reflection;

namespace PitLane
{
	public class PitLaneInfo : IUserMod {
		public string Name {
			get { return "Race Pit Lane"; }
		} 

		public string Description {
			get { return "Adds a pit lane object for spawning race cars, and the cars themselves"; }
		}
	}

	public class Loader : LoadingExtensionBase {

		public override void OnCreated(ILoading loading)
		{
			Debug.Log ("OnCreated()");
			base.OnCreated (loading);

			UIComponent publicTransportPanel = UIUtils.Instance.FindComponent<UIComponent>("PublicTransportPanel", null, UIUtils.FindOptions.NameContains);

			foreach (UIComponent c in publicTransportPanel.components) {
				Debug.Log ("Here 0 " + c.name);
				if (c.name == "GroupToolstrip") {
					Debug.Log ("Here 1");

					//c.AddUIComponent<PitLaneButton> ();
				}
			}
					

			if (publicTransportPanel == null || !publicTransportPanel.gameObject.activeInHierarchy) return;

			//publicTransportPanel.AddUIComponent<PitLaneButton> ();

			Debug.Log ("OnCreated() complete");
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



		//	UITextureAtlas atlas = CreateTextureAtlas("buttons.png", "PitLaneUI", tabTemplate.atlas.material, 64, 34, spriteNames);
		}

		UITextureAtlas CreateTextureAtlas(string textureFile, string atlasName, Material baseMaterial, int spriteWidth, int spriteHeight, string[] spriteNames) {

			Texture2D tex = new Texture2D(spriteWidth * spriteNames.Length, spriteHeight, TextureFormat.ARGB32, false);
			tex.filterMode = FilterMode.Bilinear;

			{ // LoadTexture
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
				System.IO.Stream textureStream = assembly.GetManifestResourceStream("Crossings." + textureFile);

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

