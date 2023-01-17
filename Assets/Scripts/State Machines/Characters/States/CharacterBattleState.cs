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

            if (Machine.CurrentAttackType == AttackType.Range)
            {
                float distanceToTarget = Vector3.Distance(Machine.transform.position, Machine.Target.transform.position);

                if (distanceToTarget <= Machine.Combat.Range.MinDistance)
                    Machine.SwitchAttackType(AttackType.Melee);
            }

            Machine.ChargeAttackPoint(Machine.CurrentAttackType, Machine.CurrentAttack.PrepareTime);
        }

        public override void Exit()
        {
            Machine.Health.OnDead -= Dead;

            Machine.OnLostAllTargets -= OnLostAllTargets;
            Machine.OnHeldedPositionSetted -= StopBattle;

            Machine.AnimationController.SetBattle(false);

            if (Machine.Target != null)
                Machine.Target.RemoveTargetedUnit();
        }

        public override void InitializeSubState()
        {
            SetSubState(Factory.Chase());
        }

        public override void Update()
        {
            CheckSwitchStates();

            //if (Machine.CurrentAttackType == AttackType.Range)
            //{
            //    float distanceToTarget = Vector3.Distance(Machine.transform.position, Machine.Target.transform.position);

            //    if (distanceToTarget <= Machine.Combat.Range.MinDistance)
            //        Machine.SwitchAttackType(AttackType.Melee);
            //}
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
