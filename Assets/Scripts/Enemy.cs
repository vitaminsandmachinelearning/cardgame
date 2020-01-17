using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public new string name;
    public int maxHealth;
    public int health;
    public int armor;
    public Sprite artwork;
    public string actionScript;
    public string rewardScript;
}
