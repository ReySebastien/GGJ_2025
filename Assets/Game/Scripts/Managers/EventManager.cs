using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;
using DG.Tweening;

namespace GGJ
{
    public class EventManager : Manager
    {
        [Header("---Parameters---")]
        [SerializeField] private AnimationCurve _spawnRateCurve;
        [SerializeField] private float _minSpawnRate = 15f;
        [SerializeField] private float _maxSpawnRate = 5f;
        [SerializeField] private float _spawnRateDuration = 120f;
        [SerializeField] private bool _forceEvent;
        [SerializeField] private EventType _forcedEvent;

        private Dictionary<EventType, GameEvent> _getEventByType = new Dictionary<EventType, GameEvent>();
        private List<GameEvent> _currentEvents = new List<GameEvent>();
        private Coroutine _spawnRateCoroutine;
        private float _timer;
        private NonStackableEvent _playingNonStackableEvent;


        private void Awake()
        {
            DictionaryInit();
            _currentEvents = new List<GameEvent>();
        }

        private void DictionaryInit()
        {
            _getEventByType = new Dictionary<EventType, GameEvent>()
            {
                { EventType.Straw, new StrawEvent() },
                { EventType.Tooth, new ToothEvent() },
                { EventType.Cigarette, new CigaretteEvent() },
                { EventType.GlassTilt, new GlassTilt() },
            };
        }

        void Start()
        {
            StartSpawningEvents();
        }

        #region Spawning Events

        private void StartSpawningEvents()
        {
            if (_spawnRateCoroutine != null)
                StopCoroutine(_spawnRateCoroutine);

            _spawnRateCoroutine = StartCoroutine(SpawningEvent());
        }

        private void StopSpawningEvents()
        {
            if (_spawnRateCoroutine == null)
                return;

            StopCoroutine(_spawnRateCoroutine);
            _spawnRateCoroutine = null;
        }

        private IEnumerator SpawningEvent()
        {
            float delay;
            float factor;

            while (true)
            {
                factor = _timer / _spawnRateDuration;
                delay = Mathf.Lerp(_minSpawnRate, _maxSpawnRate, _spawnRateCurve.Evaluate(factor));
                yield return new WaitForSeconds(delay);
                SpawnRandomEvent();
                _timer = Mathf.Clamp(_timer + delay, 0f, _spawnRateDuration);
            }
        }

        private void SpawnRandomEvent()
        {
            EventType eventType = 0;
            GameEvent gameEvent = null;
            bool isStackable = false;

            do
            {
                eventType = _forceEvent ? _forcedEvent : (EventType)Random.Range(0, Enum.GetValues(typeof(EventType)).Length);
                gameEvent = GetNewEvent(eventType);
                isStackable = gameEvent.GetIsStackable();
            } while (_playingNonStackableEvent != null && !isStackable);

            _currentEvents.Add(gameEvent);

            if (!isStackable)
                _playingNonStackableEvent = gameEvent as NonStackableEvent;

            gameEvent.OnStarted();
        }

        public void OnEventEnded(GameEvent gameEvent)
        {
            if (_playingNonStackableEvent == gameEvent)
                _playingNonStackableEvent = null;

            _currentEvents.Remove(gameEvent);
        }

        #endregion

        public GameEvent GetNewEvent(EventType type)
        {
            return _getEventByType[type];
        }

        void Update()
        {
            if (_currentEvents == null || _currentEvents.Count == 0)
                return;

            float delta = Time.deltaTime;

            for (int i = 0; i < _currentEvents.Count; i++)
            {
                if (_currentEvents[i] == null)
                    continue;

                _currentEvents[i].OnUpdate(delta);
            }
        }

        void FixedUpdate()
        {
            if (_currentEvents == null || _currentEvents.Count == 0)
                return;

            for (int i = 0; i < _currentEvents.Count; i++)
            {
                if (_currentEvents[i] == null)
                    continue;

                _currentEvents[i].OnFixedUpdate();
            }
        }

    }

    public enum EventType { Straw, Tooth, Cigarette, GlassTilt }

    public abstract class GameEvent
    {
        private EventManager _em;
        protected EventManager _eventManager
        {
            get
            {
                if (_em == null)
                    _em = GameManager.Instance.GetManager<EventManager>();

                return _em;
            }
        }

        public virtual bool GetIsStackable() => true;
        public abstract void OnStarted();
        public virtual void OnUpdate(float delta) { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnStopped() => _eventManager.OnEventEnded(this);
    }

    public abstract class NonStackableEvent : GameEvent { public override bool GetIsStackable() => false; }

    public class StrawEvent : NonStackableEvent
    {
        public override void OnStarted() => _eventManager.StartCoroutine(Strawing());

        private IEnumerator Strawing()
        {
            yield return Straw.Instance.Show().WaitForCompletion();
            OnStopped();
        }
    }

    public class ToothEvent : GameEvent
    {
        public override void OnStarted() { Debug.Log("Tooth"); }
    }
    public class CigaretteEvent : GameEvent
    {
        public override void OnStarted() { Debug.Log("cigarette"); }
    }
    public class GlassTilt : NonStackableEvent
    {
        public override void OnStarted()
        {
            _eventManager.StartCoroutine(Tilting());
        }

        private IEnumerator Tilting()
        {
            yield return Beer.Instance.TitlBeer().WaitForCompletion();
            OnStopped();
        }
    }
}
