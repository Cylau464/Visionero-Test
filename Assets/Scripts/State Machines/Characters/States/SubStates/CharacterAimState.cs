using Units.Attributes;
using UnityEngine;

namespace States.Characters
{
    public class CharacterAimState : CharacterState
    {
        private float _currentAimTime;

        public CharacterAimState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory)
        {
        }

        public override void CheckSwitchStates()
        {
            if (Machine.Target != null)
            {
                if (_currentAimTime < Time.time)
                {
                    SwitchState(Factory.Attack(AttackType.Range));
                    return;
                }

                if (GetDistanceToTarget() > Machine.Combat.Range.Distance)
                    UpdateTarget();
            }
            else
            {
                SwitchState(Factory.FindTarget());
            }
        }

        public override void Enter()
        {
            Machine.AnimationController.SetAim(true);
            _currentAimTime = Time.time + Machine.Combat.Range.AimTime;
            Machine.SetDestination(Machine.transform.position);
        }

        public override void Exit()
        {
            Machine.AnimationController.SetAim(false);
        }

        public override void InitializeSubState()
        {

        }

        public override void Update()
        {
            CheckSwitchStates();
            RotateToTarget();
        }
    }
}