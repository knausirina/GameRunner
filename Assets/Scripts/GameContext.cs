using Pool;
using System.Collections;
using UI;
using UnityEngine;

public class GameContext : MonoBehaviour
{
    [SerializeField] private Views _views;
    [SerializeField] private GameObject _player;

    public Views Views => _views;
    public GameObject Player => _player;
}