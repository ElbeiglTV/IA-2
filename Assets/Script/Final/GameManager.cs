using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Pathfinding pathfinding;

    public MyGrid grid;

    public AgentConfig boidConfig;
    public LayerMask FloorMask;
    public LayerMask NodeLayerMask;

    [InspectorName("MapLayerMask")] public LayerMask layerMask;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            if (Instance == this) return;
            Destroy(gameObject);
        }
    }

    public bool InLineOfSight(Vector3 start, Vector3 end)
    {
        var dir = end - start;

        return !Physics.Raycast(start, dir, dir.magnitude, layerMask);
    }

    public bool NodeInLineOfSight(Vector3 start, Vector3 end, Collider MyColider, Collider OtherColither)
    {
        var dir = end - start;
        if (Physics.Raycast(start, dir, dir.magnitude, layerMask))
        {
            return false;
        }
        else
        {
            if (Physics.Raycast(start, dir, out RaycastHit hit, dir.magnitude, NodeLayerMask))
            {
                return hit.collider == MyColider || hit.collider == OtherColither;
            }
            else
            {
                return true;
            }
        }
    }












}
[System.Serializable]
public class AgentConfig
{
    public List<SteeringAgent> Agents = new List<SteeringAgent>();

    public Transform Spawn;

    public float separationWeight = 1;

    public float separationRadius, viewRadius;

    public float evadeWeight = 1;

    public float arriveWeight = 1;

    public float obstacleWeight = 1;





}



