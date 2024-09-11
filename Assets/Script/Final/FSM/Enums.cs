using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{
    public enum LeaderStateID
    {
        Idle,PathFind,FollowPath
    }
    public enum BoidStateID
    {
        LeaderFloking,PathFindToLead,PathfindToBase,FollowPath,Escape,InBase
    }
}
