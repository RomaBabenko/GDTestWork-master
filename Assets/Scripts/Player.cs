using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float _hp = 100f;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _doubleAttackCooldown = 2;
    [SerializeField] private float _doubleDamageMultiplier = 2;
    [SerializeField] private float _movementSpeed = 4f;
    [SerializeField] private float _movementRunSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 7f;
    private float _attackRange = 2;
    private float _lastAttackTime = 0;
    private float _lastDoubleAttackTime = 0;
    private bool isDead = false;
    private bool isAttacking = false;
    private Vector3 _moveDirection = Vector3.zero;
    [SerializeField] private Animator _animatorController;
    [SerializeField] private Button _doubleAttackButton;
    [SerializeField] private Image _doubleAttackButtonImage;

    public float GetHp()
    {
        return _hp;
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (_hp <= 0)
        {
            Die();
            return;
        }

        if (!isAttacking)
        {
            HandleMovement();
        }

        UpdateDoubleAttackButtonState();
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            Die();
        }
    }

    private void UpdateDoubleAttackButtonState()
    {
        var enemies = SceneManager.Instance.Enemies;
        bool hasEnemyInRange = false;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];
            if (enemy == null) continue;

            var distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= _attackRange)
            {
                hasEnemyInRange = true;
                break;
            }
        }
        _doubleAttackButton.interactable = hasEnemyInRange;
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float currentSpeed = isRunning ? _movementSpeed * _movementRunSpeed : _movementSpeed;

        _moveDirection = new Vector3(moveHorizontal, 0, moveVertical).normalized;

        if (_moveDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            transform.Translate(_moveDirection * currentSpeed * Time.deltaTime, Space.World);
        }

        float animationSpeed = _moveDirection.magnitude * (isRunning ? 2f : 1f);
        _animatorController.SetFloat("Speed", animationSpeed);
    }

    private void Attack()
    {
        if (isDead) return;

        if (Time.time - _lastAttackTime < _attackCooldown)
        {
            return;
        }

        isAttacking = true;
        _animatorController.SetTrigger("Attack");
        _lastAttackTime = Time.time;

        float animationDuration = GetCurrentAnimationLength("sword attack");
        StartCoroutine(EndAttackAfterAnimation(animationDuration));

        var enemies = SceneManager.Instance.Enemies;
        Enemy closestEnemy = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];
            if (enemy == null)
            {
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= _attackRange)
            {
                closestEnemy = enemy;
                break;
            }
        }

        if (closestEnemy != null)
        {
            closestEnemy.TakeDamage(_damage);
        }
    }

    private void DoubleAttack()
    {
        if (isDead) return;

        if (Time.time - _lastDoubleAttackTime < _doubleAttackCooldown)
        {
            return;
        }

        isAttacking = true;
        _animatorController.SetTrigger("DoubleAttack");
        _lastDoubleAttackTime = Time.time;

        float animationDuration = GetCurrentAnimationLength("sword double attack");
        StartCoroutine(EndAttackAfterAnimation(animationDuration));

        var enemies = SceneManager.Instance.Enemies;
        Enemy closestEnemy = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];
            if (enemy == null)
            {
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= _attackRange)
            {
                closestEnemy = enemy;
                break;
            }
        }

        if (closestEnemy != null)
        {
            closestEnemy.TakeDamage(_damage * _doubleDamageMultiplier);
        }

        StartCoroutine(FillDoubleAttackButton());
    }

    private IEnumerator FillDoubleAttackButton()
    {
        _doubleAttackButtonImage.fillAmount = 0;
        float elapsed = 0f;

        while (elapsed < _doubleAttackCooldown)
        {
            elapsed += Time.deltaTime;

            _doubleAttackButtonImage.fillAmount = elapsed / _doubleAttackCooldown;
            yield return null;
        }

        _doubleAttackButtonImage.fillAmount = 1;
    }

    private IEnumerator EndAttackAfterAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);
        isAttacking = false;
    }

    private float GetCurrentAnimationLength(string animationName)
    {
        var clips = _animatorController.runtimeAnimatorController.animationClips;

        Debug.Log("Available animation clips:");
        foreach (var clip in clips)
        {
            Debug.Log(clip.name);
        }

        foreach (var clip in clips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        return 0f;
    }

    public void AddHp(float amount)
    {
        float MaxHp = 100f;
        _hp += amount;

        if (_hp > MaxHp)
        {
            _hp = MaxHp;
        }
    }

    private void Die()
    {
        isDead = true;
        _animatorController.SetTrigger("Die");

        SceneManager.Instance.GameOver();
    }
}
