using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private VisualEffect leftArmVFX;
    [SerializeField] private VisualEffect rightArmVFX;
    [SerializeField] private VisualEffect headVFX;
    [SerializeField] private VisualEffect leftLegVFX;
    [SerializeField] private VisualEffect rightLegVFX;

    [SerializeField] private GameObject leftArmARCVFX;
    [SerializeField] private GameObject rightArmARCVFX;
    [SerializeField] private GameObject headARCVFX;
    [SerializeField] private GameObject leftLegARCVFX;
    [SerializeField] private GameObject rightLegARCVFX;
    [SerializeField] private GameObject rightArmBurstVFX;
    [SerializeField] private GameObject leftArmBurstVFX;

    private void Start()
    {
        StopAllVFX();
    }

    public void PlayVFX(string bodyPart)
    {
        switch (bodyPart)
        {
            case "L_Arm":
                leftArmVFX.Play();
                leftArmARCVFX.SetActive(true);
                break;
            case "R_Arm":
                rightArmVFX.Play();
                rightArmARCVFX.SetActive(true);
                break;
            case "Torso":
                headVFX.Play();
                headARCVFX.SetActive(true);
                break;
            case "L_Leg":
                leftLegVFX.Play();
                leftLegARCVFX.SetActive(true);
                break;
            case "R_Leg":
                rightLegVFX.Play();
                rightLegARCVFX.SetActive(true);
                break;
        }
    }

    public void StopAllVFX()
    {
        leftArmVFX.Stop();
        rightArmVFX.Stop();
        headVFX.Stop();
        leftLegVFX.Stop();
        rightLegVFX.Stop();

        leftArmARCVFX.SetActive(false);
        rightArmARCVFX.SetActive(false);
        headARCVFX.SetActive(false);
        leftLegARCVFX.SetActive(false);
        rightLegARCVFX.SetActive(false);
    }

    public IEnumerator PlayAndDisableVFX(GameObject vfx, float duration)
    {
        if (vfx != null)
        {
            vfx.SetActive(true);
            yield return new WaitForSeconds(duration);
            vfx.SetActive(false);
        }
    }

    public void PlayBurstVFX(string arm)
    {
        if (arm == "R_Arm")
            StartCoroutine(PlayAndDisableVFX(rightArmBurstVFX, 0.6f));
        else if (arm == "L_Arm")
            StartCoroutine(PlayAndDisableVFX(leftArmBurstVFX, 0.6f));
    }
}
