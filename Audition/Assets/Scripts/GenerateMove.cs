using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {Up = 1, Down = 2, Left = 3, Right = 4}

public class GenerateMove : MonoBehaviour
{
    public static GenerateMove instance;

    public List<int> move;
    
    // Start is called before the first frame update
    void Awake()
    {
        //Debug.Log("GenerateMove::Awake");
        instance = this;

        move = new List<int>();
        move.Add((int)Direction.Up);    //Up
        move.Add((int)Direction.Down);  //Down
        move.Add((int)Direction.Left);  //Left
        move.Add((int)Direction.Right); //Right
    }

    public List<int> GetMove()
    {
        //Debug.Log("GenerateMove::GetMove");
        return move;
    }

    public void RandomMove()
    {
        Debug.Log("GenerateMove::RandomMove");
        move.Clear();
        move.Add(Random.Range(1,5));
        move.Add(Random.Range(1,5));
        move.Add(Random.Range(1,5));
        move.Add(Random.Range(1,5));
        move.Add(Random.Range(1,5));
    }
}
