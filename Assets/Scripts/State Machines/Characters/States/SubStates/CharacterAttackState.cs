using Units.Attributes;
using UnityEngine;

namespace States.Characters
{
    public class CharacterAttackState : CharacterState
    {
        private readonly AttackType _currentAttackType;

        public CharacterAttackState(CharacterStateMachine machine, CharacterStateFactory factory, AttackType attackType) : base(machine, factory)
        {
            _currentAttackType = attackType;
        }

        public override void CheckSwitchStates()
        {
            if (Machine.Target == null)
                SwitchState(Factory.FindTarget());
        }

        public override void Enter()
        {
            Machine.AnimationController.OnGiveDamage += GiveDamage;
            //Machine.AnimationController.OnAttackEnd += AttackEnd;
            Machine.SetDestination(Machine.transform.position);

            Attack();
        }

        public override void Exit()
        {
            Machine.AnimationController.OnGiveDamage -= GiveDamage;
            //Machine.AnimationController.OnAttackEnd -= AttackEnd;
        }

        public override void InitializeSubState()
        {

        }

        public override void Update()
        {
            CheckSwitchStates();
            RotateToTarget();
        }

        private void Attack()
        {
            Machine.AnimationController.Attack();
            Machine.ResetAttackPoint(_currentAttackType);
        }

        private void GiveDamage()
        {
            if (Machine.Target != null)
            {
                int accuracy;

                if (_currentAttackType == AttackType.Melee)
                    accuracy = Machine.Combat.Melee.Accuracy;
                else
                    accuracy = Machine.Combat.Range.Accuracy;

                if(accuracy >= Random.value * 100)
                    Machine.Target.TakeHit();
            }

            Machine.ChargeAttackPoint(Machine.CurrentAttackType, Machine.CurrentAttack.PrepareTime);
            SwitchState(Factory.FindTarget());
        }
    }
}