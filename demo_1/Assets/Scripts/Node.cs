using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public Vector3 nodePosition;
    public int nodeI, nodeJ;

    public GameObject nodeObject;

    public Grid myGrid;
    //寻路相关

    /// <summary>
    /// 父节点
    /// </summary>
    public Node parent = null;
    /// <summary>
    /// 总代价
    /// </summary>
    public int F = 0;
    /// <summary>
    /// 从起点到当前位置的代价
    /// </summary>
    public int G = 0;
    /// <summary>
    /// 从当前位置到终点的估算代价
    /// </summary>
    public int H = 0;
    /// <summary>
    /// 状态：undiscovered，open，close,obstacle(障碍物）
    /// </summary>
    public enum Status { undiscovered, close, open, obstacle };


    //属性相关
    /// <summary>
    /// 温度
    /// </summary>
    public float temperature;
    /// <summary>
    /// 电
    /// </summary>
    public bool electricity;



    //固体相关
    /// <summary>
    /// 固体总物质量
    /// </summary>
    public float solidmol = 0;
    public List<PublicEnum.SolidType> solidTypes = new List<PublicEnum.SolidType>();
    public List<float> solidNums = new List<float>();


    //液体相关
    /// <summary>
    /// 液体总物质量
    /// </summary>
    public float liquidmol = 0;
    public List<PublicEnum.LiquidType> liquidTypes = new List<PublicEnum.LiquidType>();//密度大的在下面
    public List<float> liquidNums = new List<float>();
    public List<float> liquidHeights = new List<float>();



    /// <summary>
    /// 是否还可以放置新的东西（slime，装置……），但是还可以通过液体
    /// </summary>
    public bool filled = false;

    /// <summary>
    /// slime是否可以通过
    /// </summary>
    public bool reachable = true;

    //扩散相关

    /// <summary>
    /// 不能承载溶液
    /// </summary>
    public bool hasObstacle = false;
    /// <summary>
    /// 格子每个边是否被堵住
    /// </summary>
    //public List<publicEnum.Direction> hasWall = new List<publicEnum.Direction>();
    public bool[] hasWall = { false, false, false, false };//左右上下
    //hack: 用固体高度取代haswall（haswall的地方固体高度拉满
    /// <summary>
    /// 和这个格子相连的格子
    /// </summary>
    public List<Node> connectNodes = new List<Node>();


    //public Status status = Status.undiscovered;

    

    /// <summary>
    /// 初始化格子
    /// </summary>
    /// <param name="position"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public Node(Vector3 position, int i, int j, Grid grid)
    {
        this.nodePosition = position;
        this.nodeI = i;
        this.nodeJ = j;
        this.myGrid = grid;
    }

    /// <summary>
    /// 更新固体
    /// </summary>
    /// <param name="solidType"></param>
    /// <param name="num"></param>
    public void UpdateASolid(PublicEnum.SolidType solidType, float num)
    {
        float tmp = num - solidNums[(int)solidType];
        solidmol += tmp;
        solidNums[(int)solidType] = num;
        for (int i = 0; i < liquidNums.Count; i++)
        {
            liquidHeights[i] += tmp;
        }
    }

    /// <summary>
    /// 删除固体
    /// </summary>
    /// <param name="solidType"></param>
    public void DeleteASolid(PublicEnum.SolidType solidType)
    {
        int index = solidTypes.FindIndex(c => c.Equals(solidType));
        float tmp = solidNums[index];
        solidTypes.RemoveAt(index);
        solidNums.RemoveAt(index);
        solidmol -= tmp;
        for (int i = 0; i < liquidNums.Count; i++)
        {
            liquidHeights[i] -= tmp;
        }
    }

    /// <summary>
    /// 在solid的最上层添加一层solid
    /// </summary>
    /// <param name="solidType"></param>
    /// <param name="num"></param>
    public void AddASolid(PublicEnum.SolidType solidType, float num)
    {
        solidTypes.Add(solidType);
        solidNums.Add(num);
        solidmol += num;
        for (int i = 0; i < liquidNums.Count; i++)
        {
            liquidHeights[i] += num;
        }
    }

    /// <summary>
    /// 更新液体
    /// </summary>
    /// <param name="liquidType"></param>
    /// <param name="num"></param>
    public void UpdateALiquid(PublicEnum.LiquidType liquidType, float num)
    {
        int index = liquidTypes.FindIndex(c => c.Equals(liquidType));
        float tmp = num - liquidNums[index];
        liquidmol += tmp;
        liquidNums[index] = num;
        for (int i = index + 1; i < liquidHeights.Count; i++)
        {
            liquidHeights[i] += tmp;
        }
    }

    /// <summary>
    /// 删除液体
    /// </summary>
    /// <param name="liquidType"></param>
    public void DeleteALiquid(PublicEnum.LiquidType liquidType)
    {
        int index = liquidTypes.FindIndex(c => c.Equals(liquidType));
        liquidTypes.RemoveAt(index);
        float tmp = liquidNums[index];
        liquidNums.RemoveAt(index);
        liquidHeights.RemoveAt(index);
        for (int i = index; i < liquidTypes.Count; i++)
        {
            liquidHeights[i] -= tmp;
            //Debug.Log(liquidTypes[i] + ".height" + liquidHeights[i]);
        }
        liquidmol -= tmp;
    }

    /// <summary>
    /// 在liquid的适当位置添加一层liquid（密度）
    /// </summary>
    /// <param name="liquidType"></param>
    /// <param name="num"></param>
    public void AddALiquid(PublicEnum.LiquidType liquidType, float num)
    {
        liquidmol += num;
        bool b = false;
        if (liquidTypes.Count == 0) 
        {
            liquidTypes.Add(liquidType);
            liquidNums.Add(num);
            liquidHeights.Add(solidmol);
            return;
        }
        for (int i = 0; i < liquidTypes.Count; i++)
        {
            if (!b && (int)liquidTypes[i] < (int)liquidType)
            {
                liquidTypes.Insert(i, liquidType);
                liquidNums.Insert(i, num);
                liquidHeights.Insert(i, i==0?solidmol:liquidHeights[i - 1] + liquidNums[i - 1]);
                b = true;
            }
            if (b)
            {
                liquidHeights[i] += num;
            }
        }
        if (!b)
        {
            float tmp = liquidHeights[liquidHeights.Count - 1] + liquidNums[liquidNums.Count - 1];
            liquidTypes.Add(liquidType);
            liquidNums.Add(num);
            liquidHeights.Add(tmp);
        }
    }

    /// <summary>
    /// 向一个格子转移一定量液体
    /// </summary>
    /// <param name="liquidType"></param>
    /// <param name="num"></param>
    public void FlowIn(PublicEnum.LiquidType liquidType, float num)
    {
        if (liquidTypes.Exists(c => c.Equals(liquidType)))
        {
            UpdateALiquid(liquidType, liquidNums[liquidTypes.FindIndex(c => c.Equals(liquidType))] + num);
        }
        else
        {
            AddALiquid(liquidType, num);
        }
    }

    /// <summary>
    /// 将一个格子的一定量某种液体平均转移到周围的格子上
    /// </summary>
    public void RemoveOnAverage(PublicEnum.LiquidType liquidType, float num, bool isTotal)//hack:其实不应该是平均分配，因为如果周围有一格solid比较高的话，溢出到那一格的液体会比较少
    {
        if (isTotal)
        {
            DeleteALiquid(liquidType);
        }
        else
        {
            UpdateALiquid(liquidType, liquidNums[liquidTypes.FindIndex(c => c.Equals(liquidType))] - num);
        }
        List<Vector2Int> vectors = new List<Vector2Int>();
        for (int p = 0; p < 4 + connectNodes.Count; p++)
        {
            int iNeighbor = nodeI;
            int jNeighbor = nodeJ;
            if (p < 4) 
            {
                switch (p)
                {
                    case 0:
                        iNeighbor = nodeI - 1;
                        break;
                    case 1:
                        iNeighbor = nodeI + 1;
                        break;
                    case 2:
                        jNeighbor = nodeJ + 1;
                        break;
                    case 3:
                        jNeighbor = nodeJ - 1;
                        break;

                }
                if (iNeighbor > -1 && iNeighbor < myGrid.x_node && jNeighbor > -1 && jNeighbor < myGrid.y_node &&
                            !myGrid.grid[iNeighbor, jNeighbor].hasObstacle &&
                            !myGrid.grid[nodeI, nodeJ].hasWall[p] && !myGrid.grid[iNeighbor, jNeighbor].hasWall[p % 2 == 1 ? p - 1 : p + 1])
                {
                    vectors.Add(new Vector2Int(iNeighbor, jNeighbor));
                }
            }
            else //处理被管道连接的情况
            {
                iNeighbor = myGrid.grid[nodeI, nodeJ].connectNodes[p - 4].nodeI;
                jNeighbor = myGrid.grid[nodeI, nodeJ].connectNodes[p - 4].nodeJ;
                if (!vectors.Exists(c => c.Equals(new Vector2Int(iNeighbor, jNeighbor))))
                {
                    vectors.Add(new Vector2Int(iNeighbor, jNeighbor));
                }
            }
            
        }
        for (int i = 0; i < vectors.Count; i++)
        {
            myGrid.grid[vectors[i].x, vectors[i].y].FlowIn(liquidType, num / vectors.Count);
        }
    }

    public void RemoveAllOnAverage()
    {
        for (int i = 0; i < liquidTypes.Count; i++)
        {
            RemoveOnAverage(liquidTypes[0], liquidNums[0], true);
        }
    }

    /// <summary>
    /// 放置一个障碍物
    /// </summary>
    public void PlaceAnObject(bool isObstacle, bool canReach)
    {
        RemoveAllOnAverage();
        hasObstacle = isObstacle ;
        filled = true;
        reachable = canReach ;
    }

    /// <summary>
    /// 移除一个障碍物
    /// </summary>
    public void RemoveAnObject(bool isObstacle, bool canReach)
    {
        if (isObstacle)
        {
            hasObstacle = false;
        }
        filled = false;
        if (!canReach)
        {
            reachable = true;
        }
    }

    /// <summary>
    /// 发生反应
    /// </summary>
    public void Reaction()
    {
        for (int i = 0; i < liquidTypes.Count; i++)
        {
            if (liquidTypes[i] == PublicEnum.LiquidType.lava && i + 1 < liquidTypes.Count && liquidTypes[i + 1] == PublicEnum.LiquidType.water)
            {
                if (liquidNums[i] > liquidNums[i + 1])
                {
                    Debug.Log("situation1");
                    AddASolid(PublicEnum.SolidType.stone, liquidNums[i + 1]);
                    UpdateALiquid(liquidTypes[i], liquidNums[i] - liquidNums[i + 1]);
                    DeleteALiquid(liquidTypes[i + 1]);
                }
                else
                {
                    Debug.Log("situation2");
                    AddASolid(PublicEnum.SolidType.stone, liquidNums[i]);
                    UpdateALiquid(liquidTypes[i + 1], liquidNums[i + 1] - liquidNums[i]);
                    DeleteALiquid(liquidTypes[i]);
                }
            }
        }
    }
}
