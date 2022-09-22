using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public event Action<GameObject> OnEnemySeen;
    public event Action<GameObject> OnEnemyGone;
    
    [SerializeField] private float speed = 1f;
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private string enemyTag = "Enemy";
    
    public Transform[] waypoints;
    private Renderer _renderer;
    
    private void Awake ()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void Move(Vector3 direction)
    {
        direction = direction.normalized;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void Attack(GameObject enemy)
    {
        Debug.Log("Attack");
        _renderer.material.color = Color.red;
        Destroy(enemy);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(enemyTag))
        {
            if (IsInViewRange(other.transform))
                OnEnemySeen?.Invoke(other.gameObject);
            else
                OnEnemyGone?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(enemyTag))
            OnEnemyGone?.Invoke(other.gameObject);
    }

    private bool IsInViewRange(Transform other)
    {
        Vector3 distance = other.position - transform.position;
        return Vector3.Angle(distance, transform.forward) < viewAngle / 2f;
    }
}