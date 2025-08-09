using System;
using System.Collections.Generic;
using Account;
using Common;
using ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace GameLogic
{
    public class Game : GameObserverMonoBehaviour
    {
        [SerializeField] 
        private GlobalContext globalContext;
        public GameSettings GameSettings => globalContext.gameSettings;
        private Lib.Event _event;

        private readonly List<Player> _players = new();
        public int currentPlayerIndex = 0;
        public GameState State { get; private set; } = GameState.Inactive;
        public float gameTime = 0;
        private readonly Queue<Card> _cards = new Queue<Card>();
        public Card CurrentCard { get; private set; }
        private Bomb _bomb;
        private Explosion _explosion;
        private bool _isBlockedPrevPlayer = false;
        private int _lastSecond = -1;

        private int _maxRandPlayerChoices;

        protected override void OnEnable()
        {
            base.OnEnable();
            globalContext = FindFirstObjectByType<GlobalContext>();
            _event = globalContext.MakeEvent();

#if UNITY_EDITOR
            GetComponent<Debugger>().enabled = true;
#endif
        }

        protected override void Subscribe()
        {
            EventListener.Add(Events.EvPlayerAdded, new Action<PlayerInfo>(OnPlayerAdded));
            EventListener.Add(Events.EvPlayerRemoved, new Action<PlayerInfo, int>(OnPlayerRemoved));
            EventListener.Add(Events.EvPlayerModified, new Action<PlayerInfo, int>(OnPlayerModified));
            EventListener.Add(Events.EvPlayerSwapped, new Action<int, int>(OnPlayerSwapped));
        }
        
        private static float EaseOutQuad(float t) 
        {
            return 1 - (1 - t) * (1 - t);
        }

        private void OnPlayerAdded(PlayerInfo playerInfo)
        {
            _players.Add(new Player(playerInfo, globalContext.PData().players.Count));
        }
        
        private void OnPlayerRemoved(PlayerInfo playerInfo, int playerIndex)
        {
            _players.RemoveAt(playerIndex);
            if (currentPlayerIndex == playerIndex)
            {
                int newPlayerIndex = currentPlayerIndex == _players.Count ? 0 : currentPlayerIndex;
                SetCurrentPlayerIndex(newPlayerIndex);
            }
        }      
        
        private void OnPlayerModified(PlayerInfo playerInfo, int playerIndex)
        {
            _players[playerIndex] = new Player(playerInfo, playerIndex);
        }    
        
        private void OnPlayerSwapped(int index1, int index2)
        {
            (_players[index1], _players[index2]) = (_players[index2], _players[index1]);
            if (currentPlayerIndex == index1 || currentPlayerIndex == index2)
            {
                SetCurrentPlayerIndex(currentPlayerIndex);
            }
        }

        void Start()
        {
            _bomb = new Bomb(this);
            _explosion = new Explosion(this);
            var pdata = globalContext.PData();
            for (int i = 0; i < pdata.players.Count; i++)
            {
                _players.Add(new Player(pdata.players[i], i));
            }

            List<string> cardsStrings = new List<string>(GameSettings.cards);
            cardsStrings.Shuffle();
            var cardNumbers = (int)((globalContext.PData().gameTime * 60) / ((GameSettings.maxBombAliveTime - GameSettings.minBombAliveTime) / 2.0));
            cardsStrings = cardsStrings.GetRange(0, cardNumbers);

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

            currentPlayerIndex = rand.Next(_players.Count);
            _event.Call(Events.EvCurrentPlayerChanged, GetCurrentPlayer());

            Invoke(nameof(StartGame), 0.01f);
        }

        private void StartGame()
        {
            NextCard();
            ResetRound();
            Random random = new Random();
            _maxRandPlayerChoices = 40 + random.Next(0, _players.Count);
            SetState(GameState.PlayerChoice);
        }

        public Player GetCurrentPlayer()
        {
            return _players[currentPlayerIndex];
        }

        private void SetCurrentPlayerIndex(int index)
        {
            currentPlayerIndex = index;
            _event.Call(Events.EvCurrentPlayerChanged, GetCurrentPlayer());
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

            if (currentPlayerIndex == (_players.Count - 1))
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
                SetCurrentPlayerIndex(_players.Count - 1);
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
            List<Player> result = new List<Player>(_players);
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
                case GameState.PlayerChoice:
                    var number = Mathf.Lerp(0, _maxRandPlayerChoices, EaseOutQuad(gameTime / GameSettings.playerChoiceTime));
                    var index = (int)number % _players.Count;
                    SetCurrentPlayerIndex(index);
                    if (gameTime > GameSettings.playerChoiceTime)
                    {
                        SetState(GameState.ReadyToStart);
                    }
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
            ResetRound();
            SetState(GameState.Countdown);
        }

        private void ResetRound()
        {
            gameTime = 0;
            _lastSecond = -1;
            _isBlockedPrevPlayer = true;
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
        public int Index { get; }

        public int Score { get; private set; }

        private readonly PlayerInfo _playerInfo;

        public Player(PlayerInfo playerInfo, int index)
        {
            _playerInfo = playerInfo;
            Index = index;
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