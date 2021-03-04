using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TerrainTools
{
	internal class RestoreData
	{
		public readonly Vector3 position;
		public readonly GameObject prefab;
		public readonly long creationTime;

		public RestoreData(Vector3 position, GameObject prefab, long creationTime)
		{
			this.position = position;
			this.prefab = prefab;
			this.creationTime = creationTime;
		}
	}
}
