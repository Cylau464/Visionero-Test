using NaughtyAttributes;
using States.Characters;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private AnimationCurve _trajectory;
    [SerializeField] private float _maxHeight;
    [SerializeField] private float _destroyAfterDestinationDelay = 2f;
    [SerializeField] private GameObject _hitParticle;
    [Space]
    [SerializeField] private bool _aoeDamage;
    [SerializeField, ShowIf(nameof(_aoeDamage))] private float _damageRadius;
    [SerializeField, ShowIf(nameof(_aoeDamage))] private float _slowDownRadius;
    [SerializeField, ShowIf(nameof(_aoeDamage)), Range(0f, 1f)] private float _slowDownModificator = .5f;
    [SerializeField, ShowIf(nameof(_aoeDamage))] private float _slowDownDuration = 2f;
    [SerializeField, ShowIf(nameof(_aoeDamage))] private LayerMask _targetMask;
    [SerializeField] private float _spreadRadius;

    public void Launch(UnitHealth target)
    {
        StartCoroutine(MoveToTarget(target));
    }

    public void Launch(Vector3 targetPos, bool hitTarget)
    {
        Vector2 spreadOffset;

        if (hitTarget == false)
        {
            spreadOffset = Random.insideUnitCircle * _spreadRadius;
            targetPos.x += spreadOffset.x;
            targetPos.z += spreadOffset.y;
        }

        StartCoroutine(MoveToPosition(targetPos));
    }

    private IEnumerator MoveToPosition(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float startDistance = Vector3.Distance(transform.position, targetPos);
        float distance = Vector3.Distance(transform.position, targetPos);

        while (distance > 0.2f)
        {
            float y = _trajectory.Evaluate(Mathf.InverseLerp(startDistance, 0.5f, distance)) * _maxHeight;
            Vector3 pos = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);
            distance = Vector3.Distance(pos, targetPos);
            //pos.y = startPos.y + y;
            transform.position = pos;
            transform.right = -(targetPos - transform.position).normalized;

            yield return null;
        }

        if (_aoeDamage == true)
        {
            Collider[] damageTargets = Physics.OverlapSphere(transform.position, _damageRadius, _targetMask);
            Collider[] slowDownTargets = Physics.OverlapSphere(transform.position, _slowDownRadius, _targetMask);

            foreach (Collider damageTarget in damageTargets)
            {
                if (damageTarget.TryGetComponent(out UnitHealth unit) == true)
                    unit.TakeHit();
            }

            foreach (Collider slowDownTarget in slowDownTargets)
            {
                if (slowDownTarget.TryGetComponent(out CharacterStateMachine unit) == true)
                    unit.SetMoveSpeedMofidicator(_slowDownModificator, _slowDownDuration);
            }
        }

        Instantiate(_hitParticle, transform.position, transform.rotation);
        Destroy(gameObject, _destroyAfterDestinationDelay);
    }

    private IEnumerator MoveToTarget(UnitHealth target)
    {
        Vector3 startPos = transform.position;
        float startDistance = Vector3.Distance(transform.position, target.transform.position);
        float distance = Vector3.Distance(transform.position, target.transform.position);

        while (distance > 0.2f)
        {
            float y = _trajectory.Evaluate(Mathf.InverseLerp(startDistance, 0.5f, distance)) * _maxHeight;
            Vector3 pos = Vector3.MoveTowards(transform.position, target.transform.position, _moveSpeed * Time.deltaTime);
            distance = Vector3.Distance(pos, target.transform.position);
            //pos.y = startPos.y + y;
            transform.position = pos;
            transform.right = -(target.transform.position - transform.position).normalized;
            
            yield return null;
        }

        target.TakeHit();
        Instantiate(_hitParticle, transform.position, transform.rotation);
        Destroy(gameObject, _destroyAfterDestinationDelay);
    }
}