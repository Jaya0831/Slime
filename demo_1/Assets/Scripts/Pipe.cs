using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public Vector2Int locateVector;
    public Vector2Int connectVector;

    public GameObject myGrid;
    public PublicEnum.Direction toward = PublicEnum.Direction.none;

    private void Awake()
    {
        transform.SetPositionAndRotation(TransformVectorToPosition(), Quaternion.identity);//面板中的rotation的x,y,z和四元数中的x,y,z不对应
        transform.Rotate(TransformTowardToRotate());
    }
    private void Start()
    {
        PlaceAPipe();
    }

    /// <summary>
    /// 求Pipe的position
    /// </summary>
    /// <returns></returns>
    public Vector3 TransformVectorToPosition()
    {
        Vector3 offSet = new Vector3(0, 0, 0);
        switch (toward)
        {
            case PublicEnum.Direction.right:
                offSet = new Vector3(-0.5f, 0, locateVector.y - 0.5f);//0.5f:要在地面上面
                break;
            case PublicEnum.Direction.left:
                offSet = new Vector3(0.5f, 0, locateVector.y - 0.5f);
                break;
            case PublicEnum.Direction.up:
                offSet = new Vector3(0, -0.5f, locateVector.y - 0.5f);
                break;
            case PublicEnum.Direction.down:
                offSet = new Vector3(0, 0.5f, locateVector.y - 0.5f);
                break;
        }
        return myGrid.GetComponent<Grid>().FromVectorToPosition(locateVector) + offSet;
    }

    /// <summary>
    /// 获取pipe要旋转的角度
    /// </summary>
    /// <returns></returns>
    public Vector3 TransformTowardToRotate()//rotation指面板中的rotation不是四元数
    {
        switch (toward)
        {
            case PublicEnum.Direction.right:
                return new Vector3(0, 0, 0);
            case PublicEnum.Direction.left:
                return new Vector3(0, 180, 0);
            case PublicEnum.Direction.up:
                return new Vector3(0, 0, 90);
            case PublicEnum.Direction.down:
                return new Vector3(0, 0, 270);
        }
        return new Vector3(0,0,0);
    }

    /// <summary>
    /// 放置管道
    /// </summary>
    public void PlaceAPipe()
    {
        myGrid.GetComponent<Grid>().grid[locateVector.x, locateVector.y].PlaceAnObject(true, false);
    }

    /// <summary>
    /// 移除管道
    /// </summary>
    public void RemoveAPipe()
    {
        myGrid.GetComponent<Grid>().grid[locateVector.x, locateVector.y].RemoveAnObject(true, false);
    }

    /// <summary>
    /// 平移管道
    /// </summary>
    public void TranslatePipe(PublicEnum.Direction direction)//HACK:对走出地图的情况加以限制
    {
        Vector2Int vector = new Vector2Int(0, 0);
        switch (direction)
        {
            case PublicEnum.Direction.down:
                vector = new Vector2Int(0, -1);
                break;
            case PublicEnum.Direction.up:
                vector = new Vector2Int(0, 1);
                break;
            case PublicEnum.Direction.left:
                vector = new Vector2Int(-1, 0);
                break;
            case PublicEnum.Direction.right:
                vector = new Vector2Int(1, 0);
                break;            
        }
        locateVector += vector;
        connectVector += vector;
    }

    /// <summary>
    /// 旋转管道 
    /// </summary>
    public void Rotate()
    {
        //TODO
    }

    public void ShowBeAbandoned()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
    }

    public void ShowBeChosen()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    public void ShowIsConnected()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 0.4f, 1);
    }

    public void ShowDisConnect()//和chose是一样的
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 11);
    }
}
