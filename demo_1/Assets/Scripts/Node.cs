using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public Vector3 nodePosition;
    public int nodeI, nodeJ;

    /// <summary>
    /// 父节点
    /// </summary>
    public Node parent = null;
    /// <summary>
    /// 总代价
    /// </summary>
    //public int F = 0;
    /// <summary>
    /// 从起点到当前位置的代价
    /// </summary>
    //public int G = 0;
    /// <summary>
    /// 从当前位置到终点的估算代价
    /// </summary>
    //public int H = 0;
    /// <summary>
    /// 状态：undiscovered，open，close,obstacle(障碍物）
    /// </summary>
    //public enum Status { undiscovered, close, open, obstacle };

    public float mol;

    //public Status status = Status.undiscovered;

    public Node(Vector3 position, int i, int j)
    {
        this.nodePosition = position;
        this.nodeI = i;
        this.nodeJ = j;
    }
}
