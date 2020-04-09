using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public GameObject AIPlayer1;
    public GameObject AIPlayer2;

    public static AIController instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ControlAI(int index, int state)
    {
        GameObject controlAI;
        if(index == 1)
            controlAI = AIPlayer1.gameObject;
        else if(index == 2)
            controlAI = AIPlayer2.gameObject;
        else
            return;

        if(state == 1)
            controlAI.GetComponent<CharacterController>().Dance();
        else if(state == 2)
            controlAI.GetComponent<CharacterController>().Walk();
        else
            controlAI.GetComponent<CharacterController>().Idle();
    }

    public void ControlAIDance(int index, int score)
    {
        GameObject controlAI;
        if(index == 1)
            controlAI = AIPlayer1.gameObject;
        else if(index == 2)
            controlAI = AIPlayer2.gameObject;
        else
            return;

        controlAI.GetComponent<CharacterController>().Dance(score);
    }

    public Vector3 GetAIPosition(int index)
    {
        if(index == 1)
            return GameObject.Find("AIPlayer1Pos").transform.position;
        else if(index == 2)
            return GameObject.Find("AIPlayer2Pos").transform.position;
        return Vector3.zero;
    }

    public GameObject GetAICharacter(int index)
    {
        if(index == 1)
            return GameObject.Find("AIPlayer1Pos");
        else if(index == 2)
            return GameObject.Find("AIPlayer2Pos");
        return null;
    }

    public GameObject GetAIResultPos(int index)
    {
        if(index == 1)
            return GameObject.Find("AIResult1Pos");
        else if(index == 2)
            return GameObject.Find("AIResult2Pos");
        return null;
    }
}
