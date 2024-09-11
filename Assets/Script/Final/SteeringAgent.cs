using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    protected Vector3 _velocity;
    [SerializeField] protected float _maxForce;
    [SerializeField] protected float _maxSpeed;
    [SerializeField] protected LayerMask _obstacle;
    [SerializeField] protected float _viewRadius;

    public void Move()
    {
        transform.position += _velocity * Time.deltaTime;
        if (_velocity != Vector3.zero) transform.right = _velocity;
    }

    public void AddForce(Vector3 force)
    {
        _velocity += force;
    }

    public Vector3 CalculateSteering(Vector3 desired)
    {
        return Vector3.ClampMagnitude(desired - _velocity, _maxForce * Time.deltaTime);
    }

    public bool HasToUseOA()
    {
        Vector3 avoidanceObs = ObstacleAvoidance();
        AddForce(avoidanceObs);
        return avoidanceObs != Vector3.zero;
    }

    public Vector3 Seek(Vector3 targetPos)
    {
        return Seek(targetPos, _maxSpeed);
    }

    public Vector3 Seek(Vector3 targetPos, float speed)
    {
        Vector3 desired = (targetPos - transform.position).normalized * speed;
        return CalculateSteering(desired);
    }

    public Vector3 Flee(Vector3 fleePos) => -Seek(fleePos);

    public Vector3 Arrive(Vector3 targetPos)
    {
        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist > GameManager.Instance.boidConfig.viewRadius) return Seek(targetPos);
        return Seek(targetPos, _maxSpeed * (dist / GameManager.Instance.boidConfig.viewRadius));
    }

    public Vector3 Pursuit(SteeringAgent targetAgent)
    {
        Vector3 futurePos = targetAgent.transform.position + targetAgent._velocity;
        return Seek(futurePos);
    }
    public Vector3 PrePursuit(SteeringAgent targetAgent)
    {
        Vector3 futurePos = targetAgent.transform.position + targetAgent._velocity;
        return futurePos;
    }

    public Vector3 Evade(SteeringAgent targetAgent) => -Pursuit(targetAgent);

    public Vector3 Spacing(List<SteeringAgent> agents, float _radius)
    {
        Vector3 desired = Vector3.zero;
        foreach (var agent in agents)
        {
            if (agent == this) continue;
            Vector3 dist = agent.transform.position - transform.position;
            if (dist.sqrMagnitude > _radius * _radius) continue;
            desired += dist;
        }
        if (desired == Vector3.zero) return desired;
        return CalculateSteering(-desired.normalized * _maxSpeed);
    }

    public Vector3 Alignment(List<SteeringAgent> agents, float _radius)
    {
        Vector3 desired = Vector3.zero;
        int boidCount = 0;
        foreach (var agent in agents)
        {
            Vector3 dist = agent.transform.position - transform.position;
            if (dist.sqrMagnitude > _radius * _radius) continue;
            desired += agent._velocity;
            boidCount++;
        }
        return CalculateSteering((desired /= boidCount).normalized * _maxSpeed);
    }

    public Vector3 Cohesion(List<SteeringAgent> agents, float _radius)
    {
        Vector3 desired = Vector3.zero;
        int boidCount = 0;
        foreach (var agent in agents)
        {
            if (agent == this) continue;
            Vector3 dist = agent.transform.position - transform.position;
            if (dist.sqrMagnitude > _radius * _radius) continue;
            desired += agent.transform.position;
            boidCount++;
        }
        if (boidCount <= 0) return desired;
        return CalculateSteering((desired /= boidCount).normalized * _maxSpeed);
    }

    public Vector3 ObstacleAvoidance()
    {
        Ray ray = new Ray(transform.position + transform.up * 0.5f,transform.right);
        Ray ray2 = new Ray(transform.position - transform.up * 0.5f,transform.right);
        if (Physics.SphereCast(ray, 0.5f, _viewRadius, _obstacle)) return Seek(transform.position + transform.up);
        else if (Physics.SphereCast(ray2, 0.5f, _viewRadius, _obstacle)) return Seek(transform.position - transform.up);
        return Vector3.zero;
    }

    public void ResetPosition()
    {
        transform.position = Vector3.zero;
    }
    public void ResetVelocity()
    {
        _velocity = Vector3.zero;
    }
}