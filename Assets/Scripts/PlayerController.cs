using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Text healthText;
    public Text armorText;
    public Text energyText;

    GameController gc;

    void Awake()
    {
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    void Start()
    {
        PlayerData.health = PlayerData.maxHealth;
        PlayerData.energy = PlayerData.maxEnergy;
        PlayerData.armor = 0;
        UpdateUI();
    }

    public void StartTurn()
    {
        PlayerData.energy = PlayerData.maxEnergy;
        UpdateUI();
    }

    void UpdateUI()
    {
        healthText.text = "Health: " + PlayerData.health;
        armorText.text = "Armor: " + PlayerData.armor;
        energyText.text = "Energy: " + PlayerData.energy;
    }

    public void TakeDamage(int amount)
    {
        if (amount > PlayerData.armor)
        {
            amount -= PlayerData.armor;
            ChangeArmor(-PlayerData.armor);
            ChangeHealth(-amount);
        }
        else
            ChangeArmor(-amount);
        CheckForDeath();
    }

    public void ChangeHealth(int amount)
    {
        PlayerData.health += amount;
        if (PlayerData.health > PlayerData.maxHealth)
            PlayerData.health = PlayerData.maxHealth;
        UpdateUI();
    }

    public void ChangeArmor(int amount)
    {
        PlayerData.armor += amount;
        if (PlayerData.armor < 0)
            PlayerData.armor = 0;
        UpdateUI();
    }

    public void ChangeEnergy(int amount)
    {
        PlayerData.energy += amount;
        UpdateUI();
    }

    void CheckForDeath()
    {
        if (PlayerData.health <= 0)
            gc.AlertPlayerDeath();
    }

    public void EndTurn()
    {
        gc.EndPlayerTurn();
    }
}
