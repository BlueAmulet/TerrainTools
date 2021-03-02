using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrainTools.Patches;
using UnityEngine;

namespace TerrainTools
{
	internal class LightManager : MonoBehaviour
	{
		private Light light;

		public void Awake()
		{
			light = GetComponent<Light>();
		}

		public void Update()
		{
			if (light == null || Player.m_localPlayer == null)
			{
				Destroy(this);
			}
			else
			{
				light.intensity = Settings.lightStrength.Value;
				light.enabled = Commands.debugTerrain && Vector3.Distance(Player.m_localPlayer.transform.position, transform.position) <= Settings.lightDistance.Value;
			}
		}
	}
}
