using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject prefabArrowSprite;
    private bool isRenderMove = false;
    private bool isPlayerMoveFinished = false;
    private int currentMove = 0;
    private List<int> playerMove;
    private float startRenderMovesTime;
    private float renderMovesEffectTime = 2.0f;
    private float yResultOffset = 1.5f;
    
    public GameObject moveBackgroundGood;
    public GameObject moveBackgroundBad;
    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameManager is START!");

        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(isRenderMove)
        {
            CheckInputMove();
            StartRenderMovesEffect();
        }
    }

    public void StartGame()
    {
        GetMove();
        RenderMove();
    }

    public void GetMove()
    {
        GenerateMove.instance.RandomMove();
        List<int> move = GenerateMove.instance.GetMove();
        if(move.Count > 0)
        {
            string moveList = "";
            for(int i = 0; i < move.Count; i++)
                moveList += " " + ConvertMoveFromInt(move[i]);
            Debug.Log("GetMove: " + moveList);
        }
    }

    public void RenderMove()
    {
        List<int> move = GenerateMove.instance.GetMove();
        Transform positionMove = GameObject.Find("MoveBar").transform;
        if(move.Count > 0)
        {
            for(int i = 0; i < move.Count; i++)
            {
                float x = positionMove.position.x + 1*i - 1*(move.Count/2);
                float y = positionMove.position.y;
                float z = positionMove.position.z;
                Vector3 position = new Vector3(x, y, z);
                Direction dir = (Direction)move[i];
                switch(dir)
                {
                    case Direction.Up:
                        SpawnArrowUp(position);
                        break;
                    case Direction.Down:
                        SpawnArrowDown(position);
                        break;
                    case Direction.Left:
                        SpawnArrowLeft(position);
                        break;
                    case Direction.Right:
                        SpawnArrowRight(position);
                        break;
                }
                isRenderMove = true;
                isPlayerMoveFinished = false;
                currentMove = 0;
                playerMove = new List<int>();
            }

            // Make all move sprites is transparent to fade in
            startRenderMovesTime = Time.time;
            GameObject[] objsCurrentMoves = GameObject.FindGameObjectsWithTag("CurrentMoves");
            foreach (GameObject obj in objsCurrentMoves)
            {
                obj.GetComponent<SpriteRenderer>().color = new Color(
                    obj.GetComponent<SpriteRenderer>().color.r,
                    obj.GetComponent<SpriteRenderer>().color.g,
                    obj.GetComponent<SpriteRenderer>().color.b,
                    0);

                // Remove old BG moves
                if((obj.transform.childCount > 0) && obj.transform.GetChild(0) != null)
                {
                    Destroy(obj.transform.GetChild(0).gameObject);
                }
            }
        }
    }

    void CheckInputMove()
    {
        List<int> move = GenerateMove.instance.GetMove();
        if(currentMove < move.Count)
        {
            if(Input.GetKeyUp(KeyCode.UpArrow))
            {
                //Debug.Log("PressUp");
                playerMove.Add((int)Direction.Up);
                SpawnMoveBG(currentMove);
                currentMove++;
            }
            else if(Input.GetKeyUp(KeyCode.DownArrow))
            {
                //Debug.Log("PressDown");
                playerMove.Add((int)Direction.Down);
                SpawnMoveBG(currentMove);
                currentMove++;
            }
            else if(Input.GetKeyUp(KeyCode.LeftArrow))
            {
                //Debug.Log("PressLeft");
                playerMove.Add((int)Direction.Left);
                SpawnMoveBG(currentMove);
                currentMove++;
            }
            else if(Input.GetKeyUp(KeyCode.RightArrow))
            {
                //Debug.Log("PressRight");
                playerMove.Add((int)Direction.Right);
                SpawnMoveBG(currentMove);
                currentMove++;
            }
        }

        if(currentMove >= move.Count)
        {
            if(isPlayerMoveFinished == false)
            {
                int match = 0;
                string playerMoveList = "";
                for(int i = 0; i < playerMove.Count; i++)
                {
                    playerMoveList += " " + ConvertMoveFromInt(playerMove[i]);
                    if(playerMove[i] == move[i])
                    {
                        match++;
                    }
                }
                isPlayerMoveFinished = true;

                int score = (int)(((float)match / move.Count) * 100);
                DisplayResult(score);

                // Make player start to dance
                GameObject player = GameObject.Find("Player2");
                player.GetComponent<CharacterController>().Dance();

                // Test Control AI
                AIController.instance.ControlAI(1, Random.Range((int)Animation.Dance, (int)Animation.Walk+1));
                AIController.instance.ControlAI(2, Random.Range((int)Animation.Dance, (int)Animation.Walk+1));

                // AI Score
                int AI1Score = Random.Range(1, 100);
                int AI2Score = Random.Range(1, 100);
                DisplayAIResult(AIController.instance.GetAIResultPos(1), AI1Score);
                DisplayAIResult(AIController.instance.GetAIResultPos(2), AI2Score);

                // Make a coroutine to start new move after 2 seconds
                IEnumerator coroutine = StartNewMove(2.0f);
                StartCoroutine(coroutine);        

                Debug.Log("PlayerMove: " + playerMoveList + " Result score: " + score + " AI1 score: " + AI1Score + " AI2 score: " + AI2Score);
            }
        }
    }

    IEnumerator StartNewMove(float delayTime)
    {
        yield return new WaitForSeconds(delayTime / 2);

        GameObject[] objsCurrentMoves = GameObject.FindGameObjectsWithTag("CurrentMoves");
        foreach (GameObject obj in objsCurrentMoves)
        {
            obj.gameObject.SetActive(false); 
            obj.gameObject.Kill();
        }

        yield return new WaitForSeconds(delayTime);

        // After delayTime -> reset game
        GameObject[] objsResultMoves = GameObject.FindGameObjectsWithTag("ResultMoves");
        foreach (GameObject obj in objsResultMoves)
        {
            obj.GetComponent<SpriteRenderer>().enabled = false;
            obj.transform.localPosition = Vector3.zero;
        }

        // After delayTime -> make new Move
        GetMove();
        RenderMove();

        // Set Player animation to Idle
        GameObject player = GameObject.Find("Player2");
        player.GetComponent<CharacterController>().Idle();

        // Stop AI animation
        AIController.instance.ControlAI(1, (int)Animation.Idle);
        AIController.instance.ControlAI(2, (int)Animation.Idle);

        // Destroy AI Result objects
        GameObject[] objsAIResult = GameObject.FindGameObjectsWithTag("AIResult");
        foreach (GameObject obj in objsAIResult)
        {
            Destroy(obj);
        }
    }

    void StartRenderMovesEffect()
    {
        float effectTime = renderMovesEffectTime; //2 seconds to fade in
        if((Time.time - startRenderMovesTime) <= effectTime)
        {
            GameObject[] objsCurrentMoves = GameObject.FindGameObjectsWithTag("CurrentMoves");
            int idx = 0;
            foreach (GameObject obj in objsCurrentMoves)
            {
                obj.GetComponent<SpriteRenderer>().color = new Color(
                    obj.GetComponent<SpriteRenderer>().color.r,
                    obj.GetComponent<SpriteRenderer>().color.g,
                    obj.GetComponent<SpriteRenderer>().color.b,
                    Mathf.Lerp(0, 1.0f, (Time.time - (startRenderMovesTime + (float)idx * 0.15f)) / effectTime));
                idx++;
            }
        }
    }

    void DisplayResult(int score)
    {
        if(score > 80)
        {
            GameObject result = GameObject.Find("Result_Perfect");
            if(result != null)
            {
                result.transform.localPosition = new Vector3(0, yResultOffset, 0);
                result.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        else if(score > 60)
        {
            GameObject result = GameObject.Find("Result_Good");
            if(result != null)
            {
                result.transform.localPosition = Vector3.zero;
                result.GetComponent<SpriteRenderer>().enabled = true;
                result.GetComponent<Animator>().Rebind();
                result.GetComponent<Animator>().Play("good");
            }
        }
        else if(score > 40)
        {
            GameObject result = GameObject.Find("Result_Cool");
            if(result != null)
            if(result != null)
            {
                result.transform.localPosition = Vector3.zero;
                result.GetComponent<SpriteRenderer>().enabled = true;
                result.GetComponent<Animator>().Rebind();
                result.GetComponent<Animator>().Play("good");
            }
        }
        else if(score > 20)
        {
            GameObject result = GameObject.Find("Result_Bad");
            if(result != null)
            if(result != null)
            {
                result.transform.localPosition = Vector3.zero;
                result.GetComponent<SpriteRenderer>().enabled = true;
                result.GetComponent<Animator>().Rebind();
                result.GetComponent<Animator>().Play("good");
            }
        }
        else
        {
            GameObject result = GameObject.Find("Result_Miss");
            if(result != null)
            if(result != null)
            {
                result.transform.localPosition = Vector3.zero;
                result.GetComponent<SpriteRenderer>().enabled = true;
                result.GetComponent<Animator>().Rebind();
                result.GetComponent<Animator>().Play("good");
            }
        }
    }

    void DisplayAIResult(GameObject obj, int score)
    {
        GameObject result;
        bool isPerfect = false;

        if(score > 80)
        {
           result = GameObject.Find("Result_Perfect");
           isPerfect = true; // apply offset for special sprite
        }
        else if(score > 60)
            result = GameObject.Find("Result_Good");
        else if(score > 40)
            result = GameObject.Find("Result_Cool");
        else if(score > 20)
            result = GameObject.Find("Result_Bad");
        else
            result = GameObject.Find("Result_Miss");

        if(result != null)
        {
            GameObject spawnInstance = Instantiate(result);
            spawnInstance.transform.SetParent(obj.transform);
            if(isPerfect)
                spawnInstance.transform.localPosition = new Vector3(0,yResultOffset + 0.2f,0);
            else
                spawnInstance.transform.localPosition = Vector3.zero;
            spawnInstance.tag = "AIResult";
            spawnInstance.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    string ConvertMoveFromInt(int move)
    {
        Direction dir = (Direction)move;
        switch(dir)
        {
            case Direction.Up:
                return "Up";
            case Direction.Down:
                return "Down";
            case Direction.Left:
                return "Left";
            case Direction.Right:
                return "Right";
        }
        return "";
    }

    void SpawnArrowUp(Vector3 pos)
    {
        GameObject arrow = prefabArrowSprite.Spawn();
        if (arrow != null)
        {
            arrow.transform.position = pos;
            arrow.transform.transform.Rotate(new Vector3(0, 0, 90)); 
            arrow.tag = "CurrentMoves";
            arrow.SetActive(true);
        } 
    }
    void SpawnArrowDown(Vector3 pos)
    {
        GameObject arrow = prefabArrowSprite.Spawn();
        if (arrow != null)
        {
            arrow.transform.position = pos;
            arrow.transform.transform.Rotate(new Vector3(0, 0, -90)); 
            arrow.tag = "CurrentMoves";
            arrow.SetActive(true);
        } 
    }
    void SpawnArrowLeft(Vector3 pos)
    {
        GameObject arrow = prefabArrowSprite.Spawn();
        if (arrow != null)
        {
            arrow.transform.position = pos;
            arrow.transform.transform.Rotate(new Vector3(0, 0, 180));
            arrow.tag = "CurrentMoves";
            arrow.SetActive(true);
        } 
    }
    void SpawnArrowRight(Vector3 pos)
    {
        GameObject arrow = prefabArrowSprite.Spawn();
        if (arrow != null)
        {
            arrow.transform.position = pos;
            arrow.tag = "CurrentMoves";
            arrow.SetActive(true);
        } 
    }

    void SpawnMoveBG(int current)
    {
        List<int> move = GenerateMove.instance.GetMove();
        GameObject[] objsCurrentMoves = GameObject.FindGameObjectsWithTag("CurrentMoves");
        int idx = 0;
        foreach (GameObject obj in objsCurrentMoves)
        {
            if(idx++ != current)
                continue;
            if(playerMove[current] == move[current])
            {
                GameObject spawnInstance = Instantiate(moveBackgroundGood);
                spawnInstance.transform.SetParent(obj.transform);
                spawnInstance.transform.position = obj.transform.position;
                spawnInstance.GetComponent<SpriteRenderer>().enabled = true;
                spawnInstance.SetActive(true); 
            }
            else
            {
                GameObject spawnInstance = Instantiate(moveBackgroundBad);
                spawnInstance.transform.SetParent(obj.transform);
                spawnInstance.transform.position = obj.transform.position;
                spawnInstance.GetComponent<SpriteRenderer>().enabled = true;
                spawnInstance.SetActive(true); 
            }
        }
    }
}
