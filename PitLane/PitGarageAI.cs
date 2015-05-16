using System;
using UnityEngine;
using ColossalFramework;

namespace PitLane
{
	public class PitGarageAI : DepotAI
	{
		public PitGarageAI ()
		{
			m_noiseAccumulation = 200;
			m_noiseRadius = 150;
		}

		public override Color GetColor(ushort buildingID, ref Building data, InfoManager.InfoMode infoMode)
		{
			if (infoMode == InfoManager.InfoMode.NoisePollution) {
				return CommonBuildingAI.GetNoisePollutionColor ((float)m_noiseAccumulation);
			}
			return base.GetColor (buildingID, ref data, infoMode);
		}

		public override TransportInfo GetTransportLineInfo()
		{
			return this.m_transportInfo;
		}

		protected override void ProduceGoods(ushort buildingID, ref Building buildingData, ref Building.Frame frameData, int productionRate, ref Citizen.BehaviourData behaviour, int aliveWorkerCount, int totalWorkerCount, int workPlaceCount, int aliveVisitorCount, int totalVisitorCount, int visitPlaceCount)
		{
			base.ProduceGoods(buildingID, ref buildingData, ref frameData, productionRate, ref behaviour, aliveWorkerCount, totalWorkerCount, workPlaceCount, aliveVisitorCount, totalVisitorCount, visitPlaceCount);
			int num = productionRate * this.m_noiseAccumulation / 100;
			if (num != 0)
			{
				Singleton<ImmaterialResourceManager>.instance.AddResource(ImmaterialResourceManager.Resource.NoisePollution, num, buildingData.m_position, this.m_noiseRadius);
			}
			base.HandleDead(buildingID, ref buildingData, ref behaviour, totalWorkerCount);
			TransferManager.TransferReason vehicleReason = this.m_transportInfo.m_vehicleReason;
			if (vehicleReason != TransferManager.TransferReason.None)
			{
				TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
				offer.Priority = 1;
				offer.Building = buildingID;
				offer.Position = buildingData.m_position;
				offer.Amount = 2;
				offer.Active = true;
				Singleton<TransferManager>.instance.AddOutgoingOffer(vehicleReason, offer);
			}
		}
	}
}

