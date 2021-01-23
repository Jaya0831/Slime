using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    public PipeController pipeController;
    public int type = -1;//0:开关 1:左 2:右
    


    public Vector2Int vector;

    private void Start()
    {
        transform.SetPositionAndRotation(pipeController.myGrid.FromVectorToPosition(vector) + new Vector3(0, 0, vector.y - 0.5f), Quaternion.identity);//new Vector3(0, 0, vector.y - 0.5f):让button符合遮挡关系并覆盖地面 
        pipeController.myGrid.grid[vector.x, vector.y].PlaceAnObject(true, true);
        pipeController.pipeHead.ShowBeChosen();
        if (pipeController.pipeTail.Count > 0) 
        {
            pipeController.pipeTail[0].ShowBeChosen();
        }
        for (int i = 1; i < pipeController.pipeTail.Count; i++)
        {
            pipeController.pipeTail[i].ShowBeAbandoned();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)//todo:选中时加高亮
    {
        if (collision.gameObject.tag == "Slime") 
        {
            Debug.Log("in");
            if (type == 0)
            {
                if (pipeController.open)
                {
                    pipeController.Disconnect(pipeController.pipeHead, pipeController.pipeTail[pipeController.nowIndex]);
                    pipeController.pipeHead.ShowDisConnect();
                    pipeController.pipeTail[pipeController.nowIndex].ShowDisConnect();
                    pipeController.open = false;
                }
                else
                {
                    pipeController.GetConnect(pipeController.pipeHead, pipeController.pipeTail[pipeController.nowIndex]);
                    pipeController.pipeHead.ShowIsConnected();
                    pipeController.pipeTail[pipeController.nowIndex].ShowIsConnected();
                    pipeController.open = true;
                }
            }
            if (type == 1)
            {
                if (pipeController.nowIndex > 0) 
                {
                    if (pipeController.open)
                    {
                        pipeController.Disconnect(pipeController.pipeHead, pipeController.pipeTail[pipeController.nowIndex]);
                        pipeController.GetConnect(pipeController.pipeHead, pipeController.pipeTail[pipeController.nowIndex - 1]);
                        pipeController.pipeTail[pipeController.nowIndex - 1].ShowIsConnected();                        
                    }
                    else
                    {
                        pipeController.pipeTail[pipeController.nowIndex - 1].ShowBeChosen();

                    }
                    pipeController.pipeTail[pipeController.nowIndex].ShowBeAbandoned();
                    pipeController.nowIndex--;
                }
            }
            if (type == 2)
            {
                if (pipeController.nowIndex < pipeController.pipeTail.Count)
                {
                    if (pipeController.open)
                    {
                        pipeController.Disconnect(pipeController.pipeHead, pipeController.pipeTail[pipeController.nowIndex]);
                        pipeController.GetConnect(pipeController.pipeHead, pipeController.pipeTail[pipeController.nowIndex + 1]);
                        pipeController.pipeTail[pipeController.nowIndex + 1].ShowIsConnected();
                    }
                    else
                    {
                        pipeController.pipeTail[pipeController.nowIndex + 1].ShowBeChosen();

                    }
                    pipeController.pipeTail[pipeController.nowIndex].ShowBeAbandoned();
                    pipeController.nowIndex++;
                }       
            }
        }
    }
}
