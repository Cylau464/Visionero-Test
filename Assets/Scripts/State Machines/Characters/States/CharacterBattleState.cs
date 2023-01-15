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
            // Добавить логику перехода из ближней атаки в дальнюю при унчитожении всех ближайщих юнитов
            if (Machine.CurrentAttackType == AttackType.Range)
            {
                float distanceToTarget = Vector3.Distance(Machine.transform.position, Machine.Target.transform.position);

                if (distanceToTarget <= Machine.RangeAttack.MinDistance)
                    Machine.SwitchAttackType(AttackType.Melee);
            }
        }

        public override void Enter()
        {
            Machine.Health.OnDead += Dead;

            Machine.AnimationController.SetBattle(true);
            Machine.OnLostAllTargets += OnLostAllTargets;
        }

        public override void Exit()
        {
            Machine.Health.OnDead -= Dead;

            Machine.AnimationController.SetBattle(false);
            Machine.OnLostAllTargets -= OnLostAllTargets;
        }

        public override void InitializeSubState()
        {
            SetSubState(Factory.Chase());
        }

        public override void Update()
        {
            CheckSwitchStates();
        }

        private void OnLostAllTargets(CharacterStateMachine machine)
        {
            SwitchState(Factory.Neutral());
        }
    }
}
