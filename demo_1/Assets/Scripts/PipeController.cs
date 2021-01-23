using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeController : MonoBehaviour
{
    public List<Buttons> buttons = new List<Buttons>();
    public Grid myGrid;
    public List<Pipe> pipeTail = new List<Pipe>();
    public Pipe pipeHead;

    /// <summary>
    /// 当前指向的pipeTail下标 
    /// </summary>
    public int nowIndex = 0;
    /// <summary>
    /// 管道是否连通
    /// </summary>
    public bool open = false;

    private void Awake()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].pipeController = this;
        }
        
    }
    public void GetConnect(Pipe pipe1, Pipe pipe2)
    {
        Node node1 = myGrid.grid[pipe1.connectVector.x, pipe1.connectVector.y];
        Node node2 = myGrid.grid[pipe2.connectVector.x, pipe2.connectVector.y];
        node1.connectNodes.Add(node2);
        node2.connectNodes.Add(node1);
    }

    public void Disconnect(Pipe pipe1, Pipe pipe2)
    {
        Node node1 = myGrid.grid[pipe1.connectVector.x, pipe1.connectVector.y];
        Node node2 = myGrid.grid[pipe2.connectVector.x, pipe2.connectVector.y];
        node1.connectNodes.Remove(node2);
        node2.connectNodes.Remove(node1);
    }
}
