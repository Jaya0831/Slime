using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    public Vector2Int slimeVector = new Vector2Int(0, 0);
    public Grid myGrid;

    public int jumpAbility = 5;





    // Start is called before the first frame update
    void Start()
    {
        transform.position = FromVectorToSlimePosition() + new Vector3(0, 0, slimeVector.y - 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (PossibleMove(slimeVector, slimeVector + new Vector2Int(0, 1))) 
            {
                slimeVector += new Vector2Int(0, 1);
                transform.position = FromVectorToSlimePosition() + new Vector3(0, 0, slimeVector.y - 0.5f);
                if (GetComponent<Slime>().GetHurt())
                {
                    Debug.Log("destroy " + gameObject);
                    Destroy(gameObject);
                }
                GetComponent<Slime>().Absorb();
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (PossibleMove(slimeVector, slimeVector + new Vector2Int(0, -1)))
            {
                slimeVector += new Vector2Int(0, -1);
                transform.position = FromVectorToSlimePosition() + new Vector3(0, 0, slimeVector.y - 0.5f);
                if (GetComponent<Slime>().GetHurt())
                {
                    Debug.Log("destroy " + gameObject);
                    Destroy(gameObject);
                }
                GetComponent<Slime>().Absorb();                
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (PossibleMove(slimeVector, slimeVector + new Vector2Int(-1, 0)))
            {
                slimeVector += new Vector2Int(-1, 0);
                transform.position = FromVectorToSlimePosition() + new Vector3(0, 0, slimeVector.y - 0.5f);
                if (GetComponent<Slime>().GetHurt())
                {
                    Debug.Log("destroy " + gameObject);
                    Destroy(gameObject);
                }
                GetComponent<Slime>().Absorb();
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (PossibleMove(slimeVector, slimeVector + new Vector2Int(1, 0)))
            {
                slimeVector += new Vector2Int(1, 0);
                transform.position = FromVectorToSlimePosition() + new Vector3(0, 0, slimeVector.y - 0.5f);
                if (GetComponent<Slime>().GetHurt())
                {
                    Debug.Log("destroy " + gameObject);
                    Destroy(gameObject);
                }
                GetComponent<Slime>().Absorb();
            }
        }
    }
    public Vector3 FromVectorToSlimePosition()
    {
        Vector3 temp = myGrid.FromVectorToPosition(slimeVector);
        int high = Mathf.CeilToInt(myGrid.grid[slimeVector.x, slimeVector.y].solidmol);
        Vector3 offset = new Vector3(-0.5f, -0.5f + high * 0.05f, 0);
        return temp + offset;
    }

    private bool PossibleMove(Vector2Int current, Vector2Int neighbor)
    {
        if (neighbor.x < 0 || neighbor.y < 0 || neighbor.x >= myGrid.x_node || neighbor.y >= myGrid.y_node) 
        {
            return false;
        }
        if (!myGrid.grid[neighbor.x, neighbor.y].reachable) 
        {
            return false;
        }
        int high = Mathf.CeilToInt(myGrid.grid[current.x, current.y].solidmol);
        int highNeighbor = Mathf.CeilToInt(myGrid.grid[neighbor.x, neighbor.y].solidmol);
        if (highNeighbor - high > 5) 
        {
            return false;
        }
        return true;
    }
}
