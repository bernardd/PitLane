﻿using System;
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
	}
}