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
    ///
    public Node[,] grid;
    /// <summary>
    /// 两个方向上的网格数
    /// </summary>
    ///
    public int x_node, y_node;
    /// <summary>
    /// 网格的左下角
    /// </summary>
    ///
    public Vector3 gridStart;

    /// <summary>
    /// 开放列表
    /// </summary>
    ///
    //public List<Node> openList;

    /// <summary>
    /// 地图
    /// </summary>
    ///
    public Tilemap tilemap;
    /// <summary>
    /// 可走地形对应的贴图
    /// </summary>
    ///
    //public Sprite sprite_open;
    /// <summary>
    /// 别走比分对应的贴图
    /// </summary>
    ///
    //public Sprite sprite_obstacle;

    private void Awake()
    {
        gridStart = this.transform.position - new Vector3(x / 2, y / 2, 1);
        x_node = Mathf.RoundToInt(x / grid_x);
        y_node = Mathf.RoundToInt(y / grid_y);
        grid = new Node[x_node, y_node];
        CreateTheGrid();
        //TransformTilemapToGrid(tilemap, -5, 4, -5, 4);
        Debug.Log(FromPositionToVector(new Vector3(4.5f, 4.5f, 1)));
        Debug.Log(FromPositionToVector(new Vector3(4, 4, 1)));
        Debug.Log(FromPositionToVector(new Vector3(3.5f, 3.5f, 1)));

    }
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
                grid[i, j] = new Node(nodePosition, i, j);
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

}


