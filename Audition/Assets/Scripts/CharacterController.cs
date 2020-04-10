using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Animation {Idle = 0, Dance = 1, Walk = 2}

public class CharacterController : MonoBehaviour
{
    //0: idle  (loop)
    //1: dance (non loop) -> call AnimationController.OnStateExit to back to value 0
    //2: walk  (loop)
    private int state = 0;
    //Dance style;
    //BoringDance
    //HipHopDancing1
    //HipHopDancing2
    //BreakdanceFootwork
    //GangnameStyle

    private static int danceStyle = 0;
    private bool isPerfect = false;

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

    public void Idle(bool force = false)
    {
        //Debug.Log("CharacterController: Idle");
        state = (int)Animation.Idle;
        SetAnimation(state);
        if(force)
        {
            this.GetComponent<Animator>().Rebind();
            this.GetComponent<Animator>().Play("Idle");
        }
    }

    public void Dance()
    {
        //Debug.Log("CharacterController: Dance");
        state = (int)Animation.Dance;
        SetAnimation(state);
    }

    public void Dance(GameManager.Result result)
    {
        int randDance = Random.Range(1,4);
        danceStyle = randDance;
        if(LevelRthymController.instance.getLevelHolder() >= 10)
        {
            danceStyle = 5;
        }
        this.GetComponent<Animator>().Rebind();

        if(result == GameManager.Result.Miss)
            return;
        //Dance based from score;
        //20 - BoringDance
        //40 - HipHopDancing1
        //60 - HipHopDancing2
        //80 - BreakdanceFootwork
        //100 - GangnameStyle
        state = (int)Animation.Dance;
        // if(score > 80)
        //     danceStyle = 4;
        // else if(score > 60)
        //     danceStyle = 3;
        // else if(score > 40)
        //     danceStyle = 2;
        // else if(score > 20)
        //     danceStyle = 1;
        // else
        //     danceStyle = 0;
        SetAnimation(state);
    }

    public void AIDance()
    {
        state = (int)Animation.Dance;
        this.GetComponent<Animator>().Rebind();
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
        this.GetComponent<Animator>().SetInteger("danceStyle", danceStyle);
        this.GetComponent<Animator>().SetFloat("Stype", (float)danceStyle); //thai 
        this.GetComponent<Animator>().SetBool("isPerfect", (danceStyle == 4)); //Perfect will dance Gangnam Style
    }

    public bool IsDancing()
    {
        return (this.GetComponent<Animator>().GetInteger("state") == 1);
    }
}
