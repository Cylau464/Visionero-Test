using Units.Attributes;
using UnityEngine;

namespace States.Characters
{
    public abstract class CharacterState : State
    {
        protected new CharacterStateMachine Machine => base.Machine as CharacterStateMachine;
        protected new CharacterStateFactory Factory => base.Factory as CharacterStateFactory;

        public CharacterState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory) {
            //machine._stateText.text = this.GetType().Name;
        }

        protected virtual void Dead(UnitHealth health)
        {
            SwitchState(Factory.Dead());
        }

        protected virtual UnitHealth GetTarget()
        {
            UnitHealth target = Machine.GetClosestTarget();

            if (target != null && target != Machine.Target)
            {
                if (Machine.Target != null)
                    Machine.Target.RemoveTargetedUnit(Machine);

                target.AddTargetedUnit(Machine);
            }

            return target;
        }

        protected virtual void UpdateTarget()
        {
            Machine.Target = GetTarget();
        }

        protected float GetDistanceToTarget()
        {
            float distanceToTarget = Vector3.Distance(Machine.transform.position, Machine.Target.transform.position);

            if (Machine.CurrentAttackType == AttackType.Melee)
                distanceToTarget -= Machine.Target.ExtraRangeForAttack;

            return distanceToTarget;
        }

        protected void RotateToTarget()
        {
            if (Machine.Target != null)
            {
                Vector3 direction = (Machine.Target.transform.position - Machine.transform.position).normalized;
                direction.y = 0f;
                Quaternion targetRot = Quaternion.LookRotation(direction);
                Machine.transform.rotation = Quaternion.RotateTowards(Machine.transform.rotation, targetRot, Machine.AIPath.rotationSpeed * Time.deltaTime);
            }
        }

        protected void UpdateAttackType()
        {
            if (Machine.Target != null && Machine.CurrentAttackType == AttackType.Range)
            {
                float distanceToTarget = GetDistanceToTarget();

                if (distanceToTarget <= Machine.Combat.Range.MinDistance)
                    Machine.SwitchAttackType(AttackType.Melee);
            }
        }

        protected void TryToAttack()
        {
            if (Machine.CurrentAttackCharged == false)
            {
                Machine.SetDestination(Machine.transform.position);
                return;
            }

            if (Machine.CurrentAttackType == AttackType.Range)
                SwitchState(Factory.Aim());
            else
                SwitchState(Factory.Attack(AttackType.Melee));
        }
    }
}