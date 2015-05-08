using System;
using ColossalFramework.UI;
using UnityEngine;

namespace PitLane {
	public class UIUtils {

		[Flags] 
		public enum FindOptions {
			None = 0, 
			NameContains = 1 << 0
		}

		static UIUtils instance = null;
		public static UIUtils Instance {
			get {
				if (instance == null) instance = new UIUtils();
				return instance;
			}
		}

		UIView uiRoot = null;

		void FindUIRoot() {
			Debug.Log("Finding UIRoot");
			uiRoot = null;

			foreach (UIView view in UIView.FindObjectsOfType<UIView>()) {
				if (view.transform.parent == null && view.name == "UIView") {
					Debug.Log("Found UIRoot");
					uiRoot = view;
					break;
				}
			}
		}

		public string GetTransformPath(Transform transform) {
			string path = transform.name;
			Transform t = transform.parent;
			while (t != null) {
				path = t.name + "/" + path;
				t = t.parent;
			}
			return path;
		}

		public T FindComponent<T>(string name, UIComponent parent = null, FindOptions options = FindOptions.None) where T : UIComponent {
			if (uiRoot == null) {
				FindUIRoot();
				if (uiRoot == null) {
					Debug.Log("UIRoot not found");
					return null;
				}
			}

			foreach (T component in UIComponent.FindObjectsOfType<T>()) {
				bool nameMatches;
				if ((options & FindOptions.NameContains) != 0) nameMatches = component.name.Contains(name);
				else nameMatches = component.name == name;

				if (!nameMatches) continue;

				Transform parentTransform;
				if (parent != null) parentTransform = parent.transform;
				else parentTransform = uiRoot.transform;

				Transform t = component.transform.parent;
				while (t != null && t != parentTransform) {
					t = t.parent;
				}

				if (t == null) continue;

				return component;
			}

			Debug.Log(typeof(T) + " not found: " + name);

			return null;
		}

		public static UITextureAtlas CreateTextureAtlas(string textureFile, string atlasName, Material baseMaterial, int spriteWidth, int spriteHeight, string[] spriteNames) {

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