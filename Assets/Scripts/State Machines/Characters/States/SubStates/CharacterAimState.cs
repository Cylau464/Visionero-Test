using Units.Attributes;
using UnityEngine;

namespace States.Characters
{
    public class CharacterAimState : CharacterState
    {
        private float _aimTime;

        public CharacterAimState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory)
        {
        }

        public override void CheckSwitchStates()
        {
            if (Machine.Target != null)
            {
                if (_aimTime < Time.time)
                {
                    SwitchState(Factory.Attack());
                    return;
                }

                if (Vector3.Distance(Machine.Target.transform.position, Machine.transform.position) > Machine.CurrentAttack.Distance)
                {
                    SwitchState(Factory.Chase());
                }
                else
                {
                    if (Machine.CurrentAttackType == AttackType.Range)
                        SwitchState(Factory)
                }
            }
            else
            {
                SwitchState(Factory.FindTarget());
            }
        }

        public override void Enter()
        {
            Machine.AnimationController.Aim();
            _aimTime = Time.time + Machine.Combat.Range.AimTime;
        }

        public override void Exit()
        {

        }

        public override void InitializeSubState()
        {

        }

        public override void Update()
        {
            CheckSwitchStates();

            if (Machine.Target != null)
            {
                Vector3 direction = (Machine.Target.transform.position - Machine.transform.position).normalized;
                direction.y = 0f;
                Machine.transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        private void UpdateTarget()
        {
            Machine.Target = GetTarget();
        }
    }
}