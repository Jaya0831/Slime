using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    public GameObject myGrid;
    public GameObject myEnum;
    public enum AbilityA
    {
        none,
        electricity,
        pipe,
        fire
    }
    public enum AbilityB
    {
        none,
        filter,
        block//不可以通过液体
    }
    public AbilityA abilityA;
    public AbilityB abilityB;
    public List<PublicEnum.LiquidType> filterAble = new List<PublicEnum.LiquidType>();
    public List<PublicEnum.LiquidType> filterDisable = new List<PublicEnum.LiquidType>();
    public PublicEnum.Direction pipeDirection = PublicEnum.Direction.none;

    public List<PublicEnum.LiquidType> containTypes = new List<PublicEnum.LiquidType>();
    public List<float> containNums = new List<float>();
    public float containMol = 0;
    public float containLimit = 10f;

    public bool alive = true;

    /// <summary>
    /// 计时器
    /// </summary>
    public int timer = 0;
    public int time = 50;



    private void Update()
    {
        //timer++;
    }

    private void Start()
    {
        DisplaySlime();
        //containTypes.Add(PublicEnum.LiquidType.water);
        //containNums.Add(8);
        //containMol += 8;
    }
    public void DisplaySlime()
    {
        //删除所有子物体 
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);//为什么是i：因为下一帧才会被删除，迷惑
        }
        transform.position = GetComponent<SlimeController>().FromVectorToSlimePosition() + new Vector3(0, 0, GetComponent<SlimeController>().slimeVector.y - 0.5f);//更新slime位置
        int highTemp = 0;
        float offSet = 0.05f;
        float highStart = -0.475f;
        float planeStart = 0f;
        Vector3 positionTemp = this.GetComponent<SlimeController>().FromVectorToSlimePosition() + new Vector3(0.5f, 0.5f, 0);
        for (int i = 0; i < containTypes.Count; i++)
        {
            for (int j = 0; j < Mathf.CeilToInt(containNums[i]); j++)
            {
                GameObject gameObjectTemp = Instantiate(myEnum.GetComponent<PublicEnum>().LiquidHighSprites[(int)containTypes[i]], positionTemp + new Vector3(0, highStart + offSet * highTemp, GetComponent<SlimeController>().slimeVector.y - 0.3f), Quaternion.identity, transform);//0.3f：不可以遮挡slime
                highTemp++;
            }
        }
        if (containTypes.Count != 0)
        {
            GameObject gameObjectTemp = Instantiate(myEnum.GetComponent<PublicEnum>().LiquidPlaneSprites[(int)containTypes[containTypes.Count - 1]], positionTemp + new Vector3(0, planeStart + offSet * highTemp, GetComponent<SlimeController>().slimeVector.y - 0.3f), Quaternion.identity, transform);
        }
    }

    /// <summary>
    /// 吸收格子上的液体
    /// </summary>
    public void Absorb()
    {
        //Debug.Log("start");
        Node node = myGrid.GetComponent<Grid>().grid[this.GetComponent<SlimeController>().slimeVector.x, this.GetComponent<SlimeController>().slimeVector.y];
        for (int i = 0; i < containTypes.Count; i++)
        {
            node.FlowIn(containTypes[i], containNums[i]);
        }
        containTypes.Clear();
        containNums.Clear();
        containMol = 0;
        node.Reaction();//hack：其实最好再单写一个（针对slime和液体交互的reaction），不然只靠gethurt加限制的话到反应多的时候会很麻烦
        
        for (int i = 0; i < node.liquidTypes.Count; i++)
        {
            if (containMol < containLimit) 
            {
                if (node.liquidNums[0] < containLimit - containMol)
                {
                    containTypes.Add(node.liquidTypes[i]);
                    containNums.Add(node.liquidNums[i]);
                    containMol += node.liquidNums[i];
                    node.DeleteALiquid(node.liquidTypes[i--]);
                    
                }
                else
                {
                    float tmp = node.liquidNums[0] - (containLimit - containMol);
                    containTypes.Add(node.liquidTypes[0]);
                    containNums.Add(containLimit - containMol);
                    containMol = containLimit;
                    node.UpdateALiquid(node.liquidTypes[0], tmp);
                    break;
                }
            }
            else
            {
                break;
            }
        }
        DisplaySlime();
    }

    public bool GetHurt()
    {
        Debug.Log("check");
        if (containTypes.Exists(c => c.Equals(PublicEnum.LiquidType.lava)) && containNums[containTypes.FindIndex(c => c.Equals(PublicEnum.LiquidType.lava))] >= 7)
        {
            Debug.Log("check1");
            return false;
        }
        else
        {
            Debug.Log("check2");
            Node node = myGrid.GetComponent<Grid>().grid[GetComponent<SlimeController>().slimeVector.x, GetComponent<SlimeController>().slimeVector.y];
            int index_lava = node.liquidTypes.FindIndex(c => c.Equals(PublicEnum.LiquidType.lava));
            Debug.Log(node.liquidTypes.Exists(c => c.Equals(PublicEnum.LiquidType.lava)));
            Debug.Log(node.liquidTypes.Exists(c => c.Equals(PublicEnum.LiquidType.lava)) && node.liquidNums[node.liquidTypes.FindIndex(c => c.Equals(PublicEnum.LiquidType.lava))] > 1);
            float containWaterNum = 0;
            if (containTypes.Exists(c=>c.Equals(PublicEnum.LiquidType.water)))
            {
                int index_water = containTypes.FindIndex(c => c.Equals(PublicEnum.LiquidType.water));
                containWaterNum = containNums[index_water];
            }
            if (node.liquidTypes.Exists(c => c.Equals(PublicEnum.LiquidType.lava)) && node.liquidNums[index_lava] - containWaterNum> 3)
            {
                Debug.Log("gethurt");
                alive = false;
                return true;
            }

            return false;
        }
    }
}