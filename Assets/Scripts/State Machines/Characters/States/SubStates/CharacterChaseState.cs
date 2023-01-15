using Units.Attributes;
using UnityEngine;

namespace States.Characters
{
    public class CharacterChaseState : CharacterState
    {
        private float _checkClosestTargetInterval = 1f;
        private float _curCheckDelay;

        public CharacterChaseState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory)
        {
        }

        public override void CheckSwitchStates()
        {
            // Учитывать дальность отхождения от позиции перед преследованием
            // 

            if (Machine.Target == null) return;

            float distanceToTarget = Vector3.Distance(Machine.transform.position, Machine.Target.transform.position) - Machine.Target.ExtraRangeForAttack;
            
            if (distanceToTarget <= Machine.CurrentAttack.Distance)
            {
                Machine.AIPath.destination = Machine.transform.position;
                //Machine.Agent.SetDestination(Machine.transform.position);
                SwitchState(Factory.Attack(Machine.Target));
            }
        }

        public override void Enter()
        {
            Machine.Target = GetTarget();
            Machine.OnAttackTypeSwitched += OnAttackTypeSwitched;
        }

        public override void Exit()
        {
            if (Machine.Target != null)
                Machine.Target.RemoveTargetedUnit();

            Machine.OnAttackTypeSwitched -= OnAttackTypeSwitched;
        }

        public override void InitializeSubState()
        {

        }

        public override void Update()
        {
            if (_curCheckDelay <= Time.time)
                UpdateTarget();

            Machine.AnimationController.SetMoveSpeed(Machine.AIPath/*Agent*/.velocity.magnitude);
            CheckSwitchStates();
        }

        protected virtual UnitHealth GetTarget()
        {
            UnitHealth target = Machine.GetClosestTarget();
            _curCheckDelay = Time.time + _checkClosestTargetInterval;

            if (target != null && target != Machine.Target)
                target.AddTargetedUnit();

            return target;
        }

        private void UpdateTarget()
        {
            Machine.Target = GetTarget();

            if (Machine.Target == null) return;

            Machine.AIPath.destination = Machine.Target.transform.position;
        }

        private void OnAttackTypeSwitched(CharacterStateMachine machine, AttackType attackType)
        {
            UpdateTarget();
        }
    }
}