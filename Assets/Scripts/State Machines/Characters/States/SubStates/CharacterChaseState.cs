﻿using Units.Attributes;
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
            if (Machine.Target == null) return;

            //float distanceFromPosition = Vector3.Distance(Machine.transform.position, Machine.HeldedPosition);

            //if (distanceFromPosition >= Machine.Combat.MaxDistanceFromPosition)
            //{
            //    Machine.SetDestionation(Machine.HeldedPosition);
            //    return;
            //}

            float distanceToTarget = Vector3.Distance(Machine.transform.position, Machine.Target.transform.position) - Machine.Target.ExtraRangeForAttack;

            if (distanceToTarget <= Machine.CurrentAttack.Distance)
            {
                if (Machine.CurrentAttackType == AttackType.Range)
                    SwitchState(Factory.Aim());
                else
                    SwitchState(Factory.Attack());
            }
        }

        public override void Enter()
        {
            Machine.OnAttackTypeSwitched += OnAttackTypeSwitched;
            Machine.SetDestination(Machine.Target.transform.position);
        }

        public override void Exit()
        {
            Machine.OnAttackTypeSwitched -= OnAttackTypeSwitched;
        }

        public override void InitializeSubState()
        {

        }

        public override void Update()
        {
            if (_curCheckDelay <= Time.time)
                UpdateTarget();

            if(Machine.Target != null)
                Machine.SetDestination(Machine.Target.transform.position);

            Machine.AnimationController.SetMoveSpeed(Machine.AIPath.velocity.magnitude);
            CheckSwitchStates();
        }

        protected override UnitHealth GetTarget()
        {
            _curCheckDelay = Time.time + _checkClosestTargetInterval;
            return base.GetTarget();
        }

        private void UpdateTarget()
        {
            Machine.Target = GetTarget();
        }

        private void OnAttackTypeSwitched(CharacterStateMachine machine, AttackType attackType)
        {
            UpdateTarget();
        }
    }
}