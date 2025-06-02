using System;
using System.Collections.Generic;
using Common;
using GameLogic;
using UnityEngine;
using Unity.VisualScripting;
using Random = System.Random;

public class Explosion
{
    private Game game;
    private float duration = 0;
    private bool isCountdown = false;

    public Explosion(Game game)
    {
       this.game = game;
    }

    public void init()
    {
        duration = 0;
        isCountdown = false;
    }

    public void Update(float dt)
    {
        if (!isCountdown)
        {
            if (duration > Constants.ExplosionCountdownTime)
            {
                isCountdown = true;
                game.OnReadyToStart();
            }

            duration += dt;
        }
    }
}


public class Bomb
{
    private Game game;
    private BombAnimation _animation;
    public Bomb(Game game)
    {
       this.game = game;
    }

    private float duration = 0;
    private float aliveTime = 0;
    private bool isAlerted = false;
    private bool isExploded = false;

    public void init()
    {
        var rand = new Random();
        aliveTime = Constants.MinBombAliveTime + (float)(rand.NextDouble() * (Constants.MaxBombAliveTime - Constants.MinBombAliveTime));
        Debug.Log("<><><> aliveTime " + aliveTime.ToString() );
        duration = 0;
        isAlerted = false;
        isExploded = false;

        _animation = UnityEngine.Object.FindFirstObjectByType<BombAnimation>();
        _animation.SetAlertAnim(isAlerted);
        _animation.PlayAnimStep(BombAnimStates.comes);
    }


    // Constants.BonusBombAliveTime - может тоже сделать небольшим рандомом вместо константы?
    public void tryAddBonusTime()
    {
        if (isAlerted)
        {
            aliveTime = duration + Constants.BonusBombAliveTime;
        }
    }

    public void Update(float dt)
    {
        if (isExploded)
        {
            Debug.Log("ERROR isExploded!");
            return;
        }

        // Анимации сделаны так, что могут переходить из состояния Alert обратно в обычное состояние. Возможно стоит добавить такой переход и в логике, например в методе tryAddBonusTime()
        if (!isAlerted && (aliveTime - duration) < Constants.AlertBombTime)
        {
            game.onAlert();
            isAlerted = true;
            _animation.SetAlertAnim(isAlerted);
        }

        if ((aliveTime - duration) <= 0)
        {
            game.OnExplosion();
            isExploded = true;
            _animation.PlayAnimStep(BombAnimStates.boom);
            return;
        }

        duration += dt;
    }
}


public class Player
{
    public String Name { get; private set; }
    public int Score { get; private set; }
    
    public Player(String name)
    {
        Name = name;
        Score = 0;
    }
    public void explosion()
    {
        Score += 1;
    }

}


public struct Card
{
    public string Word { get; private set; }
    public WordCondition Condition { get; private set; }

    public Card(string word, WordCondition condition)
    {
        Word = word;
        Condition = condition;
    }
}


public class Game : MonoBehaviour
{
    private GlobalContext _globalContext;
    public List<String> playerNames = new ();
    public List<Player> players_ = new List<Player>();
    public int currentPlayerIndex = 0;
    public GameState State { get; private set; } = GameState.Inactive;
    public float gameTime_ = 0;
    private Queue<Card> _cards = new Queue<Card>();
    public Card CurrentCard { get; private set; }
    private Bomb _bomb;
    private Explosion _explosion;
    private Lib.Event _event;
    private bool isBlockedPrevPlayer = false;
    
    private int _lastSecond = -1;
    

    private void OnEnable()
    {
        _globalContext = FindFirstObjectByType<GlobalContext>();
        _event = _globalContext.MakeEvent();
    }

    void Start()
    {
        this.AddComponent<Debugger>();
        Debug.Log("<><><> Game.Start");
        
        _bomb = new Bomb(this);
        _explosion = new Explosion(this);

        // UserPreferenceData userPreferenceData = UserPreference.Load();
        var pdata = _globalContext.PData();
        foreach (string playerName in pdata.playerNames)
        {
            players_.Add(new Player(playerName));
        }

        List<string> cardsStrings = new List<string>(Constants.Cards);
        cardsStrings.Shuffle();

        Random rand = new Random();
        var length = rand.Next(cardsStrings.Count);
        if (length == 0)
        {
            length += 1;
        }

        for (int i = 0; i < length; ++i)
        {
            var word = cardsStrings[i];
            _cards.Enqueue(new Card(word, Utils.GetWordConditionRandom()));
        }

        currentPlayerIndex = rand.Next(players_.Count);

        nextCard();
        startRound();
    }

    public Player getCurrentPlayer()
    {
        return players_[currentPlayerIndex];
    }

    void setCurrentPlayerIndex(int index)
    {
        currentPlayerIndex = index;
        _event.Call(Events.EvCurrentPlayerChanged);
    }

    bool nextCard()
    {
        if (_cards.Count == 0)
        {
            var result = GetResult();
            if (result.Count > 1 && result[0].Score == result[1].Score)
            {
                var rand = new Random();
                var index = rand.Next(Constants.Cards.Length);
                _cards.Enqueue(new Card(Constants.Cards[index], Utils.GetWordConditionRandom()));
            }
            else
            {
                setState(GameState.Result);
                return false;
            }
        }
        CurrentCard = _cards.Dequeue();
        return true;
    }

    public void nextPlayer()
    {
        isBlockedPrevPlayer = false;

        if (currentPlayerIndex == (players_.Count - 1))
        {
            setCurrentPlayerIndex(0);
        }
        else
        {
            setCurrentPlayerIndex(++currentPlayerIndex);
        }

        _bomb.tryAddBonusTime();
    }

    public bool prevPlayer()
    {
        if (isBlockedPrevPlayer)
            return false;

        if (currentPlayerIndex == 0)
        {
            setCurrentPlayerIndex(players_.Count - 1);
        }
        else
        {
            setCurrentPlayerIndex(--currentPlayerIndex);
        }

        isBlockedPrevPlayer = true;
        return true;
    }

    public List<Player> GetResult()
    {
        List<Player> result = new List<Player>(players_);
        result.Sort(delegate (Player p1, Player p2)
        {
            return p1.Score.CompareTo(p2.Score);
        });
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case GameState.Inactive:
                break;
            case GameState.Countdown:
                int currentSecond = Mathf.FloorToInt(gameTime_);
                if (currentSecond > _lastSecond)
                {
                    _lastSecond = currentSecond;
                    var countdownTime =  Mathf.FloorToInt(Constants.CountdownTime);
                    var count = countdownTime - _lastSecond;
                    if (count > 0)
                    {
                        _event.Call(Events.EvCountDownTickChanged, countdownTime - _lastSecond);
                    }
                }

                if (gameTime_ >= Constants.CountdownTime)
                {
                    _bomb.init();
                    _explosion.init();
                    setState(GameState.Play);
                }
                break;
            case GameState.Play:
                _bomb.Update(Time.deltaTime);
                break;
            case GameState.Explosion:
                _explosion.Update(Time.deltaTime);
                break;
            case GameState.Result:
                break;
        }
        gameTime_ += Time.deltaTime;
    }

    private void setState(GameState state)
    {
        State = state;
        _event.Call(Events.EvGameStateChanged, State);
    }

    public void startRound()
    {
        gameTime_ = 0;
        _lastSecond = -1;
        isBlockedPrevPlayer = true; ;
        setState(GameState.Countdown);
    } 
    
    public void onAlert()
    {
        _event.Call(Events.EvAlert);
    }    

    public void OnReadyToStart()
    {
        if (nextCard())
        {
            nextPlayer();
            setState(GameState.ReadyToStart);
        }
    }

    public void OnExplosion()
    {
        var player = getCurrentPlayer();
        player.explosion();
        setState(GameState.Explosion);
    }
}
