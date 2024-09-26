using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _hp;
    [SerializeField] private float _damage = 1;
    [SerializeField] private float _attackSpeed = 1;
    [SerializeField] private float _attackRange = 2;
    [SerializeField] private Animator _animatorController;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _bonusHealth;
    protected bool isDead = false;
    private bool isAttacking = false;

    private void Start()
    {
        _agent.SetDestination(SceneManager.Instance.Player.transform.position);
    }

    private void Update()
    {
        if (isDead) return;

        if (_hp <= 0)
        {
            Die();
            return;
        }

        var distance = Vector3.Distance(transform.position, SceneManager.Instance.Player.transform.position);
        if (distance <= _attackRange)
        {
            if (!isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
        else
        {
            _agent.SetDestination(SceneManager.Instance.Player.transform.position);
        }

        _animatorController.SetFloat("Speed", _agent.velocity.magnitude);
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        _agent.isStopped = true;
        _animatorController.SetTrigger("Attack");
        SceneManager.Instance.Player.TakeDamage(_damage);
        yield return new WaitForSeconds(_attackSpeed);
        isAttacking = false;
        _agent.isStopped = false;
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        SceneManager.Instance.RemoveEnemy(this);
        isDead = true;
        _animatorController.SetTrigger("Die");
        SceneManager.Instance.Player.AddHp(_bonusHealth);
    }
}
