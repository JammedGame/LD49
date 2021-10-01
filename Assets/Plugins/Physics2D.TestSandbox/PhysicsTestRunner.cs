using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

namespace Physics2D
{
	public class PhysicsTestRunner : MonoBehaviour
	{
		public PhysicsSettings Settings;

		PhysicsWorld scene;
		List<BaseCollider> childrenList = new List<BaseCollider>();

		void Awake()
		{
			scene = new PhysicsWorld();
		}

		public void Update()
		{
			gameObject.GetComponentsInChildren<BaseCollider>(childrenList);

			scene.Reset();

			// add children
			Profiler.BeginSample("Physics.AddColliders");
				foreach (var child in childrenList)
				{
					child.AddToScene(scene);
				}
			Profiler.EndSample();

			//tick
			Profiler.BeginSample("Physics.Tick");
			    scene.Tick(Settings, 1f / 60f);
			Profiler.EndSample();

			// update back to the scene.
			Profiler.BeginSample("Physics.ApplyChanges");
				for (int i = 0; i < childrenList.Count; i++)
				{
					var (position, velocity) = scene.GetPositionAndVelocity(i);
					childrenList[i].ApplyNewPosition(position, velocity);
				}
			Profiler.EndSample();
		}

		public void OnDrawGizmos()
		{
			var children = gameObject.GetComponentsInChildren<BaseCollider>();

			for (int i = 0; i < children.Length; i++)
			{
				Gizmos.color = scene != null && scene.HasBroadPhaseContact(i) ? Color.red : Color.white;

				children[i].DrawGizmos();
			}
		}

		void OnDestroy()
		{
			scene?.Dispose();
		}
	}
}