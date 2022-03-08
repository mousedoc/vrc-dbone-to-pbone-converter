using UnityEngine;
using UnityEditor;

public class DynamicToPhysicsConverter : EditorWindow
{
	private Object BaseObject;
	private bool IsColilderConverted = true;
	private bool IsDynamicBoneRemove = true;
	private bool IsDynamicBoneColilderRemove = true;
	private bool IsInBoundColilderRemove = true;
	private bool IsMovementParameterConvert = true;
	[MenuItem ("Tools/Dynamic To Physics Converter")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(DynamicToPhysicsConverter));
	}
	void OnGUI()
	{
		GUILayout.Label("Dynamic To Physics Converter");
		GUILayout.Label("\n");
		BaseObject = EditorGUILayout.ObjectField("Root Armature", BaseObject, typeof(GameObject), true);
		GUILayout.Label("\n");
		GUILayout.Label("Option");
		IsMovementParameterConvert = GUILayout.Toggle(IsMovementParameterConvert,"Cenvert Movement Parameter");
		IsColilderConverted = GUILayout.Toggle(IsColilderConverted,"Cenvert Dynamic Bone Colilder");
		IsDynamicBoneRemove = GUILayout.Toggle(IsDynamicBoneRemove,"Remove Dynamic Bone");
		IsDynamicBoneColilderRemove = GUILayout.Toggle(IsDynamicBoneColilderRemove,"Remove Dynamic Bone Colilder");
		IsInBoundColilderRemove = GUILayout.Toggle(IsInBoundColilderRemove,"Remove InBound Colilder");
		if(GUILayout.Button("Convert!"))
		{
			if(IsColilderConverted)
				ConvertDtPColilder();
			ConvertDtP();
			if(IsDynamicBoneRemove)
				RemoveDynamicBone();
			if(IsDynamicBoneColilderRemove)
				RemoveDynamicColilder();
			Debug.Log("Done!");
		}
	}
	private void ConvertDtP()
	{
		GameObject GameObjectBase = (GameObject)BaseObject;

		DynamicBone[] DBone = GameObjectBase.GetComponentsInChildren<DynamicBone>();
		for(int i = 0; i < DBone.Length; i++)
		{
			GameObject SelectedObject = DBone[i].gameObject;
			VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone PBone;
			
			PBone = SelectedObject.AddComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>();
			
			//RootBone
			PBone.rootTransform = DBone[i].m_Root;
			//ignore Transform
			PBone.ignoreTransforms = DBone[i].m_Exclusions;
			//Radius
			PBone.radius = DBone[i].m_Radius;
			//RadiusCurve
			PBone.radiusCurve = DBone[i].m_RadiusDistrib;
			
			if(IsMovementParameterConvert)
			{
				//pull
				PBone.pull = DBone[i].m_Elasticity * 0.8f + 0.2f;
				PBone.pullCurve = DBone[i].m_ElasticityDistrib;

				//spring
				PBone.spring = DBone[i].m_Damping * 0.7f + 0.3f;
				PBone.springCurve = DBone[i].m_DampingDistrib;

				//immobile
				PBone.immobile = DBone[i].m_Inert + DBone[i].m_Stiffness;
				PBone.immobileCurve = DBone[i].m_StiffnessDistrib;
			}

			for(int j = 0; j < DBone[i].m_Colliders.Count && IsColilderConverted; j++)
			{
				if(DBone[i].m_Colliders[j] == null)
				{
					PBone.colliders.Add(null);
				}
				else if(!IsInBoundColilderRemove)
				{
					int k = 0;
					while(DBone[i].m_Colliders[j] != DBone[i].m_Colliders[j].gameObject.GetComponents<DynamicBoneCollider>()[k])
						k++;
					PBone.colliders.Add(DBone[i].m_Colliders[j].gameObject.GetComponents<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider>()[k]);
				}
			}
		}
	}
	private void ConvertDtPColilder()
	{
		GameObject GameObjectBase = (GameObject)BaseObject;

		DynamicBoneCollider[] DBoneC = GameObjectBase.GetComponentsInChildren<DynamicBoneCollider>(true);
			
		for(int i = 0; i < DBoneC.Length; i++)
		{
			
			if(DBoneC[i].m_Bound == DynamicBoneCollider.Bound.Outside || !IsInBoundColilderRemove)
			{
				GameObject SelectedObject = DBoneC[i].gameObject;
				VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider PBoneC;
				
				PBoneC = SelectedObject.AddComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider>();
				//shapeType
				PBoneC.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Capsule;	
				//RootTransform
				PBoneC.rootTransform = DBoneC[i].gameObject.transform;
				//Radius
				PBoneC.radius = DBoneC[i].m_Radius;
				//Height
				PBoneC.height = DBoneC[i].m_Height;
				//Center
				PBoneC.position = DBoneC[i].m_Center;
			}
			else if(DBoneC[i].m_Bound == DynamicBoneCollider.Bound.Inside || !IsInBoundColilderRemove)
			{
				GameObject SelectedObject = DBoneC[i].gameObject;
				VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider PBoneC;
				
				PBoneC = SelectedObject.AddComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider>();
				//shapeType
				PBoneC.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Plane;	
				//RootTransform
				PBoneC.rootTransform = DBoneC[i].gameObject.transform;
				//Position & Rotation
				PBoneC.position = DBoneC[i].m_Center +  new Vector3(0, 0, -DBoneC[i].m_Radius);
				PBoneC.rotation = Quaternion.Euler(90f, 0f, 0f);
			}
		}
	}
	private void RemoveDynamicBone()
	{
		GameObject GameObjectBase = (GameObject)BaseObject;

		DynamicBone[] DBone = GameObjectBase.GetComponentsInChildren<DynamicBone>();
			
		for(int i = 0; i < DBone.Length; i++)
		{
			DestroyImmediate(DBone[i]);
		}
	}
	private void RemoveDynamicColilder()
	{
		GameObject GameObjectBase = (GameObject)BaseObject;

		DynamicBoneCollider[] DBoneC = GameObjectBase.GetComponentsInChildren<DynamicBoneCollider>();
			
		for(int i = 0; i < DBoneC.Length; i++)
		{
			DestroyImmediate(DBoneC[i]);
		}
	}
}
