using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] GameObject RweaponLogic;
    [SerializeField] GameObject LweaponLogic;

    public void EnableWeapon()
    {
        RweaponLogic.SetActive(true);
        LweaponLogic.SetActive(true);
    }

    public void DisableWeapon()
    {
        RweaponLogic.SetActive(false);
        LweaponLogic.SetActive(false);
    }
}