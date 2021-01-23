using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridManager : MonoBehaviour
{
    public Grid myGrid;
    // 0~1,1~2...

    public SlimeController slimeController;
    public Slime slime;

    // Start is called before the first frame update
    void Start()
    {
    }

    private int timer=0;
    // Update is called once per frame
    void Update()//其实这个应该写到GameManager里面更好
    {
        if (timer == 30 && slime.alive) 
        {
            timer = 0;
            List<PublicEnum.LiquidType> temp = new List<PublicEnum.LiquidType> { PublicEnum.LiquidType.lava, PublicEnum.LiquidType.water };
            slime.Absorb();
            //hack:需不需要检测gethurt？因为实际上岩浆并不会到格子上，但是会在slime所在格子的旁边
            myGrid.grid[slimeController.slimeVector.x, slimeController.slimeVector.y].RemoveAllOnAverage();
            Spread(myGrid, temp);
            myGrid.DeleteMap();
            myGrid.Display();
            //Debug.Log("[5,0].mol:" + myGrid.GetComponent<Grid>().grid[5, 0].liquidmol);
            //Debug.Log("[4,0].mol:" + myGrid.GetComponent<Grid>().grid[4, 0].liquidmol);
            //Debug.Log("[6,0].mol:" + myGrid.GetComponent<Grid>().grid[6, 0].liquidmol);
            //Debug.Log("[5,1].mol:" + myGrid.GetComponent<Grid>().grid[5, 1].liquidmol);
            //Debug.Log("[5,0].liquidTypes.Count:" + myGrid.GetComponent<Grid>().grid[5, 0].liquidTypes.Count);
            //Debug.Log("[5,0].liquidNums[1]:" + myGrid.GetComponent<Grid>().grid[5, 0].liquidNums[1]);
            //Debug.Log("[5,0].waterHeights:" + myGrid.GetComponent<Grid>().grid[5, 0].liquidHeights[myGrid.GetComponent<Grid>().grid[5, 0].liquidHeights.Count - 1]);
        }
        timer++;
    }

    /// <summary>
    /// 0～n-1的随机排列
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 扩散
    /// </summary>
    /// <param name="mytilemap"></param>
    /// <param name="mygrid"></param>
    private void Spread(Grid myGrid, List<PublicEnum.LiquidType> possibleLiquids)
    {
        for (int k = 0; k < possibleLiquids.Count; k++)//决定是哪种液体
        {
            int[] randomSortx = RandomSort(myGrid.x_node);
            for (int i_index = 0; i_index < myGrid.x_node; i_index++)
            {
                int i = randomSortx[i_index];
                int[] randomSorty = RandomSort(myGrid.y_node);
                for (int j_index = 0; j_index < myGrid.y_node; j_index++)
                {

                    int j = randomSorty[j_index];
                    if (myGrid.grid[i, j].hasObstacle || !myGrid.grid[i, j].liquidTypes.Exists(c => c.Equals(possibleLiquids[k])))
                    {
                        continue;
                    }

                    int index = myGrid.grid[i, j].liquidTypes.FindIndex(c => c.Equals(possibleLiquids[k]));

                    int[] randomSort3 = RandomSort(4);
                    for (int p = 0; p < 4 + myGrid.grid[i, j].connectNodes.Count; p++)
                    {
                        int iNeighbor = i;
                        int jNeighbor = j;
                        if (p < 4) 
                        {
                            switch (randomSort3[p])
                            {
                                case 0:
                                    iNeighbor = i - 1;
                                    break;
                                case 1:
                                    iNeighbor = i + 1;
                                    break;
                                case 2:
                                    jNeighbor = j + 1;
                                    break;
                                case 3:
                                    jNeighbor = j - 1;
                                    break;

                            }
                        }                       
                        if (p >= 4)//处理被管道连接的情况
                        {//hack:被管道连接的地方就是邻居节点的情况
                            iNeighbor = myGrid.grid[i, j].connectNodes[p - 4].nodeI;
                            jNeighbor = myGrid.grid[i, j].connectNodes[p - 4].nodeJ;
                        }
                        if (iNeighbor > -1 && iNeighbor < myGrid.x_node && jNeighbor > -1 && jNeighbor < myGrid.y_node &&
                        !myGrid.grid[iNeighbor, jNeighbor].hasObstacle &&
                        (p >= 4 || !myGrid.grid[i, j].hasWall[randomSort3[p]] && !myGrid.grid[iNeighbor, jNeighbor].hasWall[randomSort3[p] % 2 == 1 ? randomSort3[p] - 1 : randomSort3[p] + 1]))
                        {

                            float height = myGrid.grid[i, j].liquidHeights[index];
                            float num = myGrid.grid[i, j].liquidNums[index];
                            if (!myGrid.grid[iNeighbor, jNeighbor].liquidTypes.Exists(c => c.Equals(possibleLiquids[k])))
                            {
                                if (height + num > myGrid.grid[iNeighbor, jNeighbor].solidmol)
                                {
                                    float remove = height > myGrid.grid[iNeighbor, jNeighbor].solidmol ? (num / 4.0f) : ((height + num - myGrid.grid[iNeighbor, jNeighbor].solidmol) / 4.0f);
                                    myGrid.grid[i, j].UpdateALiquid(possibleLiquids[k], num - remove);
                                    myGrid.grid[iNeighbor, jNeighbor].AddALiquid(possibleLiquids[k], remove);
                                }
                            }
                            else
                            {
                                int indexNeighbor = myGrid.grid[iNeighbor, jNeighbor].liquidTypes.FindIndex(c => c.Equals(possibleLiquids[k]));
                                float heightNeighbor = myGrid.grid[iNeighbor, jNeighbor].liquidHeights[indexNeighbor];
                                float numNeighbor = myGrid.grid[iNeighbor, jNeighbor].liquidNums[indexNeighbor];
                                if (height + num > heightNeighbor + numNeighbor)
                                {
                                    float remove = height > heightNeighbor + numNeighbor ? (num / 4.0f) : ((height + num - heightNeighbor - numNeighbor) / 4.0f);
                                    myGrid.grid[i, j].UpdateALiquid(possibleLiquids[k], num - remove);
                                    myGrid.grid[iNeighbor, jNeighbor].UpdateALiquid(possibleLiquids[k], numNeighbor + remove);
                                }
                            }
                            
                        }
                    }
                }
            }
        }
        for (int i = 0; i < myGrid.x_node; i++)
        {
            for (int j = 0; j < myGrid.y_node; j++)
            {
                for (int k = 0; k < myGrid.grid[i,j].solidNums.Count; k++)
                {
                    if (myGrid.grid[i, j].solidNums[k] < 0.01f)
                    {
                        myGrid.grid[i, j].DeleteASolid(myGrid.grid[i, j].solidTypes[k]);
                    }
                }
                for (int k = 0; k < myGrid.grid[i, j].liquidNums.Count; k++)
                {
                    if (myGrid.grid[i, j].liquidNums[k] < 0.01f)
                    {
                        myGrid.grid[i, j].DeleteALiquid(myGrid.grid[i, j].liquidTypes[k]);
                    }
                }
                
            }
        }
        for (int i = 0; i < myGrid.x_node; i++)
        {
            for (int j = 0; j < myGrid.y_node; j++)
            {
                myGrid.grid[i, j].Reaction();
            }
        }
    }
}
