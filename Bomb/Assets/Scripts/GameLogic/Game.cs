using System;
using System.Collections.Generic;
using Account;
using Common;
using ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace GameLogic
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private GlobalContext globalContext;
        public GameSettings GameSettings => globalContext.gameSettings;

        private List<Player> Players = new List<Player>();
        public int currentPlayerIndex = 0;
        public GameState State { get; private set; } = GameState.Inactive;
        public float gameTime = 0;
        private readonly Queue<Card> _cards = new Queue<Card>();
        public Card CurrentCard { get; private set; }
        private Bomb _bomb;
        private Explosion _explosion;
        private Lib.Event _event;
        private bool _isBlockedPrevPlayer = false;
        private int _lastSecond = -1;

        private void OnEnable()
        {
            globalContext = FindFirstObjectByType<GlobalContext>();
            _event = globalContext.MakeEvent();

#if UNITY_EDITOR
            GetComponent<Debugger>().enabled = true;
#endif
        }

        void Start()
        {
            _bomb = new Bomb(this);
            _explosion = new Explosion(this);
            var pdata = globalContext.PData();
            foreach (var playerInfo in pdata.players)
            {
                Players.Add(new Player(playerInfo));
            }

            List<string> cardsStrings = new List<string>(GameSettings.cards);
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

            currentPlayerIndex = rand.Next(Players.Count);

            Invoke(nameof(StartGame), 0.01f);
        }

        private void StartGame()
        {
            NextCard();
            StartRound();
        }

        public Player GetCurrentPlayer()
        {
            return Players[currentPlayerIndex];
        }

        private void SetCurrentPlayerIndex(int index)
        {
            currentPlayerIndex = index;
            _event.Call(Events.EvCurrentPlayerChanged);
        }

        private bool NextCard()
        {
            if (_cards.Count == 0)
            {
                var result = GetResult();
                if (result.Count > 1 && result[0].Score == result[1].Score)
                {
                    var rand = new Random();
                    var index = rand.Next(GameSettings.cards.Length);
                    _cards.Enqueue(new Card(GameSettings.cards[index], Utils.GetWordConditionRandom()));
                }
                else
                {
                    SetState(GameState.Result);
                    return false;
                }
            }

            CurrentCard = _cards.Dequeue();
            return true;
        }

        public void NextPlayer()
        {
            _isBlockedPrevPlayer = false;

            if (currentPlayerIndex == (Players.Count - 1))
            {
                SetCurrentPlayerIndex(0);
            }
            else
            {
                SetCurrentPlayerIndex(++currentPlayerIndex);
            }

            _bomb.TryAddBonusTime();
        }

        public bool PrevPlayer()
        {
            if (_isBlockedPrevPlayer)
                return false;

            if (currentPlayerIndex == 0)
            {
                SetCurrentPlayerIndex(Players.Count - 1);
            }
            else
            {
                SetCurrentPlayerIndex(--currentPlayerIndex);
            }

            _isBlockedPrevPlayer = true;
            return true;
        }

        public List<Player> GetResult()
        {
            List<Player> result = new List<Player>(Players);
            result.Sort((p1, p2) => p1.Score.CompareTo(p2.Score));
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
                    int currentSecond = Mathf.FloorToInt(gameTime);
                    if (currentSecond > _lastSecond)
                    {
                        _lastSecond = currentSecond;
                        var countdownTime = Mathf.FloorToInt(GameSettings.countdownTime);
                        var count = countdownTime - _lastSecond;
                        if (count > 0)
                        {
                            _event.Call(Events.EvCountDownTickChanged, countdownTime - _lastSecond);
                        }
                    }

                    if (gameTime >= GameSettings.countdownTime)
                    {
                        _bomb.Init();
                        _explosion.Init();
                        SetState(GameState.Play);
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

            gameTime += Time.deltaTime;
        }

        private void SetState(GameState state)
        {
            State = state;
            _event.Call(Events.EvGameStateChanged, State);
        }

        public void StartRound()
        {
            gameTime = 0;
            _lastSecond = -1;
            _isBlockedPrevPlayer = true;
            ;
            SetState(GameState.Countdown);
        }

        public void OnAlert()
        {
            _event.Call(Events.EvAlert);
        }

        public void OnReadyToStart()
        {
            if (NextCard())
            {
                NextPlayer();
                SetState(GameState.ReadyToStart);
            }
        }

        public void OnExplosion()
        {
            var player = GetCurrentPlayer();
            player.Explosion();
            SetState(GameState.Explosion);
        }
    }


    public class Explosion
    {
        private readonly Game _game;
        private float _duration = 0;
        private bool _isCountdown = false;

        public Explosion(Game game)
        {
            this._game = game;
        }

        private GameSettings GameSettings => _game.GameSettings;

        public void Init()
        {
            _duration = 0;
            _isCountdown = false;
        }

        public void Update(float dt)
        {
            if (!_isCountdown)
            {
                if (_duration > GameSettings.explosionCountdownTime)
                {
                    _isCountdown = true;
                    _game.OnReadyToStart();
                }

                _duration += dt;
            }
        }
    }


    public class Bomb
    {
        private readonly Game _game;

        public Bomb(Game game)
        {
            this._game = game;
        }

        private GameSettings GameSettings => _game.GameSettings;

        private float _duration = 0;
        private float _aliveTime = 0;
        private bool _isAlerted = false;
        private bool _isExploded = false;

        public void Init()
        {
            var rand = new Random();
            _aliveTime = GameSettings.minBombAliveTime +
                         (float)(rand.NextDouble() * (GameSettings.maxBombAliveTime - GameSettings.minBombAliveTime));
            Debug.Log("<><><> aliveTime " + _aliveTime.ToString());
            _duration = 0;
            _isAlerted = false;
            _isExploded = false;
        }


        public void TryAddBonusTime()
        {
            if (_isAlerted)
            {
                _aliveTime = _duration + GameSettings.bonusBombAliveTime;
            }
        }

        public void Update(float dt)
        {
            if (_isExploded)
            {
                Debug.Log("ERROR isExploded!");
                return;
            }

            if (!_isAlerted && (_aliveTime - _duration) < GameSettings.alertBombTime)
            {
                _game.OnAlert();
                _isAlerted = true;
            }

            if ((_aliveTime - _duration) <= 0)
            {
                _game.OnExplosion();
                _isExploded = true;
                return;
            }

            _duration += dt;
        }
    }


    public class Player
    {
        public String Name => _playerInfo.name;
        public int PresetId => _playerInfo.presetId;
        public int Score { get; private set; }

        private readonly PlayerInfo _playerInfo;

        public Player(PlayerInfo playerInfo)
        {
            _playerInfo = playerInfo;
            Score = 0;
        }

        public void Explosion()
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
}