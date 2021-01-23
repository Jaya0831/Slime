using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Grid : MonoBehaviour
{
    /*
 三种坐标：
    1，世界坐标，以相机为原点
    2，tilemap内定义坐标（第一象限最左下角的一个格子为原点）
    3，Grid类内定义的原点：整个网格最左下角的格子为原点
 */
    // 目前无论x或y的值是奇数还是偶数，表格都是居中的，也就是说当x，y值为奇数时，和tilemap不匹配

    /// <summary>
    /// 每个格子的长和宽
    /// </summary>
    /// 
    public float grid_x = 1f, grid_y = 1f;
    /// <summary>
    /// 网格的长和宽
    /// </summary>
    ///
    public float x = 10f, y = 10f;
    /// <summary>
    /// 网格
    /// </summary>
    public Node[,] grid;
    /// <summary>
    /// 两个方向上的网格数
    /// </summary>
    public int x_node, y_node;
    /// <summary>
    /// 网格的左下角
    /// </summary>
    public Vector3 gridStart;

    /// <summary>
    /// 开放列表
    /// </summary>
    public List<Node> openList;

    /// <summary>
    /// 地图
    /// </summary>
    public GameObject Map;
    //public Tilemap tilemap;
    //public Tilemap solidMap;
    //public Tilemap liquidMap;

    /// <summary>
    /// PublicEnum
    /// </summary>
    public GameObject myEnum;
    /// <summary>
    /// 一个格子上贴图的父物体
    /// </summary>
    public GameObject nodeSprites;


    /// <summary>
    /// 可走地形对应的贴图
    /// </summary>
    //public Sprite sprite_open;
    /// <summary>
    /// 别走比分对应的贴图
    /// </summary>
    //public Sprite sprite_obstacle;

    public int level;



    private void Awake()
    {
        gridStart = this.transform.position - new Vector3(x / 2, y / 2, 1);
        x_node = Mathf.RoundToInt(x / grid_x);
        y_node = Mathf.RoundToInt(y / grid_y);
        grid = new Node[x_node, y_node];
        CreateTheGrid();
        //TransformTilemapToGrid(tilemap, -5, 4, -5, 4);
        /*
        List<Node> temp = new List<Node>();
        temp.Add(grid[4, 3]);
        temp.Add(grid[4, 6]);
        temp.Add(grid[4, 4]);
        temp.Add(grid[4, 5]);
        connectNodes.Add(temp);
        List<Node> temp_ = new List<Node>();
        temp.Add(grid[6, 3]);
        temp.Add(grid[6, 6]);
        temp.Add(grid[6, 4]);
        temp.Add(grid[6, 5]);
        //connectNodes.Add(temp_);
        for (int i = 0; i < y_node; i++)
        {
            grid[8, i].hasWall[0] = true;
        }
        */
        Display();
    }






    //网格化相关内容
    /// <summary>
    /// 网格化
    /// </summary>
    private void CreateTheGrid()
    {
        for (int i = 0; i < x_node; i++)
        {
            for (int j = 0; j < y_node; j++)
            {
                Vector3 nodePosition = gridStart + Vector3.right * i + Vector3.right * 0.5f + Vector3.up * j + Vector3.up * 0.5f;
                grid[i, j] = new Node(nodePosition, i, j, this);               
            }
        }
        if (level == 1) 
        {
            List<List<int>> x_index = new List<List<int>>();
            List<List<int>> y_index = new List<List<int>>();
            List<int> stone3x = new List<int> { 4, 5, 6, 7, 8, 8, 8, 8, 8, 7, 6, 5, 4, 10, 10, 10, 10, 10, 10, 10 };
            List<int> stone3y = new List<int> { 5, 5, 5, 5, 5, 4, 3, 2, 1, 1, 1, 1, 1, 6, 5, 4, 3, 2, 1, 0 };
            List<int> stone4x = new List<int> { 1, 1, 1, 1, 1, 1, 1, 3, 4, 3, 3, 3, 3, 3, 3, 4 };
            List<int> stone4y = new List<int> { 6, 5, 4, 3, 2, 1, 0, 6, 6, 5, 4, 3, 2, 1, 0, 0 };
            List<int> water4x = new List<int> { 2, 2, 2, 2, 2, 2, 2 };
            List<int> water4y = new List<int> { 6, 5, 4, 3, 2, 1, 0 };
            List<int> lava3x = new List<int> { 5, 6, 7, 8, 9, 9, 9, 9, 9, 9, 9, 8, 7, 6, 5 };
            List<int> lava3y = new List<int> { 6, 6, 6, 6, 6, 5, 4, 3, 2, 1, 0, 0, 0, 0, 0 };
            x_index.Add(stone3x);
            x_index.Add(stone4x);
            x_index.Add(water4x);
            x_index.Add(lava3x);

            y_index.Add(stone3y);
            y_index.Add(stone4y);
            y_index.Add(water4y);
            y_index.Add(lava3y);

            List<float> nums = new List<float> { 3, 4, 4, 3 };
            for (int k = 0; k < x_index.Count; k++)
            {
                if (k < 2)
                {
                    for (int p = 0; p < x_index[k].Count; p++)
                    {
                        grid[x_index[k][p], y_index[k][p]].AddASolid(PublicEnum.SolidType.stone, nums[k]);
                    }
                }
                else if (k == 2)
                {
                    for (int p = 0; p < x_index[k].Count; p++)
                    {
                        grid[x_index[k][p], y_index[k][p]].AddALiquid(PublicEnum.LiquidType.water, nums[k]);
                    }
                }
                else if (k == 3) 
                {
                    for (int p = 0; p < x_index[k].Count; p++)
                    {
                        grid[x_index[k][p], y_index[k][p]].AddALiquid(PublicEnum.LiquidType.lava, nums[k]);
                    }
                }
            }
            //grid[5, 0].AddALiquid(PublicEnum.LiquidType.water, 1);
        }
        else if (level == 2) 
        {
            List<List<int>> x_index = new List<List<int>>();
            List<List<int>> y_index = new List<List<int>>();
            List<int> stone10x = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 9, 1, 2, 3, 4, 6, 7, 8, 9 };
            List<int> stone10y = new List<int> { 9, 9, 9, 9, 9, 9, 9, 9, 9, 8, 8, 7, 7, 7, 7, 7, 7, 7, 7 };
            List<int> stone3x = new List<int> { 0, 0, 0 };
            List<int> stone3y = new List<int> { 5, 4, 3 };
            List<int> stone4x = new List<int> { 2, 2, 1, 2 };
            List<int> stone4y = new List<int> { 5, 4, 3, 3 };
            List<int> stone5x = new List<int> { 4, 4, 3, 4 };
            List<int> stone5y = new List<int> { 5, 4, 3, 3 };
            List<int> stone6x = new List<int> { 6, 6, 5, 6 };
            List<int> stone6y = new List<int> { 5, 4, 3, 3 };
            List<int> stone7x = new List<int> { 8, 8, 7, 8 };
            List<int> stone7y = new List<int> { 5, 4, 3, 3 };
            List<int> stone8x = new List<int> { 10, 10, 9, 10 };
            List<int> stone8y = new List<int> { 5, 4, 3, 3 };
            List<int> lava10x = new List<int> { 2, 3, 4, 5, 6, 7, 8 };
            List<int> lava10y = new List<int> { 8, 8, 8, 8, 8, 8, 8 };
            x_index.Add(stone10x);
            x_index.Add(stone3x);
            x_index.Add(stone4x);
            x_index.Add(stone5x);
            x_index.Add(stone6x);
            x_index.Add(stone7x);
            x_index.Add(stone8x);
            x_index.Add(lava10x);

            y_index.Add(stone10y);
            y_index.Add(stone3y);
            y_index.Add(stone4y);
            y_index.Add(stone5y);
            y_index.Add(stone6y);
            y_index.Add(stone7y);
            y_index.Add(stone8y);
            y_index.Add(lava10y);

            List<float> nums = new List<float> { 10, 3, 4, 5, 6, 7, 8, 10 };
            for (int k = 0; k < x_index.Count; k++)
            {
                if (k < 7)
                {
                    for (int p = 0; p < x_index[k].Count; p++)
                    {
                        grid[x_index[k][p], y_index[k][p]].AddASolid(PublicEnum.SolidType.stone, nums[k]);
                    }
                }
                else
                {
                    for (int p = 0; p < x_index[k].Count; p++)
                    {
                        grid[x_index[k][p], y_index[k][p]].AddALiquid(PublicEnum.LiquidType.lava, nums[k]);
                    }
                }
            }
        }
        
    }

    /// <summary>
    /// 将世界位置转化为grid坐标
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector2Int FromPositionToVector(Vector3 position)
    {
        int i = (int)((position.x - gridStart.x) / grid_x);
        int j = (int)((position.y - gridStart.y) / grid_y);
        // 下面的四个if是为了确保smooth中的边界状态成立（就是刚好在边缘线上的坐标转化）如果之后要改动超出范围的处理规则，记得保证边缘状况！！！
        if (i >= x_node) i = x_node - 1;
        if (i < 0) i = 0;
        if (j >= y_node) j = y_node - 1;
        if (j < 0) j = 0;
        return new Vector2Int(i, j);
    }
    /// <summary>
    /// 将grid坐标转化为世界坐标 
    /// </summary>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public Vector3 FromVectorToPosition(Vector2Int vector2)
    {
        float position_x = gridStart.x + grid_x * (vector2.x + 0.5f);
        float position_y = gridStart.y + grid_y * (vector2.y + 0.5f);
        return new Vector3(position_x, position_y, 1);
    }

    /// <summary>
    /// 画网格
    /// </summary>
    private void OnDrawGizmos()
    {
        //Awake函数中代码
        gridStart = this.transform.position - new Vector3(x / 2, y / 2, 1);
        x_node = Mathf.RoundToInt(x / grid_x);
        y_node = Mathf.RoundToInt(y / grid_y);
        grid = new Node[x_node, y_node];
        CreateTheGrid();
        //画网格
        Gizmos.DrawWireCube(this.transform.position, new Vector3(x, y));
        if (grid == null) return;
        Gizmos.color = new Color(1, 1, 1, 0.3f);
        for (int i = 0; i < x_node; i++)
        {
            for (int j = 0; j < y_node; j++)
            {
                Gizmos.DrawWireCube(grid[i, j].nodePosition, new Vector3(grid_x, grid_y));
            }
        }
    }

    /// <summary>
    /// 渲染整个地图
    /// </summary>
    public void Display()
    {
        for (int i = 0; i < x_node; i++)
        {
            for (int j = 0; j < y_node; j++)
            {
                DisplayNode(grid[i, j], i, j); 
            }
        }
    }
    /// <summary>
    /// 渲染一个node
    /// </summary>
    /// <param name="node"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void DisplayNode(Node node, int x, int y)
    {
        int highTemp = 0;
        float offSet = 0.05f;
        float highStart = -0.475f;
        float planeStart = 0f;
        Vector3 positionTemp = FromVectorToPosition(new Vector2Int(x, y)) + new Vector3(0, 0, y);
        node.nodeObject = Instantiate(nodeSprites, positionTemp, Quaternion.identity, Map.transform);
        for (int i = 0; i < node.solidTypes.Count; i++)
        {
            for (int j = 0; j < Mathf.CeilToInt(node.solidNums[i]); j++)
            {
                Instantiate(myEnum.GetComponent<PublicEnum>().SolidHighSprites[(int)node.solidTypes[i]], positionTemp + new Vector3(0, highStart + offSet * highTemp, 0), Quaternion.identity, node.nodeObject.transform);
                highTemp++;
            }
        }
        for (int i = 0; i < node.liquidTypes.Count; i++)
        {
            for (int j = 0; j < Mathf.CeilToInt(node.liquidNums[i]); j++)
            {
                Instantiate(myEnum.GetComponent<PublicEnum>().LiquidHighSprites[(int)node.liquidTypes[i]], positionTemp + new Vector3(0, highStart + offSet * highTemp, 0), Quaternion.identity, node.nodeObject.transform);
                highTemp++;
            }
        }
        if (node.liquidTypes.Count != 0)
        {
            Instantiate(myEnum.GetComponent<PublicEnum>().LiquidPlaneSprites[(int)node.liquidTypes[node.liquidTypes.Count - 1]], positionTemp + new Vector3(0, planeStart + offSet * highTemp, 0), Quaternion.identity, node.nodeObject.transform);
        }
        else if (node.solidTypes.Count != 0) 
        {
            Instantiate(myEnum.GetComponent<PublicEnum>().SolidPlaneSprites[(int)node.solidTypes[node.solidTypes.Count - 1]], positionTemp + new Vector3(0, planeStart + offSet * highTemp, 0), Quaternion.identity, node.nodeObject.transform);
        }
        else
        {
            GameObject gameObject=Instantiate(myEnum.GetComponent<PublicEnum>().groundSprite, positionTemp, Quaternion.identity, node.nodeObject.transform);
        }
    }

    /// <summary>
    /// 清空格子 
    /// </summary>
    /// <param name="node"></param>
    private void DeleteNode(Node node)
    {
        GameObject.Destroy(node.nodeObject);
    }
    /// <summary>
    /// 清空地图
    /// </summary>
    public void DeleteMap()
    {
        for (int i = 0; i < x_node; i++)
        {
            for (int j = 0; j < y_node; j++)
            {
                DeleteNode(grid[i, j]);
            }
        }
        
    }

}


