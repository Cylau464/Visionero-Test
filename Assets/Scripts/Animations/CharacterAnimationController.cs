using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Animations
{
    public class CharacterAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private int _attackAnimationsCount = 2;

        private int _moveSpeedParamID;
        private int _inBattleParamID;
        private int _aimParamID;
        private int _attackParamID;
        private int _attackAnimIndexParamID;
        private int _deadParamID;

        public Action OnGiveDamage { get; set; }
        public Action OnAttackEnd { get; set; }

        private void Awake()
        {
            _moveSpeedParamID = Animator.StringToHash("move_speed");
            _inBattleParamID = Animator.StringToHash("in_battle");
            _aimParamID = Animator.StringToHash("aim");
            _attackParamID = Animator.StringToHash("attack");
            _attackAnimIndexParamID = Animator.StringToHash("attack_anim_index");
            _deadParamID = Animator.StringToHash("dead");
        }

        public void SetMoveSpeed(float moveSpeed)
        {
            _animator.SetFloat(_moveSpeedParamID, moveSpeed);
        }

        public void SetBattle(bool inBattle)
        {
            _animator.SetBool(_inBattleParamID, inBattle);
        }

        public void SetAim(bool state)
        {
            _animator.SetBool(_aimParamID, state);
        }

        public void Attack()
        {
            _animator.SetInteger(_attackAnimIndexParamID, Random.Range(0, _attackAnimationsCount));
            _animator.SetTrigger(_attackParamID);
        }

        public void Dead()
        {
            _animator.SetTrigger(_deadParamID);
        }

        public void GiveDamage()
        {
            OnGiveDamage?.Invoke();
        }

        public void AttackEnd()
        {
            OnAttackEnd?.Invoke();
        }
    }
}