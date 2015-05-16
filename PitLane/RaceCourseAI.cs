using System;

namespace PitLane
{
	public class RaceCourseAI : TransportLineAI
	{
		public RaceCourseAI ()
		{
			m_maxPassengerWaitingDistance = 1;
			m_publicTransportAccumulation = 1;
			m_publicTransportRadius = 1;
		}

	}
}

