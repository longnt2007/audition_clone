using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Animation {Idle = 0, Dance = 1, Walk = 2}

public class CharacterController : MonoBehaviour
{
    //0: idle  (loop)
    //1: dance (non loop) -> call AnimationController.OnStateExit to back to value 0
    //2: walk  (loop)
    private int state; 

    // Start is called before the first frame update
    void Awake()
    {
        // Do smt
    }

    // Update is called once per frame
    void Update()
    {
        // Do smt
    }

    public void Idle()
    {
        //Debug.Log("CharacterController: Idle");
        state = (int)Animation.Idle;
        SetAnimation(state);
    }

    public void Dance()
    {
        //Debug.Log("CharacterController: Dance");
        state = (int)Animation.Dance;
        SetAnimation(state);
    }

    public void Walk()
    {
        //Debug.Log("CharacterController: Walk");
        state = (int)Animation.Walk;
        SetAnimation(state);
    }

    public void SetAnimation(int state)
    {
        this.GetComponent<Animator>().SetInteger("state", state);
    }
}
