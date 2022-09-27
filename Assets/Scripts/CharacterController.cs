
using UnityEngine;

[RequireComponent(typeof (Character))]
public class CharacterController : MonoBehaviour
{
    [SerializeField] private float threshold = 0.1f;

    private Character _character;
    private CharacterStateBase _current;

    private void Awake()
    {
        _character = GetComponent<Character>();
        MakeTransitionToPatrol();
    }

    private void Start()
    {
        _current.Start();
        _character.OnEnemyGone += OnEnemyGone;
        _character.OnEnemySeen += OnEnemySeen;
    }

    private void OnEnemySeen(GameObject enemy)
    {
        _current.OnEnemySeen(enemy);
    }

    private void OnEnemyGone(GameObject enemy)
    {
        _current.OnEnemyGone(enemy);
    }

    private void Update()
    {
        _current.Update();
    }
    
    private void MakeTransitionToChase(GameObject enemy)
    {
        if (_current != null)
            _current.Exit();
        _current = new ChaseCharacterState(_character, this, threshold, enemy);
        _current.Start();
    }

    private void MakeTransitionToPatrol()
    {
        if (_current != null)
            _current.Exit();
        _current = new PatrolCharacterState(_character, this, threshold);
        _current.Start();
    }

    private void OnDestroy()
    {
        _character.OnEnemyGone -= OnEnemyGone;
        _character.OnEnemySeen -= OnEnemySeen;
    }

    public abstract class CharacterStateBase
    {
        protected Character controlled;
        protected CharacterController controller;

        public CharacterStateBase(Character controlled, CharacterController controller)
        {
            this.controlled = controlled;
            this.controller = controller;
        }

        public virtual void Start() { }
        public virtual void Exit() { }
        public virtual void OnEnemySeen(GameObject enemy) { }
        public virtual void OnEnemyGone(GameObject enemy) { }

        public abstract void Update();
    }

    public class PatrolCharacterState : CharacterStateBase
    {
        private int currentTarget;
        private float threshold;

        public PatrolCharacterState(Character controlled, CharacterController controller, float threshold) : base(controlled, controller)
        {
            this.threshold = threshold;
        }

        public override void Start()
        {
            currentTarget = 0;
        }

        public override void OnEnemySeen(GameObject enemy)
        {
            controller.MakeTransitionToChase(enemy);
        }

        public override void Update()
        {
            Vector3 distance = controlled.waypoints[currentTarget].position - controlled.transform.position;
            if (distance.magnitude < threshold)
                currentTarget = (currentTarget + 1) % controlled.waypoints.Length;
            else
                controlled.Move(distance);
        }
    }

    public class ChaseCharacterState : CharacterStateBase
    {
        private float _threshold;
        private GameObject _enemy;

        public ChaseCharacterState(Character controlled, CharacterController controller, float threshold, GameObject enemy) : base(controlled, controller)
        {
            _enemy = enemy;
            _threshold = threshold;
        }

        public override void OnEnemyGone(GameObject enemy)
        {
            controller.MakeTransitionToPatrol();
        }

        public override void Update()
        {
            if (_enemy == null)
            {
                controller.MakeTransitionToPatrol();
                return;
            }
            
            Vector3 distance = _enemy.transform.position - controlled.transform.position;
            controlled.Move(distance);
            if (distance.magnitude < _threshold)
                controlled.Attack(_enemy);
        }
    }
}
