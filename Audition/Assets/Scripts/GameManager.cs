using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    private GameObject footParticle;
    private int playerScore;
    private int currentScore;
    private int currentLastTurnScore;
    private int playerLastTurnScore;
    private int ai1Score;
    private int ai2Score;
    public GameObject playerScoreTopText;
    //private bool playEffectScore = false;
    public GameObject playerScore1stText;
    public GameObject playerScore2ndText;
    public GameObject playerScore3rdText;
    private bool isAIChangedScore = false;

    public float SpeedBmp;
    public Slider SpeedBar;

    public GameObject HitEffect;
    private bool playerTurn = false;

    private int countNextMove = 0;
    private bool lockCountNextMove = true;
    private bool isShowMove = false;

    public enum Result
    {
        Perfect,
        Great,
        Cool,
        Bad,
        Miss
    }
    int[] ScoreBoard =
    {
        5400,
        4050,
        2700,
        1890,
        0
    };

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
            //StartRenderMovesEffect();
            RenderTopScore();
            RenderAIScore();
        }
        moveSpeedBarSmooth();
    }

    public void StartGame()
    {
        //GetMove();
        //RenderMove();

        currentLastTurnScore = playerLastTurnScore = currentScore = playerScore = ai1Score = ai2Score = 0;
        footParticle = GameObject.Find("FinishMove");
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
            StartRenderMovesEffect();

            // // Make all move sprites is transparent to fade in
            // startRenderMovesTime = Time.time;
            // GameObject[] objsCurrentMoves = GameObject.FindGameObjectsWithTag("CurrentMoves");
            // foreach (GameObject obj in objsCurrentMoves)
            // {
            //     obj.GetComponent<SpriteRenderer>().color = new Color(
            //         obj.GetComponent<SpriteRenderer>().color.r,
            //         obj.GetComponent<SpriteRenderer>().color.g,
            //         obj.GetComponent<SpriteRenderer>().color.b,
            //         0);

            //     // Remove old BG moves
            //     if((obj.transform.childCount > 0) && obj.transform.GetChild(0) != null)
            //     {
            //         Destroy(obj.transform.GetChild(0).gameObject);
            //     }
            // }
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

            // Test music
            //if(currentMove > 0)
            //{
            //    SoundManager.instance.PlayMusic();
            //}
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //playerTurn = false;
            Result resultHit = IsResult();
            if (currentMove >= move.Count)
            {
                //if (isPlayerMoveFinished == false)
                {
                    int match = 0;
                    string playerMoveList = "";
                    for (int i = 0; i < playerMove.Count; i++)
                    {
                        playerMoveList += " " + ConvertMoveFromInt(playerMove[i]);
                        if (playerMove[i] == move[i])
                        {
                            match++;
                        }
                    }
                    isPlayerMoveFinished = true;
                    int score = 0;
                    if (match >= playerMove.Count)
                    {
                        score = ScoreBoard[(int)resultHit];
                        playerScoreTopText.GetComponent<Animator>().Rebind();
                        playerScoreTopText.GetComponent<Animator>().Play("ScorePlayer");
                        
                        HitEffect.transform.position = GameObject.Find("MoveBar").transform.position;
                        HitEffect.GetComponent<ParticleSystem>().Play(); 
                    }

                    //int score = (int)(((float)match / move.Count) * 100);
                    DisplayResult(resultHit);

                    playerDance(resultHit);
                    // hide particle at player foot
                    footParticle.SetActive(false);

                    // Random AI Score

                    AIDance();

                    // Increase score for players
                    playerLastTurnScore = score;
                    playerScore += score;
                    
                    //playerTurn = true;
                    resetForNextMove();
                }
            }
        }
        // Make a coroutine to start new move after 2 seconds
        // if (currentMove >= move.Count && playerTurn)
        // {
        //     IEnumerator coroutine = StartNewMove(0.1f);
        //     StartCoroutine(coroutine);
        // }
    }
    void playerDance(Result resultHit)
    {
        // Make player start to dance
        GameObject player = GameObject.Find("Player2");
        player.GetComponent<CharacterController>().Dance(resultHit);
    }

    void resetForNextMove()
    {
        GameObject[] objsCurrentMoves = GameObject.FindGameObjectsWithTag("CurrentMoves");
        foreach (GameObject obj in objsCurrentMoves)
        {
            obj.gameObject.SetActive(false); 
            obj.gameObject.Kill();
        }
        // After delayTime -> reset game
        GameObject[] objsResultMoves = GameObject.FindGameObjectsWithTag("ResultMoves");
        foreach (GameObject obj in objsResultMoves)
        {
            obj.GetComponent<SpriteRenderer>().enabled = false;
            obj.transform.localPosition = Vector3.zero;
        }

        // Make all move sprites is transparent to fade in
            startRenderMovesTime = Time.time;
            //GameObject[] objsCurrentMoves = GameObject.FindGameObjectsWithTag("CurrentMoves");
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
        isShowMove = false;
    }

    void NextMove()
    {
        GameObject[] objsCurrentMoves = GameObject.FindGameObjectsWithTag("CurrentMoves");
        foreach (GameObject obj in objsCurrentMoves)
        {
            obj.gameObject.SetActive(false); 
            obj.gameObject.Kill();
        }
        // After delayTime -> reset game
        GameObject[] objsResultMoves = GameObject.FindGameObjectsWithTag("ResultMoves");
        foreach (GameObject obj in objsResultMoves)
        {
            obj.GetComponent<SpriteRenderer>().enabled = false;
            obj.transform.localPosition = Vector3.zero;
        }
        GetMove();
        RenderMove();

        // Destroy AI Result objects
        GameObject[] objsAIResult = GameObject.FindGameObjectsWithTag("AIResult");
        foreach (GameObject obj in objsAIResult)
        {
            Destroy(obj);
        }

        //show particle at player foot
        footParticle.SetActive(true);

        isShowMove = true;
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

        GameObject player = GameObject.Find("Player2");
        yield return new WaitForSeconds(delayTime);
        
        while(player.GetComponent<CharacterController>().IsDancing())
        {
            yield return new WaitForSeconds(0.1f); // wait until player dance is done
        }

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

        // Stop AI animation
        AIController.instance.ControlAI(1, (int)Animation.Idle);
        AIController.instance.ControlAI(2, (int)Animation.Idle);

        // Destroy AI Result objects
        GameObject[] objsAIResult = GameObject.FindGameObjectsWithTag("AIResult");
        foreach (GameObject obj in objsAIResult)
        {
            Destroy(obj);
        }

        //show particle at player foot
        footParticle.SetActive(true);
    }

    void StartRenderMovesEffect()
    {
        //float effectTime = renderMovesEffectTime; //2 seconds to fade in
        //if((Time.time - startRenderMovesTime) <= effectTime)
        {
            GameObject[] objsCurrentMoves = GameObject.FindGameObjectsWithTag("CurrentMoves");
            int idx = 0;
            foreach (GameObject obj in objsCurrentMoves)
            {
                // obj.GetComponent<SpriteRenderer>().color = new Color(
                //     obj.GetComponent<SpriteRenderer>().color.r,
                //     obj.GetComponent<SpriteRenderer>().color.g,
                //     obj.GetComponent<SpriteRenderer>().color.b,
                //     Mathf.Lerp(0, 1.0f, (Time.time - (startRenderMovesTime + (float)idx * 0.15f)) / effectTime));
                // idx++;

                obj.GetComponent<SpriteRenderer>().color = new Color(
                     obj.GetComponent<SpriteRenderer>().color.r,
                     obj.GetComponent<SpriteRenderer>().color.g,
                     obj.GetComponent<SpriteRenderer>().color.b,
                     1.0f);
            }
        }
    }

    void DisplayResult(Result resultHit)
    {
        GameObject result;
        if(resultHit == Result.Perfect)
        {
            result = GameObject.Find("Result_Perfect");
            if(result != null)
            {
                result.transform.localPosition = new Vector3(0, yResultOffset, 0);
                result.GetComponent<SpriteRenderer>().enabled = true;
                result.GetComponent<Animator>().Rebind();
                result.GetComponent<Animator>().Play("good");
            }
        }
        else if(resultHit == Result.Great)
        {
            result = GameObject.Find("Result_Great");
            if(result != null)
            {
                result.transform.localPosition = new Vector3(0, yResultOffset, 0);
                result.GetComponent<SpriteRenderer>().enabled = true;
                result.GetComponent<Animator>().Rebind();
                result.GetComponent<Animator>().Play("good");
            }
        }
        else if(resultHit == Result.Cool)
        {
            result = GameObject.Find("Result_Cool");
            if(result != null)
            {
                result.transform.localPosition = new Vector3(0, yResultOffset, 0);
                result.GetComponent<SpriteRenderer>().enabled = true;
                result.GetComponent<Animator>().Rebind();
                result.GetComponent<Animator>().Play("good");
            }
        }
        else if(resultHit == Result.Bad)
        {
            result = GameObject.Find("Result_Bad");
            if(result != null)
            {
                result.transform.localPosition = new Vector3(0, yResultOffset, 0);
                result.GetComponent<SpriteRenderer>().enabled = true;
                result.GetComponent<Animator>().Rebind();
                result.GetComponent<Animator>().Play("good");
            }
        }
        else
        {
            result = GameObject.Find("Result_Miss");
            if(result != null)
            {
                result.transform.localPosition = new Vector3(0, yResultOffset, 0);
                result.GetComponent<SpriteRenderer>().enabled = true;
                result.GetComponent<Animator>().Rebind();
                result.GetComponent<Animator>().Play("good");
            }
        }
    }

    void DisplayAIResult(GameObject obj, int resultScore)
    {
        GameObject result;
        bool isPerfect = false;

        if(resultScore == (int)Result.Perfect)
        {
           result = GameObject.Find("Result_Perfect");
           isPerfect = true; // apply offset for special sprite
        }
        else if(resultScore == (int)Result.Great)
            result = GameObject.Find("Result_Great");
        else if(resultScore == (int)Result.Cool)
            result = GameObject.Find("Result_Cool");
        else if(resultScore == (int)Result.Bad)
            result = GameObject.Find("Result_Bad");
        else
            result = GameObject.Find("Result_Miss");

        if(result != null)
        {
            GameObject spawnInstance = Instantiate(result);
            spawnInstance.transform.SetParent(obj.transform);
            //if(isPerfect)
                spawnInstance.transform.localPosition = new Vector3(0,yResultOffset + 0.2f,0);
            //else
                //spawnInstance.transform.localPosition = Vector3.zero;
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
                //spawnInstance.transform.position = obj.transform.position;
                spawnInstance.transform.position = new Vector3(obj.transform.position.x,obj.transform.position.y,obj.transform.position.z-0.1f);
                spawnInstance.GetComponent<SpriteRenderer>().enabled = true;
                spawnInstance.SetActive(true); 
            }
            else
            {
                GameObject spawnInstance = Instantiate(moveBackgroundBad);
                spawnInstance.transform.SetParent(obj.transform);
                //spawnInstance.transform.position = obj.transform.position;
                spawnInstance.transform.position = new Vector3(obj.transform.position.x,obj.transform.position.y,obj.transform.position.z-0.1f);
                spawnInstance.GetComponent<SpriteRenderer>().enabled = true;
                spawnInstance.SetActive(true); 
            }
        }
    }

    void RenderTopScore()
    {
        /*
        if(currentLastTurnScore != GetPlayerLastTurnScore())
        {
            if(currentLastTurnScore > GetPlayerLastTurnScore())
                currentLastTurnScore--;
            else
                currentLastTurnScore++;
            playEffectScore = true;
        }

        TextMeshProUGUI textmeshPro = playerScoreTopText.GetComponent<TextMeshProUGUI>();
        if(textmeshPro != null)
        {
            textmeshPro.SetText("{0}", currentLastTurnScore);
            if(playEffectScore)
            {
                playerScoreTopText.GetComponent<Animator>().Rebind();
                playerScoreTopText.GetComponent<Animator>().Play("ScorePlayer");
                playEffectScore = false;
            }
        }
        */

        bool playEffectScore = false;
        if(currentScore < GetPlayerScore())
        {
            currentScore+=50;
            if(currentScore > GetPlayerScore())
                currentScore = GetPlayerScore();
            playEffectScore = true;
        }

        TextMeshProUGUI textmeshPro = playerScoreTopText.GetComponent<TextMeshProUGUI>();
        if(textmeshPro != null)
        {
            textmeshPro.SetText("{0}", currentScore);
            if(playEffectScore)
            {
                //playerScoreTopText.GetComponent<Animator>().Rebind();
                //playerScoreTopText.GetComponent<Animator>().Play("ScorePlayer");
            }
        }

        TextMeshProUGUI textmesh1stPro = playerScore1stText.GetComponent<TextMeshProUGUI>();
        if(textmesh1stPro != null)
        {
            textmesh1stPro.SetText("You: {0}", currentScore);
            if(playEffectScore)
            {
                playerScore1stText.GetComponent<Animator>().Rebind();
                playerScore1stText.GetComponent<Animator>().Play("ScorePlayer");
                playEffectScore = false;
            }
        }
    }

    void RenderAIScore()
    {
        TextMeshProUGUI textmesh2ndPro = playerScore2ndText.GetComponent<TextMeshProUGUI>();
        if(textmesh2ndPro != null)
        {
            textmesh2ndPro.SetText("AI1: {0}", GetAIScore(1));
            if(isAIChangedScore)
            {
                playerScore2ndText.GetComponent<Animator>().Rebind();
                playerScore2ndText.GetComponent<Animator>().Play("ScorePlayer");
            }
        }

        TextMeshProUGUI textmesh3rdPro = playerScore3rdText.GetComponent<TextMeshProUGUI>();
        if(textmesh3rdPro != null)
        {
            textmesh3rdPro.SetText("AI2: {0}", GetAIScore(2));
            if(isAIChangedScore)
            {
                playerScore3rdText.GetComponent<Animator>().Rebind();
                playerScore3rdText.GetComponent<Animator>().Play("ScorePlayer");
                isAIChangedScore = false;
            }
        }
    }

    int GetPlayerLastTurnScore()
    {
        return playerLastTurnScore;
    }

    int GetPlayerScore()
    {
        return playerScore;
    }

    int GetAIScore(int index)
    {
        if(index == 1)
            return ai1Score;
        else
            return ai2Score;
    }

    void moveSpeedBar()
    {

    }

    void moveSpeedBarSmooth()
    {
        SpeedBar.value += SpeedBmp * Time.deltaTime;
        Debug.Log("progress bar = " + SpeedBar.value);
        if(SpeedBar.value > 0.88f)
        {
            
            if(!lockCountNextMove)
            {
                if(playerTurn && isShowMove)
                {
                    if(currentMove < GenerateMove.instance.GetMove().Count)
                    {
                        HitMissResult();
                    }
                    AIDance();
                    playerTurn = false;
                }
                countNextMove++;
                lockCountNextMove  = true;
            }
        }
        if (SpeedBar.value >= 1.0f)
        {
            SpeedBar.value = 0.0f;
            lockCountNextMove = false;
            playerTurn = true;
        }
        if(countNextMove >= 2)
        {
            NextMove();
            countNextMove = 0;
        }
    }

    void HitMissResult()
    {
        DisplayResult(Result.Miss);
        playerDance(Result.Miss);
        resetForNextMove();
        
    }

    void AIDance()
    {
        int randScoreAI1 = Random.Range(0,ScoreBoard.Length);
        int randScoreAI2 = Random.Range(0,ScoreBoard.Length);
                    
        int AI1Score = ScoreBoard[randScoreAI1];
        int AI2Score = ScoreBoard[randScoreAI2];
                    // Control AI Dance
        AIController.instance.ControlAIDance(1, (Result)randScoreAI1);
        AIController.instance.ControlAIDance(2, (Result)randScoreAI2);
                    // Display AI Score
        DisplayAIResult(AIController.instance.GetAIResultPos(1), randScoreAI1);
        DisplayAIResult(AIController.instance.GetAIResultPos(2), randScoreAI2);
        ai1Score += AI1Score;
        ai2Score += AI2Score;
        isAIChangedScore = true;
    }

    Result IsResult()
    {
        if (SpeedBar.value >= 0.79f && SpeedBar.value <= 0.82f)
            return Result.Perfect;
        else if ((SpeedBar.value >= 0.77f && SpeedBar.value < 0.79f) || (SpeedBar.value > 0.82f && SpeedBar.value <= 0.84f))
            return Result.Great;
        else if ((SpeedBar.value >= 0.75f && SpeedBar.value < 0.77f) || (SpeedBar.value > 0.84f && SpeedBar.value <= 0.86f))
            return Result.Cool;
        else if ((SpeedBar.value >= 0.73f && SpeedBar.value < 0.75f) || (SpeedBar.value > 0.86f && SpeedBar.value <= 0.88f))
            return Result.Bad;
        else
            return Result.Miss;
    }
}
