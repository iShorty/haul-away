using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.SceneManagement;

//Holds functions for rendering the trajectory path the cannon is going to show when the player is using the INACTIVE CANNON state
public partial class MultiUseStation
{
    #region Exposed
    [Header("===== TRAJECTORY =====")]
    [SerializeField]
    Transform _trajectorySphereHolder = default;

    [SerializeField]
    Transform _reticleModelTransform = default;

    [SerializeField]
    MeshRenderer[] _reticleMeshRenderers = default;
    #endregion

    //Hidden
    float _trajectoryAnimTimer = default;


    #region Properties

    #endregion

    #region Initialize
    void Trajectory_Awake()
    {
#if UNITY_EDITOR
        Debug.Assert(Info.Iteration > 3, $"The multistation info {Info}'s iteration for trajectory must be more than 3!", Info);
        Debug.Assert(_reticleMeshRenderers.Length > 0, $"The multistation reticlemeshrenders arent assigned!", this);
        Debug.Assert(_reticleModelTransform, $"The multistation reticle model isnt assigned!", this);
#endif

        //These are the diff sizes of spheres for the trajectory spheres
        Vector3 bigSphereSize = Vector3.one * Info.BigSphereSize;
        Vector3 smallSphereSize = Vector3.one * Info.SmallSphereSize;

        //Spawn all trajectory spheres and assign them as child under sphereholder
        for (int i = 0; i < Info.Iteration - 1; i++)
        {
            GameObject sphere = Instantiate(Info.TrajectoryPrefab);
            sphere.transform.localScale = i % Info.BigSphereInterval == 0 ? bigSphereSize : smallSphereSize;
            sphere.transform.SetParent(_trajectorySphereHolder);
        }

        //Spawn reticle gameobject and set it as last sibling child under sphereholder
        // GameObject reticle = Instantiate(Info.TrajectoryReticlePrefab);
        // reticle.transform.SetParent(_trajectorySphereHolder);
        _reticleModelTransform.transform.SetAsLastSibling();

        Trajectory_ToggleActiveAll(false);
        CANNON_Trajectory_ChangeLineColor(false);
    }

    void Trajectory_OnEnable()
    {
        _trajectoryAnimTimer = Info.TrajectoryAnimationInterval;
    }

    #endregion

    #region Toggle
    ///<Summary>Toggles all of the trajectory spheres and reticle's gameobjects</Summary>
    void Trajectory_ToggleActiveAll(bool state)
    {
        // _trajectorySphereHolder.gameObject.SetActive(state);
        _trajectoryAnimTimer = Info.TrajectoryAnimationInterval;
        Trajectory_ToggleActiveSpheres(state);
        Trajectory_ToggleActiveReticle(state);
    }

    ///<Summary>Toggles all of the trajectory spheres' gameobjects</Summary>
    void Trajectory_ToggleActiveSpheres(bool state)
    {
        int spheresCount = Info.Iteration - 1;
        for (int i = 0; i < spheresCount; i++)
        {
            _trajectorySphereHolder.GetChild(i).gameObject.SetActive(state);
        }
    }

    ///<Summary>Toggles the trajectory reticle's gameobject</Summary>
    void Trajectory_ToggleActiveReticle(bool state)
    {
        _reticleModelTransform.gameObject.SetActive(state);
    }


    ///<Summary>Toggles the trajectory reticle material color1</Summary>
    void Trajectory_ToggleReticleModelColor(bool isCannon)
    {
        Color color = isCannon ? Info.CannonReticleColor : Info.GrapplingReticleColor;
        foreach (var mr in _reticleMeshRenderers)
        {
            mr.material.SetColor(Constants.MATERIAL_DEFAULT_PROPERTYNAME_COLOR, color);
        }
    }

    #endregion


    ///<Summary>Updates the trajectory spheres' positions using the cannon's trajectory and animates it. Must be called after the _launchData has been updated. </Summary>
    void Trajectory_CANNON_Update()
    {
        Trajectory_CANNON_UpdateLinePositions();
        Trajectory_UpdateAnimation();
    }

    ///<Summary>Updates the reticle's position only using the grappling's trajectory </Summary>
    void Trajectory_GRAPPLING_Update()
    {
        Transform t = _reticleModelTransform;
        t.position = _targetPoint;
    }

    ///<Summary>Changes the spheres' alpha or color when the cannon is cooling down or not </Summary>
    void CANNON_Trajectory_ChangeLineColor(bool cooldowning)
    {
        Color color = cooldowning ? Info.TrajectoryInActive : Info.TrajectoryActive;

        for (int i = 0; i < Info.Iteration - 1; i++)
        {
            UnityEngine.Transform sphere = _trajectorySphereHolder.GetChild(i);
            MeshRenderer mr = sphere.GetComponent<MeshRenderer>();
            Material material = mr.material;
            material.SetColor(Constants.MATERIAL_DEFAULT_PROPERTYNAME_COLOR, color);
        }

    }

    #region Private Methods
    ///<Summary>Animates only the trajectory sphere's sizes</Summary>
    private void Trajectory_UpdateAnimation()
    {
        if (_trajectoryAnimTimer > 0)
        {
            _trajectoryAnimTimer -= Time.deltaTime;
            return;
        }

        //First sphere take 2nd last sphere's localscale first (because last index is reticle)
        int secondLastIndex = Info.Iteration - 2;
        Vector3 secondLastSphereScale = _trajectorySphereHolder.GetChild(secondLastIndex).localScale;

        for (; secondLastIndex >= 1; secondLastIndex--)
        {
            //Change current sphere's scale to the previous sphere's scale
            Transform sphere = _trajectorySphereHolder.GetChild(secondLastIndex);
            _trajectorySphereHolder.GetChild(secondLastIndex).localScale = _trajectorySphereHolder.GetChild(secondLastIndex - 1).localScale;
        }

        //Finally, set the first sphere with the last sphere's scale
        _trajectorySphereHolder.GetChild(0).localScale = secondLastSphereScale;

        //Reset timer
        _trajectoryAnimTimer = Info.TrajectoryAnimationInterval;
    }

    ///<Summary>Updates the entire trajectory lines' sphere and reticle position</Summary>
    void Trajectory_CANNON_UpdateLinePositions()
    {
        for (int i = 1; i <= Info.Iteration; i++)
        {
            float simulationTime = i / (float)Info.Iteration * _launchData.timeToTarget;
            Vector3 displacement = _launchData.initialVelocity * simulationTime + Physics.gravity * simulationTime * simulationTime / 2f;
            Vector3 spherePos = _firePoint.position + displacement;

            //Set the sphereposition to trajectoryspheres
            Transform sphere = _trajectorySphereHolder.GetChild(i - 1);
            sphere.position = spherePos;
        }
    }
    #endregion

}
