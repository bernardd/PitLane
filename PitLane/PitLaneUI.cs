using System;
using ICities;
using ColossalFramework.UI;
using ColossalFramework;
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

		TransportInfo raceCourse = null;
		BuildingInfo pitGarage = null;

		public bool isVisible { get; private set; }

		bool Initialize() {
			Debug.Log("Initializing UI");

			if (UIUtils.Instance == null) return false;

			UIPanel publicTransportPanel = UIUtils.Instance.FindComponent<UIPanel>("PublicTransportPanel", null, UIUtils.FindOptions.NameContains);
			if (publicTransportPanel == null || !publicTransportPanel.gameObject.activeInHierarchy) return false;

			publicTransportTabstrip = UIUtils.Instance.FindComponent<UITabstrip> ("GroupToolstrip", publicTransportPanel, UIUtils.FindOptions.NameContains);
			publicTransportContainer = UIUtils.Instance.FindComponent<UITabContainer> ("GTSContainer", publicTransportPanel, UIUtils.FindOptions.NameContains);

			if (pitGarage == null)
				CreatePitGarage ();

			if (raceCourse == null)
				CreateRaceCourse ();

			pitLaneButton = UIUtils.Instance.FindComponent<UIButton>("PitLaneButton");
			if (pitLaneButton != null) {
				DestroyView();
			}

			CreateView();
			Hook ();
			if (pitLaneButton == null) return false; 
	
			// Discovery only:
			//DiscoverUI(publicTransportPanel, 0);
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
			BuildingTool bt = GetTool<BuildingTool> ();
			Debug.Log ("pitGarage: " + pitGarage);
			bt.m_prefab = pitGarage;
			ToolsModifierControl.toolController.CurrentTool = bt;
		}

		void CourseButtonClickHandler(UIComponent button, UIMouseEventParameter evt)
		{
			TransportTool nt = GetTool<TransportTool> ();
			nt.m_prefab = raceCourse;
			ToolsModifierControl.toolController.CurrentTool = nt;
		}

		T GetTool<T>() where T : ToolBase
		{
			ToolBase[] tools = UnityEngine.Object.FindObjectOfType<ToolController> ().Tools;
			foreach (ToolBase tb in tools) {
				if (tb is T) {
					return tb as T;
				}
			}
			return default(T);
		}

		T GetCollection<T>(string name) where T : UnityEngine.MonoBehaviour
		{
			foreach (T o in UnityEngine.GameObject.FindObjectsOfType<T>()) {
				if (o.name == name) {
					return o;
				}
			}
			return null;
		}

		T GetPrefabByName<T>(string name, PrefabInfo[] prefabs) where T : PrefabInfo
		{
			foreach (PrefabInfo p in prefabs) {
				if (name == p.name) {
					return p as T;
				}
			}
			return null;
		}

		void CreatePitGarage() {
			BuildingCollection bc = GetCollection<BuildingCollection> ("Public Transport");
			Debug.Log ("bc: " + bc);
			BuildingInfo busDepot = GetPrefabByName<BuildingInfo>("Bus Depot", bc.m_prefabs);
			Debug.Log ("buDepot: " + busDepot);
			pitGarage = PrefabCloner.CloneBuilding (busDepot);
			Debug.Log ("---------------------");
	//		DumpObject (pitGarage);
			Debug.Log ("---------------------");
			//CompareObjects(busDepot, pitGarage);
			Debug.Log ("pitGarage1: " + pitGarage.transform);
			pitGarage.transform.SetParent (busDepot.transform.parent);
			Debug.Log ("pitGarage2: " + pitGarage );
			pitGarage.name = "Pit Garage";
			PitGarageAI garageAI = new PitGarageAI ();
			//garageAI.m_info = pitGarage;
			garageAI.m_transportInfo = PrefabCloner.ShallowCopy (((DepotAI)busDepot.m_buildingAI).m_transportInfo);
			garageAI.m_transportInfo.m_vehicleReason = TransferManager.TransferReason.DummyCar;
			pitGarage.m_buildingAI = garageAI;

			Debug.Log ("pitGarage3: " + pitGarage);
		}

		void CreateRaceCourse() {
			TransportCollection tc = GetCollection<TransportCollection> ("Public Transport");
			TransportInfo bus = GetPrefabByName<TransportInfo> ("Bus", tc.m_prefabs);
			raceCourse = PrefabCloner.CloneTransport (bus);

			raceCourse.m_netInfo = PrefabCloner.ShallowCopy (bus.m_netInfo);
			RaceCourseAI raceCourseAI = new RaceCourseAI ();
			//raceCourseAI.m_info = raceCourse.m_netInfo;
			raceCourse.m_netInfo.m_netAI = raceCourseAI;
			raceCourse.m_vehicleReason = TransferManager.TransferReason.DummyCar;
			raceCourse.name = "Race Course";
	
			Debug.Log ("*******************");
		}

		void DumpObject(object o)
		{
			Type T = o.GetType ();
			Debug.Log ("TYPE: " + T);
			foreach (FieldInfo fi in o.GetType().GetFields()) {
				Debug.Log(fi.Name + " = " + fi.GetValue(o));
			}
		}

		void CompareObjects<T>(T o1, T o2)
		{
			foreach (FieldInfo fi in o1.GetType().GetFields()) {
				if (!Equals(fi.GetValue (o1),fi.GetValue (o2))) {
					Debug.Log ("Unequal: " + fi.Name + " => " + fi.GetValue (o1) + " != " + fi.GetValue (o2));
				}
			}
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



		// NetCollection "Bus Line" NetInfo
		// BuildingCollection "Bus Depot" BuildingInfo
		T TryGetComponent<T>(string name)
		{
			GameObject go = GameObject.Find (name);
			if (go != null)
				return go.GetComponent<T> ();

			return default(T);
		}

		static void Hook()
		{
			var allFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
			MethodInfo method = typeof(DepotAI).GetMethod("StartTransfer", allFlags);
			RedirectionHelper.RedirectCalls (method, typeof(PitLaneUI).GetMethod ("StartTransfer", allFlags));
		}

		public void StartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason reason, TransferManager.TransferOffer offer)
		{
			DepotAI thisObject = (DepotAI)BuildingManager.instance.m_buildings.m_buffer[buildingID].Info.m_buildingAI;
			Debug.Log ("ID:" + buildingID + " /Reason:" + reason);
			DumpObject (data);
			DumpObject (offer);
			if (reason == thisObject.m_transportInfo.m_vehicleReason)
			{
				VehicleInfo randomVehicleInfo = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, thisObject.m_info.m_class.m_service, thisObject.m_info.m_class.m_subService, thisObject.m_info.m_class.m_level);
				if (randomVehicleInfo != null)
				{
					Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
					Vector3 position;
					Vector3 vector;
					thisObject.CalculateSpawnPosition(buildingID, ref data, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, out position, out vector);
					ushort num;
					if (Singleton<VehicleManager>.instance.CreateVehicle(out num, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, position, reason, false, true))
					{
						randomVehicleInfo.m_vehicleAI.SetSource(num, ref vehicles.m_buffer[(int)num], buildingID);
						randomVehicleInfo.m_vehicleAI.StartTransfer(num, ref vehicles.m_buffer[(int)num], reason, offer);
					}
				}
			}
			else
			{
				// BuildingAI.StartTransfer(buildingID, ref data, reason, offer);  // No-op - no need to call for now
			}
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



