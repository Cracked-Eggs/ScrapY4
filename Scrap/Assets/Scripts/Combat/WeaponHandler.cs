using System;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] GameObject RweaponLogic;
    [SerializeField] GameObject LweaponLogic;
    AudioManager audioManager;

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void EnableWeapon()
    {
        audioManager.PlayAttack();
        RweaponLogic.SetActive(true);
        LweaponLogic.SetActive(true);
    }

    public void DisableWeapon()
    {
        RweaponLogic.SetActive(false);
        LweaponLogic.SetActive(false);
    }
}