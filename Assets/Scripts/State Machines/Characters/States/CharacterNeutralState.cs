using System;
using System.Diagnostics;
using Units.Attributes;
using UnityEngine;

namespace States.Characters
{
    public class CharacterNeutralState : CharacterState
    {
        private float _currentBattleDelay;

        public CharacterNeutralState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory)
        {
            IsRootState = true;
            InitializeSubState();
        }

        public override void CheckSwitchStates()
        {
            CheckTargets();
        }

        public override void Enter()
        {
            Machine.Health.OnDead += Dead;
            //Machine.OnFindTarget += OnFindTarget;
            //Machine.OnLostAllTargets += OnLostTarget;

            Machine.ChargeAttackPoint(AttackType.Melee, Machine.Combat.Melee.PrepareTime);
            Machine.SetDestination(Machine.HeldedPosition);

            //CheckTargets();
        }

        public override void Exit()
        {
            Machine.Health.OnDead -= Dead;
            //Machine.OnFindTarget -= OnFindTarget;
            //Machine.OnLostAllTargets -= OnLostTarget;
        }

        public override void InitializeSubState()
        {
            if (Machine.AIPath.reachedDestination == false)// Machine.Agent.ReachedDestinationOrGaveUp() == false)
                SetSubState(Factory.Move());
            else
                SetSubState(Factory.Idle());
        }

        public override void Update()
        {
            Machine.AnimationController.SetMoveSpeed(Machine.AIPath.velocity.magnitude);

            UpdateAttackType();
            CheckSwitchStates();
        }

        private void CheckTargets()
        {
            if (Machine.Targets.Count > 0)
            {
                UpdateTarget();

                if (Machine.DontAttackUntilReachedDestination == false)
                {
                    SwitchState(Factory.Battle());
                    return;
                }
                else if (Machine.AIPath.reachedDestination == true)
                {
                    SwitchState(Factory.Battle());
                    return;
                }

                if (Machine.CurrentAttackType == AttackType.Melee && Machine.UnitsGroup.CurrentBattleDelayAfterMove <= Time.time)
                    SwitchState(Factory.Battle());
            }
        }
    }
}