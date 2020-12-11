using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridManager : MonoBehaviour
{
    public GameObject grid;
    // 0~0.5,0.5~1.5...
    public Sprite[] gridSprites_demo;
    public TileBase[] tileBases_demo;
    public int numOfSprite;

    // Start is called before the first frame update
    void Start()
    {
        numOfSprite = gridSprites_demo.Length;
        LoadGridMessage(grid.GetComponent<Grid>().tilemap, grid.GetComponent<Grid>()); //hack：for test
    }

    private int timer=0;
    // Update is called once per frame
    void Update()
    {
        if (timer == 120) 
        {
            timer = 0;
            Spread(grid.GetComponent<Grid>().tilemap, grid.GetComponent<Grid>()); 
        }
        timer++;
    }


    /// <summary>
    /// 根据sprite加载地图信息（仅适用于整数）
    /// </summary>
    /// <param name="mytilemap"></param>
    /// <param name="mygrid"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <param name="down"></param>
    /// <param name="up"></param>
    private void LoadGridMessage(Tilemap mytilemap, Grid mygrid)//最（左/右/上/下）的坐标
    {
        int left = -(mygrid.x_node / 2);
        int right = mygrid.x_node / 2 - 1;
        int down = -(mygrid.y_node / 2);
        int up = mygrid.y_node / 2 - 1;
        for (int i = left; i <= right; i++)
        {
            for (int j = down; j <= up; j++)
            {
                for (int k = 0; k < numOfSprite; k++)
                {
                    if (mytilemap.GetSprite(new Vector3Int(i, j, 0)) == gridSprites_demo[k]) 
                    {
                        mygrid.grid[i - left, j - down].mol = k;
                        //Debug.Log("(" + i + "," + j + ")" + ":" + k);
                    }
                }
                
            }
        }
    }

    //private void Spread(Tilemap mytilemap, Grid mygrid)
    //{
    //    for (int i = 0; i < mygrid.x_node; i++)
    //    {
    //        for (int j = 0; j < mygrid.y_node; j++)
    //        {
    //            if (i - 1 >= 0 && mygrid.grid[i, j].nums != 0 && mygrid.grid[i, j].nums - 1 > mygrid.grid[i - 1, j].nums) 
    //            {
    //                mygrid.grid[i, j].nums--;
    //                mygrid.grid[i - 1, j].nums++;
    //            }
    //            if (i + 1 < mygrid.x_node && mygrid.grid[i, j].nums != 0 && mygrid.grid[i, j].nums - 1 > mygrid.grid[i + 1, j].nums)
    //            {
    //                mygrid.grid[i, j].nums--;
    //                mygrid.grid[i + 1, j].nums++;
    //            }
    //            if (j - 1 >= 0 && mygrid.grid[i, j].nums != 0 && mygrid.grid[i, j].nums - 1 > mygrid.grid[i, j - 1].nums)
    //            {
    //                mygrid.grid[i, j].nums--;
    //                mygrid.grid[i, j - 1].nums++;
    //            }
    //            if (j + 1 < mygrid.y_node && mygrid.grid[i, j].nums != 0 && mygrid.grid[i, j].nums - 1 > mygrid.grid[i, j + 1].nums)
    //            {
    //                mygrid.grid[i, j].nums--;
    //                mygrid.grid[i, j + 1].nums++;
    //            }
    //        }
    //    }
    //    LoadGridSprites(mytilemap, mygrid);
    //}
    private int[] RandomSort(int n)
    {
        List<int> availableNums = new List<int>();
        for (int i = 0; i < n; i++)
        {
            availableNums.Add(i);
        }
        int[] randomsort = new int[n];
        for (int i = 0; i < n; i++)
        {
            int temp = Random.Range(0, availableNums.Count);
            randomsort[i] = availableNums[temp];
            availableNums.RemoveAt(temp);
        }
        return randomsort;
    }
    private void Spread(Tilemap mytilemap, Grid mygrid)
    {
        int[] randomSortx = RandomSort(mygrid.x_node);
        for (int i_index = 0; i_index < mygrid.x_node; i_index++)
        {
            int i = randomSortx[i_index];
            int[] randomSorty = RandomSort(mygrid.y_node);
            for (int j_index = 0; j_index < mygrid.y_node; j_index++)
            {
                int j = randomSorty[j_index];
                //Debug.Log(i + "," + j);
                int[] randomSort3 = RandomSort(4);
                for (int k = 0; k < 4; k++)
                {//在差大于一的情况下转移1单位
                    if (randomSort3[k] == 0 && i - 1 >= 0 && mygrid.grid[i, j].mol > 0 && mygrid.grid[i, j].mol > mygrid.grid[i - 1, j].mol) //hack: mygrid.grid[i, j].mol - 0.1 > mygrid.grid[i - 1, j].mol停止条件
                    {
                        float move = (mygrid.grid[i, j].mol - mygrid.grid[i - 1, j].mol) / 4; //hack：转移公式
                        mygrid.grid[i, j].mol = mygrid.grid[i, j].mol - move;
                        mygrid.grid[i - 1, j].mol = mygrid.grid[i - 1, j].mol + move;
                    }
                    if (randomSort3[k] == 1 && i + 1 < mygrid.x_node && mygrid.grid[i, j].mol > 0 && mygrid.grid[i, j].mol > mygrid.grid[i + 1, j].mol) 
                    {
                        float move = (mygrid.grid[i, j].mol - mygrid.grid[i + 1, j].mol) / 4;
                        mygrid.grid[i, j].mol = mygrid.grid[i, j].mol - move;
                        mygrid.grid[i + 1, j].mol = mygrid.grid[i + 1, j].mol + move;
                    }
                    if (randomSort3[k] == 2 && j - 1 >= 0 && mygrid.grid[i, j].mol > 0 && mygrid.grid[i, j].mol > mygrid.grid[i, j - 1].mol)
                    {
                        float move = (mygrid.grid[i, j].mol - mygrid.grid[i, j - 1].mol) / 4;
                        mygrid.grid[i, j].mol = mygrid.grid[i, j].mol - move;
                        mygrid.grid[i, j - 1].mol = mygrid.grid[i, j - 1].mol + move;
                    }
                    if (randomSort3[k] == 3 && j + 1 < mygrid.y_node && mygrid.grid[i, j].mol > 0 && mygrid.grid[i, j].mol > mygrid.grid[i, j + 1].mol)
                    {
                        float move = (mygrid.grid[i, j].mol - mygrid.grid[i, j + 1].mol) / 4;
                        mygrid.grid[i, j].mol = mygrid.grid[i, j].mol - move;
                        mygrid.grid[i, j + 1].mol = mygrid.grid[i, j + 1].mol + move;
                    }
                }
            }
        }
        LoadGridSprites(mytilemap, mygrid);
    }

    private void LoadGridSprites(Tilemap mytilemap, Grid mygrid)
    {
        int left = -(mygrid.x_node / 2);
        int right = mygrid.x_node / 2 - 1;
        int down = -(mygrid.y_node / 2);
        int up = mygrid.y_node / 2 - 1;
        for (int i = left; i <= right; i++)
        {
            for (int j = down; j <= up; j++)
            {
                mytilemap.SetTile(new Vector3Int(i, j, 0), tileBases_demo[Mathf.RoundToInt(mygrid.grid[i - left, j - down].mol)]);
            }
        }

    }
}
