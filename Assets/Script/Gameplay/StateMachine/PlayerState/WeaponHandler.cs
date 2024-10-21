using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private GameObject weaponLogic;
    [SerializeField] private GameObject effect;

    private void EnableWeapon(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            weaponLogic.SetActive(true);
        }
    }

    private void DisableWeapon(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            weaponLogic.SetActive(false);
        }
    }

    private void EnableParticle(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            effect.SetActive(true);
        }
    }

    private void DisableParticle(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            effect.SetActive(false);
        }
    }
}
