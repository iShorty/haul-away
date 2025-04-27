using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public struct LaunchData {
    public readonly Vector3 initialVelocity, target;
    public readonly float timeToTarget;

    public LaunchData(Vector3 initialVelocity, Vector3 target, float timeToTarget) {
        this.initialVelocity = initialVelocity;
        this.target = target;
        this.timeToTarget = timeToTarget;
    }
}


public static class GameUtils {

    #region Pathing Methods

    public static int BdLoop(int startingIndex, ref int step, int loopLength) {
        int localStep = step;
        while(localStep != 0) {
            if(localStep < 0 && startingIndex != 0) {
                startingIndex--;
                localStep++;
            }
            else if (localStep > 0 && startingIndex != loopLength - 1) {
                startingIndex++;
                localStep--;
            }
            if(startingIndex == 0 || startingIndex == loopLength - 1) {
                step = -step;
                localStep = -localStep;
            }
        }
        return startingIndex;
    }

    public static int CyclicLoop(int startingIndex, int step, int loopLength) {
        startingIndex += step;
        if (startingIndex < 0) {
            startingIndex = Mathf.Abs(startingIndex) % loopLength;
        }
        else if (startingIndex >= loopLength) {
            startingIndex = startingIndex % loopLength;
        }
        return startingIndex;
    }

    #endregion

    #region Collision Bump

    public static void Bump(Collision collision, Transform transform, Rigidbody rb, float value) {
        // Bounce the obj in the appropriate direction with the appropriate force. 
        Vector3 dir = (collision.GetContact(0).point - transform.position).normalized;
        dir = Vector3.Reflect(transform.forward, collision.GetContact(0).normal);
        rb.AddForceAtPosition(dir * value, collision.GetContact(0).point, ForceMode.VelocityChange);
    }

    public static void ApplyImpulse(Collision collision, Transform transform, float value) {
        // Bounce the obj in the appropriate direction with the appropriate force. 
        Vector3 dir = (collision.GetContact(0).point - transform.position).normalized;
        collision.rigidbody?.AddForceAtPosition(dir * value, collision.GetContact(0).point, ForceMode.Impulse);
    }

    #endregion

    #region Facing Direction
    
    public static float Dot(Vector3 target, Transform transform)
    {
        Vector3 other = target - transform.position;
        other.y = transform.position.y;
        other = other.normalized;
        return Vector3.Dot(transform.forward, other);
    }
    
    public static bool InFront(Vector3 target, Transform transform)
    {
        float dot = GameUtils.Dot(target, transform);
        if(dot > 0) return true;
        else return false;
    }

    public static float DotRight(Vector3 target, Transform transform)
    {
        Vector3 other = target - transform.position;
        other.y = transform.position.y;
        other = other.normalized;
        return Vector3.Dot(transform.right, other);
    }

    
    ///<summary>Tells me if target is on the right (if dotR > 0 is true) and on the left (else) of transform.</summary>
    public static bool DotR(Vector3 target, Transform transform) {
        float dotR = GameUtils.DotRight(target, transform);
        if(dotR > 0) return true;
        else return false;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask) {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        
        randomDirection.y = 0;
        randomDirection += origin;
        
        NavMeshHit navHit;
        
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }

    #endregion

    #region Pirate Boat Movement

    ///<summary>Called by enemies. Get a suitable offset position near the player boat within range and outside of minimum distance.</summary>
    public static Vector3 GetCombatRangeOffset(float attackRange, float minDist, bool? side = null) {
        Quaternion rot = Quaternion.AngleAxis(
            Random.Range(EnemyManager._forwardArcAngleMin, EnemyManager._forwardArcAngleMax),
            Vector3.up);
        Vector3 dir = rot * Vector3.forward;
        float scalar = (Random.Range(minDist, attackRange));
        Vector3 pos = dir * scalar;
        if(side == null) {
            pos.x *= Random.Range(0, 2) > 0 ? -1 : 1;
        }
        else if(side == false) {
            pos.x *= -1;
        }
        return pos;
    }

    // public static Vector3 PrepAttack(GameObject target, int iterationAhead, Transform transform)
    // {
    //     Vector3 position = Vector3.positiveInfinity;
    //     Vector3 targetSpeed = target.GetComponent<Rigidbody>().velocity;
    //     targetSpeed.y = 0;
    //     if (targetSpeed.magnitude < 1)
    //     {
    //         iterationAhead = 0;
    //     }
    //     else
    //     {
    //         iterationAhead = (int)((target.transform.position - transform.position).magnitude / targetSpeed.magnitude);
    //     }
    //     position = target.transform.position + targetSpeed * iterationAhead;

    //     // target position = randomised position around the predicted position based on targets current speed.
    //     if (targetOffset.x == Mathf.Infinity)
    //     {
    //         targetOffset = GameUtils.GetTargetOffset(AttackRadius);
    //         position += targetOffset;
    //     }

    //     return position;
    // }

    ///<summary>Returns the normal at the hit point on a mesh.</summary>
	public static Vector3 GetMeshColliderNormal(RaycastHit hit) {
        Mesh mesh = hit.collider.gameObject.GetComponent<MeshFilter>().mesh;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        // Extract local space normals of the triangle we hit
        Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];

        // interpolate using the barycentric coordinate of the hitpoint
        Vector3 baryCenter = hit.barycentricCoordinate;

        // Use barycentric coordinate to interpolate normal
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
        // normalize the interpolated normal
        interpolatedNormal = interpolatedNormal.normalized;

        // Transform local space normals to world space
        Transform hitTransform = hit.collider.transform;
        interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);

        return interpolatedNormal;
    }

    public static Vector3 GetOceanSurfaceNormal(Vector3 position, RaycastHit hit, float dist, LayerMask mask) {
        if (Physics.Raycast(position + Vector3.up * 25f, Vector3.down, out hit, dist, mask))
        {
            var normal = GetMeshColliderNormal(hit);
            return normal;
        }
        else
        {
            Debug.LogWarning("Can't find normal ");
            return Vector3.negativeInfinity;
        }
    }

    public static Vector3 ProjectOnContactPlane(Vector3 vector, Vector3 contactNormal) {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    /// <summary>Snaps a point to a suitable position on the navmesh.</summary>
    public static Vector3 GetNearestNavPos(Vector3 target, UnityEngine.AI.NavMeshHit hit, float maxDist, int layerMask = -1) {
        if (UnityEngine.AI.NavMesh.SamplePosition(target, out hit, maxDist, layerMask))
        {
            Vector3 pos = hit.position;
            hit = default;
            return pos;
        }
        else
        {
            // Debug.LogWarning("Getnearestnavpos sampled and found no valid navmesh pos. Aaah.");
            return Vector3.positiveInfinity;
        }
    }

    #endregion

    public static float PathLength(Vector3[] points) {
        float length = 0;

        if (points.Length <= 1)
        {
            return Mathf.Infinity;
        }

        for (int i = 0; i < points.Length; i++)
        {
            int nextIndex = i + 1;
            if (i + 1 < points.Length)
            {
                length += Vector3.Distance(points[i], points[i + 1]);
            }
        }

        return length;
    }

    #region Firing

    public static LaunchData CalculateLaunchData(Vector3 start, Vector3 target) {
        float h = Constants.For_Projectiles.H_VALUE;
        // float h = (start.y + target.y) * 0.5f;
        float displacementY = target.y - start.y;
        Vector3 displacementXZ = new Vector3(target.x - start.x, 0, target.z - start.z);
        float time = Mathf.Sqrt(-2 * h / Constants.For_Projectiles.GRAVITY) + Mathf.Sqrt(2 * (displacementY - h) / Constants.For_Projectiles.GRAVITY);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * Constants.For_Projectiles.GRAVITY * h);
        Vector3 velocityXZ = displacementXZ / time;

        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(Constants.For_Projectiles.GRAVITY), target, time);
    }

    // public static Vector3 GetTargetWithOffset(Vector3 target, float size = 1f) {
    // 	Vector3 pos = target + GetTargetOffset(target, size);
    // 	return pos;
    // }

    public static Vector3 GetTargetOffset(float size = 1f)
    {
        Vector3 pos = Random.insideUnitCircle * size;
        pos.z = pos.y;
        pos.y = 0;
        return pos;
    }

    public static void DrawPath(Vector3 start, LaunchData launchData)
    {
        // LaunchData launchData = CalculateLaunchData(start, target);
        Vector3 previousDrawPoint = start;

        int resolution = 30;
        for (int i = 1; i <= resolution; i++)
        {
            float simulationTime = i / (float)resolution * launchData.timeToTarget;
            Vector3 displacement = launchData.initialVelocity * simulationTime +
            Vector3.up * Constants.For_Projectiles.GRAVITY * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = start + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green, 0.1f);
            previousDrawPoint = drawPoint;
        }
    }

    #endregion


}

