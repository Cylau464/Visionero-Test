using Pathfinding;
using Units.Attributes;
using Units.Modificators;
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
                float accuracy;

                if (_currentAttackType == AttackType.Melee)
                    accuracy = Mathf.Max(0f, Machine.Combat.Melee.Accuracy + Machine.Combat.Melee.Accuracy * Machine.AccuracyPseudoRandomMultiplier);
                else
                {
                    accuracy = Mathf.Max(0f, Machine.Combat.Range.Accuracy + Machine.Combat.Range.Accuracy * Machine.AccuracyPseudoRandomMultiplier);

                    AccuracyModificator moveSpeedModificator = Machine.Combat.Range.MoveSpeedAccuracyModificator;
                    float moveSpeedThresholdPercent = Mathf.InverseLerp(
                        moveSpeedModificator.Threshold.x,
                        moveSpeedModificator.Threshold.y,
                        Machine.Target.GetComponent<AIPath>().velocity.magnitude
                    ); //moveSpeedModificator.GetThresholdPercent(Machine.Target.GetComponent<AIPath>().velocity.magnitude);
                    accuracy *= Mathf.Lerp(moveSpeedModificator.Value.y, moveSpeedModificator.Value.x, moveSpeedThresholdPercent) / 100f;

                    AccuracyModificator distanceModificator = Machine.Combat.Range.DistanceAccuracyModificator;
                    float distanceThresholdPercent = Mathf.InverseLerp(
                        moveSpeedModificator.Threshold.x,
                        moveSpeedModificator.Threshold.y,
                        Vector3.Distance(Machine.Target.transform.position, Machine.transform.position)
                    );
                    accuracy *= Mathf.Lerp(distanceModificator.Value.y, distanceModificator.Value.x, distanceThresholdPercent) / 100f;
                }

                Machine._stateText.text = accuracy.ToString();

                if (accuracy >= Random.value * 100)
                {
                    Machine.Target.TakeHit();

                    if (Machine.AccuracyPseudoRandomMultiplier > 0)
                        Machine.AccuracyPseudoRandomMultiplier = 0;
                    else
                        Machine.AccuracyPseudoRandomMultiplier--;
                }
                else
                {
                    Machine.AccuracyPseudoRandomMultiplier++;
                }
            }

            Machine.ChargeAttackPoint(Machine.CurrentAttackType, Machine.CurrentAttack.PrepareTime);
            SwitchState(Factory.FindTarget());
        }
    }
}