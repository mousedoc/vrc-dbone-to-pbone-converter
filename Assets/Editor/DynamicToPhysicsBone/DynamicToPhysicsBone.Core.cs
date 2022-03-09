using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DynamicToPhysicsBone
{

    public class ConvertOption
    {
    }

    public class Core 
    {

        private GameObject targetObject; 

        public void Convert(GameObject targetObject)
        {
            if(targetObject == null)
            {
                Debug.LogError("Target object is empty");
                return;
            }

            this.targetObject = targetObject;

        }

        private void ProcessConvert()
        {
            var dBones = targetObject.GetComponentsInChildren<DynamicBone>(true);
            var dBoneColliders = targetObject.GetComponentsInChildren<DynamicBoneCollider>(true);

            RemoveDynamicBone();
            RemoveDynamicBoneCollider();

            EditorUtility.SetDirty(targetObject);
        }

        private void RemoveDynamicBone()
        {
            
        }

        private void RemoveDynamicBoneCollider()
        {

            var boneCollider = targetObject
        }

    }

}
