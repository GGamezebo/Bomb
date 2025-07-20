using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;


namespace Lib.FSM
{
    
    public class FSMError : Exception
    {
        public FSMError(string message) : base(message)
        {
        }
    }

    public class FSMConfigError : Exception
    {
        public FSMConfigError(string message) : base(message)
        {
        }
    }

    public class FysomError : Exception
    {
        public object Event { get; }

        public FysomError(string msg, object eventObj = null) : base(msg)
        {
            Event = eventObj;
        }
    }

    public interface IFSMState
    {
        string Name { get; }
        FSM Fsm { get; set; }

        void Sync(FSM fsm);
        void Finit();
        void Enter(IFSMState prevState, object eventData);
        void Leave(object eventData);
        void Reenter(object eventData);
        void Interrupt(object eventData);
        bool CanTransit();
        void Update(float dt);
        void AddEvent(string eventName, object eventData = null);
    }

    public class FSMState : IFSMState
    {
        private readonly string _name;
        private FSM _fsm;

        public FSMState(string name)
        {
            _name = name;
        }

        public string Name => _name;

        public FSM Fsm
        {
            get => _fsm;
            set => _fsm = value;
        }

        public override string ToString()
        {
            return $"State({Name}, {Fsm})";
        }

        public virtual void Sync(FSM fsm)
        {
            _fsm = fsm;
        }

        public virtual void Finit()
        {
        }

        public virtual void Enter(IFSMState prevState, object eventData)
        {
        }

        public virtual void Leave(object eventData)
        {
        }

        public virtual void Reenter(object eventData)
        {
        }

        public virtual void Interrupt(object eventData)
        {
        }

        public virtual bool CanTransit() => true;

        public virtual void Update(float dt)
        {
        }

        public void AddEvent(string eventName, object eventData = null)
        {
            _fsm?.AddEvent(eventName, eventData);
        }
    }

    public class FSM
    {
        private const string ALL_STATES = "*";
        private const string SAME_DST = "=";
        private const string INIT_STATE = "__default_root_state";
        private const string INIT_EVENT_NAME = "__default_root_setup_event";
        private const string UPDATE_EVENT = "__update_event";
        private const int MAX_TRANSITIONS = 100;

        private readonly Dictionary<string, IFSMState> _statesMap;
        private readonly Dictionary<string, Dictionary<string, Tuple<string, Func<bool>>>> _transactionMap;
        private readonly Dictionary<string, Dictionary<string, string>> _eventTransitionMap;
        private string _currentStateId;
        private readonly string _final;
        private readonly List<Tuple<string, object>> _newEvents = new List<Tuple<string, object>>();
        private bool _isRunning;
        private bool _isDestroyed;
        private int _transitionsCount;

        private readonly Dictionary<Tuple<string, string>, List<Action<string, string>>> _callbacks =
            new Dictionary<Tuple<string, string>, List<Action<string, string>>>();

        public FSM(Dictionary<string, object> cfg)
        {
            if (!cfg.ContainsKey("initial"))
                throw new FSMConfigError($"Config doesn't have 'initial' {cfg}");

            var initial = (Dictionary<string, object>)cfg["initial"];
            var initialState = (string)initial["state"];
            var initialEvent = initial.ContainsKey("event") ? (string)initial["event"] : INIT_EVENT_NAME;

            if (!cfg.ContainsKey("transitions"))
                throw new FSMConfigError($"Config doesn't have 'transitions' {cfg}");

            var transitions = (List<Dictionary<string, object>>)cfg["transitions"];
            var conditions = cfg.ContainsKey("conditions")
                ? (Dictionary<string, Func<bool>>)cfg["conditions"]
                : new Dictionary<string, Func<bool>>();

            foreach (var cond in conditions)
            {
                if (cond.Value == null)
                    throw new FSMConfigError($"Condition '{cond.Key}' is not callable");
            }

            var customStates = cfg.ContainsKey("states")
                ? ((List<IFSMState>)cfg["states"]).ToDictionary(s => s.Name, s => s)
                : new Dictionary<string, IFSMState>();

            _statesMap = new Dictionary<string, IFSMState>
            {
                [INIT_STATE] = new FSMState(INIT_STATE),
                [initialState] = customStates.ContainsKey(initialState)
                    ? customStates[initialState]
                    : new FSMState(initialState)
            };

            foreach (var transition in transitions)
            {
                var src = transition.ContainsKey("src") ? transition["src"] : ALL_STATES;

                if (src is string srcStr)
                {
                    if (srcStr != ALL_STATES && !_statesMap.ContainsKey(srcStr))
                    {
                        _statesMap[srcStr] = customStates.ContainsKey(srcStr)
                            ? customStates[srcStr]
                            : new FSMState(srcStr);
                    }
                }
                else if (src is List<object> srcList)
                {
                    foreach (string source in srcList)
                    {
                        if (source == ALL_STATES)
                            throw new FSMConfigError("State * you can use only without another states");

                        if (!_statesMap.ContainsKey(source))
                        {
                            _statesMap[source] = customStates.ContainsKey(source)
                                ? customStates[source]
                                : new FSMState(source);
                        }
                    }
                }

                var dst = transition.ContainsKey("dst") ? transition["dst"] : null;
                dst = dst?.ToString() == SAME_DST ? src : dst;

                if (dst?.ToString() != ALL_STATES)
                {
                    var dsts = dst is string dstStr ? new List<object> { dstStr } : (List<object>)dst;
                    foreach (string d in dsts)
                    {
                        if (!_statesMap.ContainsKey(d))
                        {
                            _statesMap[d] = customStates.ContainsKey(d) ? customStates[d] : new FSMState(d);
                        }
                    }
                }
            }

            var dstsSet = new HashSet<string>();
            var eventsCheck = new Dictionary<Tuple<string, string>, HashSet<string>>();
            var conditionsForCheck = new Dictionary<Tuple<string, string>, HashSet<Func<bool>>>();
            var allActiveStates = _statesMap.Keys.Where(s => s != INIT_STATE).ToList();

            foreach (var transition in transitions)
            {
                var src = transition.ContainsKey("src") ? transition["src"] : ALL_STATES;
                // Then the conversion becomes simpler:
                var srcs = src is string srcStr
                    ? (srcStr == ALL_STATES ? allActiveStates : new List<string> { srcStr })
                    : ((List<object>)src).ConvertAll(x => x.ToString());

                foreach (string s in srcs)
                {
                    var dst = transition["dst"];
                    dst = dst?.ToString() == SAME_DST ? s : dst;
                    dstsSet.Add(dst.ToString());

                    if (transition.ContainsKey("event"))
                    {
                        var eventName = (string)transition["event"];
                        var key = Tuple.Create(s, dst.ToString());

                        if (!eventsCheck.ContainsKey(key))
                            eventsCheck[key] = new HashSet<string>();

                        if (eventsCheck[key].Contains(eventName))
                            throw new FSMConfigError($"duplicated event {eventName}");

                        eventsCheck[key].Add(eventName);
                    }
                    else if (transition.ContainsKey("condition"))
                    {
                        var conditionName = (string)transition["condition"];
                        if (!conditions.ContainsKey(conditionName))
                            throw new FysomError($"Condition '{conditionName}' doesn't exist");

                        var condition = conditions[conditionName];
                        var key = Tuple.Create(s, dst.ToString());

                        if (!conditionsForCheck.ContainsKey(key))
                            conditionsForCheck[key] = new HashSet<Func<bool>>();

                        if (conditionsForCheck[key].Contains(condition))
                            throw new FysomError($"Condition '{conditionName}' already exists");

                        conditionsForCheck[key].Add(condition);
                    }
                }
            }

            _final = cfg.ContainsKey("final") ? (string)cfg["final"] : null;
            if (_final != null && !dstsSet.Contains(_final))
                throw new FSMConfigError($"Final state '{_final}' doesn't have appropriate dst states");

            foreach (var state in _statesMap.Values)
            {
                state.Sync(this);
            }

            _transactionMap = new Dictionary<string, Dictionary<string, Tuple<string, Func<bool>>>>();
            _eventTransitionMap = new Dictionary<string, Dictionary<string, string>>();

            AddTransaction(_statesMap[INIT_STATE], _statesMap[initialState], initialEvent, null, _transactionMap,
                _eventTransitionMap);

            foreach (var transition in transitions)
            {
                var src = transition.ContainsKey("src") ? transition["src"] : ALL_STATES;
                var srcs = src is string srcStr
                    ? (srcStr == ALL_STATES 
                        ? allActiveStates 
                        : new List<string> { srcStr })
                    : ((List<object>)src).ConvertAll(x => x.ToString());

                var dst = transition["dst"];
                var eventName = transition.ContainsKey("event") ? (string)transition["event"] : null;

                foreach (string s in srcs)
                {
                    var dstState = dst?.ToString() == SAME_DST ? s : dst.ToString();
                    var conditionName = transition.ContainsKey("condition") ? (string)transition["condition"] : null;
                    var condition = conditionName != null ? conditions[conditionName] : null;

                    AddTransaction(_statesMap[s], _statesMap[dstState], eventName, condition, _transactionMap,
                        _eventTransitionMap);
                }
            }

            _currentStateId = INIT_STATE;

            var isCustomInitialEvent = initial.ContainsKey("event");
            if (!isCustomInitialEvent)
            {
                AddEvent(INIT_EVENT_NAME);
            }
        }

        public static FSM MakeFSMFromJSON(string jsonFile, List<IFSMState> states, Dictionary<string, Func<bool>> conditions)
        {
            if (!File.Exists(jsonFile))
                throw new FSMConfigError($"File '{jsonFile}' doesn't exist");

            var json = File.ReadAllText(jsonFile);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            data["states"] = states;
            data["conditions"] = conditions;
            return new FSM(data);
        }

        public void Finit()
        {
            foreach (var state in _statesMap.Values)
            {
                state.Finit();
            }

            _statesMap.Clear();
            _transactionMap.Clear();
            _callbacks.Clear();
            _newEvents.Clear();
            _isRunning = false;
            _isDestroyed = true;
        }

        public void AddCallback(string fromState, string toState, Action<string, string> callback)
        {
            var key = Tuple.Create(fromState, toState);
            if (!_callbacks.ContainsKey(key))
                _callbacks[key] = new List<Action<string, string>>();

            if (!_callbacks[key].Contains(callback))
                _callbacks[key].Add(callback);
        }

        public void RemoveCallback(string fromState, string toState, Action<string, string> callback)
        {
            var key = Tuple.Create(fromState, toState);
            if (_callbacks.ContainsKey(key) && _callbacks[key].Contains(callback))
                _callbacks[key].Remove(callback);
        }

        private void CallCallbacks(string fromState, string toState)
        {
            var key = Tuple.Create(fromState, toState);
            if (_callbacks.ContainsKey(key))
            {
                foreach (var callback in _callbacks[key])
                {
                    callback(fromState, toState);
                }
            }
        }

        public void AddEvent(string eventName, object eventData = null)
        {
            _newEvents.Add(Tuple.Create(eventName, eventData));

            if (_isRunning) return;

            _isRunning = true;
            try
            {
                Run();
            }
            finally
            {
                _isRunning = false;
            }
        }

        public bool Can(string eventName)
        {
            if (!_eventTransitionMap.ContainsKey(eventName))
                return false;

            if (!_eventTransitionMap[eventName].ContainsKey(_currentStateId))
                return false;

            var dst = _eventTransitionMap[eventName][_currentStateId];
            return !IsFinished() &&
                   _transactionMap.ContainsKey(_currentStateId) &&
                   _transactionMap[_currentStateId].ContainsKey(dst);
        }

        public string GetCurrentState() => _currentStateId;

        public bool IsFinished() => _final != null && _currentStateId == _final;

        public void Update(float dt)
        {
            var transitionCount = 0;
            while (true)
            {
                var transited = UpdateTransitions();
                if (!transited || _isDestroyed)
                    break;

                transitionCount++;
                if (transitionCount > MAX_TRANSITIONS)
                {
                    Console.WriteLine("Finite state machine has exceeded the maximum amount of transitions per tick");
                    break;
                }
            }

            if (!_isDestroyed)
            {
                _statesMap[_currentStateId].Update(dt);
            }
        }

        private bool UpdateTransitions()
        {
            if (!IsFinished() &&
                _statesMap[_currentStateId].CanTransit() &&
                _transactionMap.ContainsKey(_currentStateId))
            {
                foreach (var kvp in _transactionMap[_currentStateId])
                {
                    var dst = kvp.Key;
                    var (eventName, condition) = kvp.Value;

                    if (eventName == null && condition != null && condition())
                    {
                        PerformTransition(dst, null);
                        return true;
                    }
                }
            }

            return false;
        }

        private void PerformTransition(string dst, Action callback, bool forced = false)
        {
            var previousStateId = _currentStateId;
            if (forced)
            {
                _statesMap[_currentStateId].Interrupt(new Dictionary<string, object>());
            }
            else
            {
                _statesMap[_currentStateId].Leave(new Dictionary<string, object>());
            }

            _currentStateId = dst;
            _statesMap[_currentStateId].Enter(_statesMap[previousStateId], new Dictionary<string, object>());

            callback?.Invoke();

            if (!_isDestroyed)
            {
                CallCallbacks(previousStateId, _currentStateId);
            }
        }

        public IFSMState CurrentState => _statesMap[_currentStateId];

        private void AddUpdateEvent()
        {
            if (_transitionsCount > MAX_TRANSITIONS)
            {
                Console.WriteLine("Finite state machine has exceeded the maximum amount of transitions per tick");
                return;
            }

            AddEvent(UPDATE_EVENT);
            _transitionsCount++;
        }

        private void Run()
        {
            while (_newEvents.Count > 0)
            {
                var events = new List<Tuple<string, object>>(_newEvents);
                _newEvents.Clear();

                foreach (var (eventName, eventData) in events)
                {
                    ProcessEvent(eventName, eventData);
                }
            }
        }

        private void ProcessEvent(string eventName, object eventData)
        {
            eventData ??= new Dictionary<string, object>();

            if (!Can(eventName))
                throw new FSMError($"event {eventName} inappropriate in current state {_currentStateId}");

            var dst = _eventTransitionMap[eventName][_currentStateId];
            var (_, condition) = _transactionMap[_currentStateId][dst];

            if (condition != null && !condition())
                return;

            if (_currentStateId != dst)
            {
                var prevState = _statesMap[_currentStateId];
                prevState.Leave(eventData);

                _currentStateId = dst;
                var currentState = _statesMap[_currentStateId];
                currentState.Enter(prevState, eventData);

                CallCallbacks(prevState.Name, currentState.Name);
            }
            else
            {
                var currentState = _statesMap[_currentStateId];
                currentState.Reenter(eventData);
            }
        }

        private static void AddTransaction(
            IFSMState src,
            IFSMState dst,
            string eventName,
            Func<bool> condition,
            Dictionary<string, Dictionary<string, Tuple<string, Func<bool>>>> transactionMap,
            Dictionary<string, Dictionary<string, string>> eventTransitionMap)
        {
            if (!transactionMap.ContainsKey(src.Name))
                transactionMap[src.Name] = new Dictionary<string, Tuple<string, Func<bool>>>();

            transactionMap[src.Name][dst.Name] = Tuple.Create(eventName, condition);

            if (eventName != null)
            {
                if (!eventTransitionMap.ContainsKey(eventName))
                    eventTransitionMap[eventName] = new Dictionary<string, string>();

                eventTransitionMap[eventName][src.Name] = dst.Name;
            }
        }
    }

}