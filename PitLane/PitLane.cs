﻿using System;
using ICities;
using ColossalFramework.UI;
using UnityEngine;
using System.Reflection;

namespace PitLane
{
	public class PitLaneInfo : IUserMod {
		public string Name {
			get { return "Pit Lane"; }
		} 

		public string Description {
			get { return "Adds a pit lane object for spawning race cars, and the cars themselves"; }
		}
	}

	public class Loader : LoadingExtensionBase {
		public static PitLaneUI ui = new PitLaneUI();
		public static bool loadingLevel = false;

		public override void OnLevelUnloading() {
			base.OnLevelUnloading ();
			ui.DestroyView();
			loadingLevel = true;
		}

		public override void OnLevelLoaded(LoadMode mode) {
			base.OnLevelLoaded (mode);
			loadingLevel = false;
			if (ui.isVisible)
				ui.DestroyView ();

			ui.Show ();
			Debug.Log("OnLevelLoaded");
		}

		public override void OnCreated(ILoading loading)
		{
			Debug.Log ("OnCreated()");
			base.OnCreated (loading);

			Debug.Log ("OnCreated() complete");
		}

		public override void OnReleased() {
			ui.DestroyView();
			base.OnReleased ();
		}

	}
	public class ThreadingExtension : ThreadingExtensionBase {
		public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
			if (Loader.loadingLevel)
				return;

			if (!Loader.ui.isVisible) {
				Loader.ui.Show ();
			}
		}

		#if false
		void CreateBuildTool() {
			if (buildTool == null) {
				buildTool = ToolsModifierControl.toolController.gameObject.GetComponent<CrossingTool>();
				if (buildTool == null) {  
					buildTool = ToolsModifierControl.toolController.gameObject.AddComponent<CrossingTool>();
					Debug.Log("Tool created: " + buildTool);
				}
				else {
					Debug.Log("Found existing tool: " + buildTool);
				}
			} 
		}

		void DestroyBuildTool() {
			if (buildTool != null) {
				Debug.Log("Tool destroyed");
				CrossingTool.Destroy(buildTool);
				buildTool = null;
			}
		}

		void SetToolMode(ToolMode mode, bool resetNetToolModeToStraight = false) {
			if (mode == toolMode) return;

			if (mode != ToolMode.Off) {
				CreateBuildTool();
				ToolsModifierControl.toolController.CurrentTool = buildTool;

				if (mode == ToolMode.On) {
					Debug.Log("Crossing placement mode activated");
					toolMode = ToolMode.On;
				}

				ui.toolMode = toolMode;
			}
			else {
				Debug.Log("Tool disabled");
				toolMode = ToolMode.Off;

				if (ToolsModifierControl.toolController.CurrentTool == buildTool || ToolsModifierControl.toolController.CurrentTool == null) {
					ToolsModifierControl.toolController.CurrentTool = netTool;
				}

				DestroyBuildTool();

				ui.toolMode = toolMode;

				if (resetNetToolModeToStraight) {
					netTool.m_mode = NetTool.Mode.Straight;
					Debug.Log("Reseted netTool mode: " + netTool.m_mode);
				}
			}
		}

		public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
			if (loadingLevel)
				return;

			if (roadsPanel == null) {
				roadsPanel = UIView.Find<UIPanel> ("RoadsPanel");
			}

			if (roadsPanel == null || !roadsPanel.isVisible) {
				if (toolMode != ToolMode.Off) {
					Debug.Log ("Roads panel no longer visible");
					SetToolMode (ToolMode.Off, true);
				}
				return;
			}

			if (netTool == null) {
				foreach (var tool in ToolsModifierControl.toolController.Tools) {
					NetTool nt = tool as NetTool;
					if (nt != null && nt.m_prefab != null) {
						Debug.Log ("NetTool found: " + nt.name);
						netTool = nt;
						break;
					}
				}

				if (netTool == null)
					return;

				Debug.Log ("UI visible: " + ui.isVisible);
			}

			if (!ui.isVisible) {
				ui.Show ();
			}

			if (toolMode != ToolMode.Off) {
				if (ToolsModifierControl.toolController.CurrentTool != buildTool) {
					Debug.Log ("Another tool selected");
					SetToolMode (ToolMode.Off);
				}
			} else {
				ui.toolMode = ToolMode.Off;

				if (ToolsModifierControl.toolController.CurrentTool == buildTool) {
					ToolsModifierControl.toolController.CurrentTool = netTool;
				}
			}
		}
		#endif

	}

}

