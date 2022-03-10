using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace DynamicToPhysicsBone
{
    public class ConvertOption
    {
        // Grab & Pose
        public bool AllowGrab = true;
        public bool AllowPose = true;
        public float MaxStretch = 0f;
        public float GrabMovement = 0.5f;

        // Offset
        public float PullOffset = 20f;
        public float SpringOffset = 0f;
        public float ImmobileOffset = 0.25f;

        // Clamp
        public float MaxAngle = 30f;
    }

    public class Core
    {
        private GameObject targetObject;
        private ConvertOption option;

        public void Convert(GameObject targetObject, ConvertOption option)
        {
            if (targetObject == null)
            {
                Debug.LogError("Target object is empty");
                return;
            }

            this.targetObject = targetObject;
            this.option = option;

            ProcessConvert();
        }

        private void ProcessConvert()
        {
            var dBones = targetObject.GetComponentsInChildren<DynamicBone>(true);
            var dBoneColliders = targetObject.GetComponentsInChildren<DynamicBoneCollider>(true);

            AddPhysicsBoneCollider(dBoneColliders);
            AddPhysicsBone(dBones);

            RemoveDynamicBone(dBones);
            RemoveDynamicBoneCollider(dBoneColliders);

            EditorUtility.SetDirty(targetObject);
        }

        private void AddPhysicsBoneCollider(IEnumerable<DynamicBoneCollider> dBoneColliders)
        {
            foreach (var dBoneCollider in dBoneColliders)
            {
                if (dBoneCollider.m_Bound == DynamicBoneColliderBase.Bound.Outside)
                {
                    var dBoneColliderObj = dBoneCollider.gameObject;
                    var pBoneCollider = dBoneColliderObj.AddComponent<VRCPhysBoneCollider>();

                    // shapeType
                    pBoneCollider.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Capsule;
                    // RootTransform
                    pBoneCollider.rootTransform = dBoneCollider.gameObject.transform;
                    // Radius
                    pBoneCollider.radius = dBoneCollider.m_Radius;
                    // Height
                    pBoneCollider.height = dBoneCollider.m_Height;
                    // Center
                    pBoneCollider.position = dBoneCollider.m_Center;
                }
                /// Ignore insdie collider
                //else if (dBoneCollider.m_Bound == DynamicBoneCollider.Bound.Inside || !IsInBoundColilderRemove)
                //{
                //    GameObject SelectedObject = dBoneCollider.gameObject;
                //    VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider pBone;

                //    pBone = SelectedObject.AddComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider>();
                //    //shapeType
                //    pBone.shapeType = VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Plane;
                //    //RootTransform
                //    pBone.rootTransform = dBoneCollider.gameObject.transform;
                //    //Position & Rotation
                //    pBone.position = dBoneCollider.m_Center + new Vector3(0, 0, -dBoneCollider.m_Radius);
                //    pBone.rotation = Quaternion.Euler(90f, 0f, 0f);
                //}
            }
        }

        private void AddPhysicsBone(IEnumerable<DynamicBone> dBones)
        {
            foreach (var dBone in dBones)
            {
                var dBoneObject = dBone.gameObject;
                var pBone = dBoneObject.AddComponent<VRCPhysBone>();

                // RootBone
                pBone.rootTransform = dBone.m_Root;
                // Ignore Transform
                pBone.ignoreTransforms = dBone.m_Exclusions;
                // Radius
                pBone.radius = dBone.m_Radius;
                // RadiusCurve
                pBone.radiusCurve = dBone.m_RadiusDistrib;

                //DBone.damping ---> PBone.pull
                //pBone.pull = dBone.m_Elasticity * 0.8f + 0.2f;
                pBone.pull = Mathf.Clamp(dBone.m_Damping + option.PullOffset, 0f, 1f);
                pBone.pullCurve = dBone.m_DampingDistrib;

                // DBone.elasticity ---> PBone.spring
                //pBone.spring = dBone.m_Damping * 0.7f + 0.3f;
                pBone.spring = Mathf.Clamp(dBone.m_Elasticity + option.SpringOffset, 0f, 1f);
                pBone.springCurve = dBone.m_ElasticityDistrib;

                // DBone.inert ---> PBone.immobile
                //pBone.immobile = dBone.m_Inert + dBone.m_Stiffness;
                pBone.immobile = Mathf.Clamp(1f - dBone.m_Inert + option.ImmobileOffset, 0f, 1f);
                pBone.immobileCurve = dBone.m_InertDistrib;

                // Options
                pBone.allowGrabbing = option.AllowGrab;
                pBone.allowPosing = option.AllowPose;
                pBone.maxAngle = Mathf.Clamp(option.MaxAngle, 0f, 180f);
                pBone.maxStretch = Mathf.Clamp(option.MaxStretch, 0, 5f);
                pBone.grabMovement = Mathf.Clamp(option.GrabMovement, 0, 1f);

                foreach (var dBonesCollider in dBone.m_Colliders)
                {
                    if (dBonesCollider == null)
                        continue;

                    var pBoneCollider = dBonesCollider.GetComponents<VRCPhysBoneCollider>();
                    pBone.colliders.AddRange(pBoneCollider);
                }
            }
        }

        private void RemoveDynamicBone(IEnumerable<DynamicBone> dBones)
        {
            foreach (var dBone in dBones)
                Object.DestroyImmediate(dBone);
        }

        private void RemoveDynamicBoneCollider(IEnumerable<DynamicBoneCollider> dBoneColliders)
        {
            foreach (var dBoneCollider in dBoneColliders)
                Object.DestroyImmediate(dBoneCollider);
        }
    }
}