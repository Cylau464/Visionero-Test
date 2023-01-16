using Units.Attributes;
using UnityEngine;

namespace States.Characters
{
    public class CharacterAttackState : CharacterState
    {
        private float _attackDelay;
        private bool _canAttack = true;
        private AttackType _currentAttackType;

        public CharacterAttackState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory)
        {
        }

        public override void CheckSwitchStates()
        {
            if (Machine.Target == null) return;

            float distanceToTarget = Vector3.Distance(Machine.transform.position, Machine.Target.transform.position) - Machine.Target.ExtraRangeForAttack;
            
            if (_canAttack == true && distanceToTarget > Machine.CurrentAttack.Distance)
                SwitchState(Factory.Chase());
        }

        public override void Enter()
        {
            Machine.Target.OnDead += OnTargetDead;

            Machine.AnimationController.OnGiveDamage += GiveDamage;
            Machine.AnimationController.OnAttackEnd += AttackEnd;
            Machine.AnimationController.SetMoveSpeed(0f);
        }

        public override void Exit()
        {
            if (Machine.Target != null)
            {
                Machine.Target.RemoveTargetedUnit();
                Machine.Target.OnDead -= OnTargetDead;
            }

            Machine.AnimationController.OnGiveDamage -= GiveDamage;
            Machine.AnimationController.OnAttackEnd -= AttackEnd;
        }

        public override void InitializeSubState()
        {

        }

        public override void Update()
        {
            CheckSwitchStates();

            // Почему-то атака не работает у противников
            if (_canAttack == true)
            {
                if (Machine.CurrentAttackType == AttackType.Range)
                {
                    float distanceToTarget = Vector3.Distance(Machine.transform.position, Machine.Target.transform.position);

                    if (distanceToTarget <= Machine.Combat.Range.MinDistance)
                        Machine.SwitchAttackType(AttackType.Melee);
                }

                if (Machine.AttackCharged == true)
                {
                    if (Machine.Target == null || Machine.Target.IsDead == true)
                        Machine.Target = GetTarget();

                    if (Machine.Target == null) return;

                    if (_attackDelay > 0f)
                    {
                        if (_attackDelay <= Time.time)
                            Attack();
                    }
                    else
                    {
                        StartAttack();
                    }
                }
            }

            if (Machine.Target != null)
            {
                Vector3 direction = (Machine.Target.transform.position - Machine.transform.position).normalized;
                direction.y = 0f;
                Machine.transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        private void StartAttack()
        {
            _currentAttackType = Machine.CurrentAttackType;

            if (_currentAttackType == AttackType.Melee)
                Attack();
            else
                Aim();
        }

        private void Aim()
        {
            Machine.AnimationController.Aim();
            _attackDelay = Time.time + Machine.Combat.Range.AimTime;
        }

        private void Attack()
        {
            Machine.AnimationController.Attack();
            Machine.ResetAttackPoint();
            _attackDelay = 0f;
            _canAttack = false;
        }

        private void GiveDamage()
        {
            if (Machine.Target != null)
            {
                if (_currentAttackType == AttackType.Range)
                {
                    if(Machine.Combat.Range.Accuracy >= Random.value * 100)
                        Machine.Target.TakeHit();
                }
                else
                {
                    Machine.Target.TakeHit();
                }
            }

            Machine.ChargeAttackPoint(Machine.CurrentAttack.PrepareTime);
        }

        private void AttackEnd()
        {
            _canAttack = true;
        }

        private void OnTargetDead(UnitHealth target)
        {
            Machine.Target.OnDead -= OnTargetDead;
            Machine.Target = GetTarget();

            if(Machine.Target != null)
                Machine.Target.OnDead += OnTargetDead;
        }

        protected virtual UnitHealth GetTarget()
        {
            UnitHealth target = Machine.GetClosestTarget();

            if (target != null && target != Machine.Target)
                target.AddTargetedUnit();

            return target;
        }
    }
}