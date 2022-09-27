using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof (Character))]
public class EventReceiver : MonoBehaviour
{
    private Character _character;

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    private void Start()
    {
        _character.OnEnemySeen += SendOnEnemySeen;
        _character.OnEnemyGone += SendOnEnemyGone;
    }

    private void SendOnEnemySeen(GameObject enemy)
    {
        CustomEvent.Trigger(gameObject, "EnemySeen", enemy);
    }

    private void SendOnEnemyGone()
    {
        CustomEvent.Trigger(gameObject, "EnemyGone");
    }
    
    private void OnDestroy()
    {
        _character.OnEnemyGone -= SendOnEnemyGone;
        _character.OnEnemySeen -= SendOnEnemySeen;
    }
}
