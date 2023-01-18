using Units.Attributes;
using UnityEngine;

namespace States.Characters
{
    public class CharacterBattleState : CharacterState
    {
        public CharacterBattleState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory)
        {
            IsRootState = true;
            InitializeSubState();
        }

        public override void CheckSwitchStates()
        {

        }

        public override void Enter()
        {
            Machine.Health.OnDead += Dead;

            Machine.OnHeldedPositionSetted += StopBattle;
            Machine.OnLostAllTargets += OnLostAllTargets;

            Machine.AnimationController.SetBattle(true);

            Machine.ChargeAttackPoint(Machine.CurrentAttackType, Machine.CurrentAttack.PrepareTime);
            Machine.BattleHeldedPosition = Machine.AIPath.position;
        }

        public override void Exit()
        {
            Machine.Health.OnDead -= Dead;

            Machine.OnLostAllTargets -= OnLostAllTargets;
            Machine.OnHeldedPositionSetted -= StopBattle;

            Machine.AnimationController.SetBattle(false);

            if (Machine.Target != null)
                Machine.Target.RemoveTargetedUnit(Machine);
        }

        public override void InitializeSubState()
        {
            SetSubState(Factory.FindTarget());
        }

        public override void Update()
        {
            Machine.AnimationController.SetMoveSpeed(Machine.AIPath.velocity.magnitude);

            UpdateAttackType();
            CheckSwitchStates();
        }

        private void OnLostAllTargets(CharacterStateMachine machine)
        {
            StopBattle();
        }

        private void StopBattle()
        {
            if (Machine.Combat.AttackType == AttackType.Both && Machine.CurrentAttackType == AttackType.Melee)
            {
                Machine.SwitchAttackType(AttackType.Range);
            }

            SwitchState(Factory.Neutral());
        }
    }
}
