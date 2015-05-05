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

			Debug.Log ("OnCreated() complete");
		}

	}
	public class ThreadingExtension : ThreadingExtensionBase {
		public static ThreadingExtension Instance { get; private set; }

		PitLaneUI ui = new PitLaneUI();
		bool loadingLevel = false;

		public void OnLevelUnloading() {
			ui.DestroyView();
			loadingLevel = true;
		}

		public void OnLevelLoaded(LoadMode mode) {
			loadingLevel = false;
			Debug.Log("OnLevelLoaded");
		}

		public override void OnCreated(IThreading threading) {
			Instance = this;
		}

		public override void OnReleased() {
			ui.DestroyView();
		}

		public override void OnUpdate(float realTimeDelta, float simulationTimeDelta) {
			if (loadingLevel)
				return;

			if (!ui.isVisible) {
				ui.Show ();
			}
		}

	}





}

