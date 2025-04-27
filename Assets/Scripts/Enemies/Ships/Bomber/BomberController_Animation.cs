// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public partial class BomberController
// {
//     #region ------- Const ----------
//     const float ROTATIONSPEED = 5;
//     #endregion

//     [Header("===== Animation =====")]
//     [SerializeField] Transform _standModel = default;
//     [SerializeField] Transform _barrelModel = default;
//     [SerializeField] Transform _trueForwardAxis = default;

//     [SerializeField]
//     Animator _anim = default;

//     #region Look Animation
//     Quaternion lookRot;
//     Vector3 rotationVector = default;

//     ///<Summary>Rotates the cannon towards a normalized direction in worldspace</Summary>
//     void Animation_LookTowards(Vector3 worldSpaceDirNormalized)
//     {
// #if UNITY_EDITOR
//         Debug.DrawRay(_standModel.position, worldSpaceDirNormalized, Color.yellow, Time.deltaTime);
// #endif
//         // Debug.Log(worldSpaceDir);
//         rotationVector = worldSpaceDirNormalized;

//         //============= ROTATE CANNON STAND Y AXIS ===============
//         rotationVector.y = 0;
//         //Get the rotation we want the base to look at
//         lookRot = Quaternion.LookRotation(rotationVector);
//         _standModel.transform.rotation = Quaternion.Slerp(_standModel.transform.rotation, lookRot, Time.deltaTime * ROTATIONSPEED);

//         //============= ROTATE CANNON BARREL Z AXIS ===============
//         //Since the _standModel is responsible for y axis, its forward axis will always be in the same general direction as the world direction. Thus, we should use trueForwardAxis 
//         rotationVector = _trueForwardAxis.InverseTransformDirection(worldSpaceDirNormalized);
//         rotationVector.x = 0;
//         rotationVector = _trueForwardAxis.TransformDirection(rotationVector);

//         //This always returns positive angle
//         float zAngle = Vector3.Angle(_trueForwardAxis.forward, rotationVector);

//         //We need to find a way to determine if this angle is +ve or -ve
//         //if rotationvector aims upwards, zAngle is negative else, the other way around (because model rotation)
//         zAngle = rotationVector.y > 0 ? -zAngle : zAngle;

// #if UNITY_EDITOR
//         Debug.DrawRay(_trueForwardAxis.position, rotationVector, Color.green, Time.deltaTime);
// #endif

//         rotationVector.y = 0;
//         rotationVector.x = Mathf.MoveTowards(_barrelModel.transform.localEulerAngles.x,zAngle,Time.deltaTime);

//         // rotationVector = Vector3.MoveTowards(_barrelModel.transform.localEulerAngles, rotationVector, Time.deltaTime *0.005f);
//         _barrelModel.transform.localEulerAngles = rotationVector;
//     }
//     #endregion

//     void Animation_FireCannon()
//     {
//         //Fire
//         _anim.SetTrigger(Constants.For_PlayerStations.MULTIPURPOSE_ANIMATION_PARAM_FIRE);
//         // SetTimer(Info.CannonFireDelay);
//     }
// }
