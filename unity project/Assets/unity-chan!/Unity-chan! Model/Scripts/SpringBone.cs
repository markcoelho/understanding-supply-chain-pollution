//
//SpringBone.cs for unity-chan!
//
//Original Script is here:
//ricopin / SpringBone.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
//Revised by N.Kobayashi 2014/06/20
//
using UnityEngine;
using System.Collections;

namespace UnityChan
{
	public class SpringBone : MonoBehaviour
	{
		//次のボーン
		public Transform child;

		//ボーンの向き
		public Vector3 boneAxis = new Vector3 (-1.0f, 0.0f, 0.0f);
		public float radius = 0.05f;

		//各SpringBoneに設定されているstiffnessForceとdragForceを使用するか？
		public bool isUseEachBoneForceSettings = false; 

		//バネが戻る力
		public float stiffnessForce = 0.01f;

		//力の減衰力
		public float dragForce = 0.4f;
		public Vector3 springForce = new Vector3 (0.0f, -0.0001f, 0.0f);
		public SpringCollider[] colliders;
		public bool debug = true;
		//Kobayashi:Thredshold Starting to activate activeRatio
		public float threshold = 0.01f;
		private float springLength;
		private Quaternion localRotation;
		private Transform trs;
		private Vector3 currTipPos;
		private Vector3 prevTipPos;
		//Kobayashi
		private Transform org;
		//Kobayashi:Reference for "SpringManager" component with unitychan 
		private SpringManager managerRef;

		private void Awake ()
		{
			trs = transform;
			localRotation = transform.localRotation;
			//Kobayashi:Reference for "SpringManager" component with unitychan
			// GameObject.Find("unitychan_dynamic").GetComponent<SpringManager>();
			managerRef = GetParentSpringManager (transform);
		}

		private SpringManager GetParentSpringManager (Transform t)
		{
			var springManager = t.GetComponent<SpringManager> ();

			if (springManager != null)
				return springManager;

			if (t.parent != null) {
				return GetParentSpringManager (t.parent);
			}

			return null;
		}

		private void Start ()
		{
			springLength = Vector3.Distance (trs.position, child.position);
			currTipPos = child.position;
			prevTipPos = child.position;
		}

		public void UpdateSpring ()
		{
			
		}

		private void OnDrawGizmos ()
		{
			if (debug) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere (currTipPos, radius);
			}
		}
	}
}
