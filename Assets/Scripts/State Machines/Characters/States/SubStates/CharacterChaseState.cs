using Units.Attributes;
using UnityEngine;

namespace States.Characters
{
    public class CharacterChaseState : CharacterState
    {
        private float _checkClosestTargetInterval = 1f;
        private float _curCheckDelay;
        private float _ignoreTargetTime = 2f;
        private float _curIgnoreTargetTime;

        public CharacterChaseState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory)
        {
        }

        public override void CheckSwitchStates()
        {
            if (_curIgnoreTargetTime > Time.time) return;

            if (Machine.Target == null) return;

            if (ReturnToHeldedPosition() == true) return;

            float distanceToTarget = GetDistanceToTarget();

            if (distanceToTarget <= Machine.CurrentAttack.Distance)
                TryToAttack();
        }

        public override void Enter()
        {
            Machine.OnAttackTypeSwitched += OnAttackTypeSwitched;
            Machine.SetDestination(Machine.Target.transform.position);
        }

        public override void Exit()
        {
            Machine.OnAttackTypeSwitched -= OnAttackTypeSwitched;
            Machine.SetMoveSpeed(Machine.Movement.MoveSpeed);
        }

        public override void InitializeSubState()
        {

        }

        public override void Update()
        {
            if (_curCheckDelay <= Time.time)
                UpdateTarget();

            if(Machine.Target != null && _curIgnoreTargetTime <= Time.time)
                Machine.SetDestination(Machine.Target.transform.position);

            CheckSwitchStates();
        }

        protected override UnitHealth GetTarget()
        {
            _curCheckDelay = Time.time + _checkClosestTargetInterval;
            return base.GetTarget();
        }

        private void OnAttackTypeSwitched()
        {
            UpdateTarget();
        }

        private bool ReturnToHeldedPosition()
        {
            float distanceFromPosition = Vector3.Distance(Machine.transform.position, Machine.BattleHeldedPosition);

            if (distanceFromPosition >= Machine.Combat.MaxDistanceFromPosition)
            {
                Machine.SetDestination(Machine.BattleHeldedPosition);
                _curIgnoreTargetTime = Time.time + _ignoreTargetTime;
                Machine.SetMoveSpeed(Machine.Movement.MaxChaseMoveSpeed);
                return true;
            }

            return false;
        }
    }
}