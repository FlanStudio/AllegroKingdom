////////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
////////////////////////////////////////////////////////////////////////

﻿using UnityEngine;
using System.Collections;

public class AdventuressAnimationEventHandler : MonoBehaviour
{
    public AudioClip[] footStepsWalk;
    public AudioClip[] footStepsRun;

    [Header("Object Links")]
    [SerializeField]
    private Animator playerAnimator;

    [Header("Weapons")]
    public AudioClip[] swordSounds;
    public AudioClip[] daggerSounds;
    public AudioClip[] axeSounds;
    public AudioClip[] pickAxeSounds;
    public AudioClip[] hammerSounds;

    [Header("Swing")]
    public AudioClip[] swingSounds;

    [Header("PickUps")]
    public AudioClip[] PickUps;

    [SerializeField]
    private GameObject runParticles;

    private PlayerFoot foot_L;
    private PlayerFoot foot_R;

    #region private variables
    private bool hasPausedMovement;
    private readonly int canShootMagicHash = Animator.StringToHash("CanShootMagic");
    private readonly int isAttackingHash = Animator.StringToHash("IsAttacking");
    #endregion

    private void Awake()
    {
        GameObject L = GameObject.Find("toe_left");
        GameObject R = GameObject.Find("toe_right");
        if (L != null)
        {
            foot_L = L.GetComponent<PlayerFoot>();
        }
        else {
            print("Left foot missing");
        }
        if (R != null)
        {
            foot_R = R.GetComponent<PlayerFoot>();
        }
        else
        {
            print("Right foot missing");
        }
    }


    void enableWeaponCollider()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.equippedWeaponInfo != null)
        {
            PlayerManager.Instance.equippedWeaponInfo.EnableHitbox();
        }
    }

    void disableWeaponCollider()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.equippedWeaponInfo != null)
        {
            PlayerManager.Instance.equippedWeaponInfo.DisableHitbox();
        }

    }

    void ScreenShake()
    {
        PlayerManager.Instance.cameraScript.CamShake(new PlayerCamera.CameraShake(0.4f, 0.7f));
    }

    bool onCooldown = false;
    public enum FootSide { left, right };
    public void TakeFootstep(FootSide side)
    {
        if (foot_L != null && foot_R != null) {
            if (!PlayerManager.Instance.inAir && !onCooldown)
            {
                Vector3 particlePosition;

                if (side == FootSide.left )
                {
                    //if (foot_L.FootstepSound.Validate())
                    { 
                        // HINT: Play left footstep sound
                        particlePosition = foot_L.transform.position;
                        FootstepParticles(particlePosition);
                        AudioSource audioSource = GetComponent<AudioSource>();
                        if(Input.GetKey(KeyCode.LeftShift))
                            audioSource.PlayOneShot(footStepsWalk[Random.Range(0,5)], 0.7F);
                        else
                            audioSource.PlayOneShot(footStepsRun[Random.Range(0,5)], 0.7F);
                    }
                }
                else
                {
                    //if (foot_R.FootstepSound.Validate())
                    {
                        // HINT: Play right footstep sound
                        particlePosition = foot_R.transform.position;
                        FootstepParticles(particlePosition);
                        AudioSource audioSource = GetComponent<AudioSource>();
                        if (Input.GetKey(KeyCode.LeftShift))
                            audioSource.PlayOneShot(footStepsWalk[Random.Range(0, 5)], 0.7F);
                        else
                            audioSource.PlayOneShot(footStepsRun[Random.Range(0, 5)], 0.7F);
                    }
                }
            }
        }
    }

    void FootstepParticles(Vector3 particlePosition) {
        GameObject p = Instantiate(runParticles, particlePosition + Vector3.up * 0.1f, Quaternion.identity) as GameObject;
        p.transform.parent = SceneStructure.Instance.TemporaryInstantiations.transform;
        Destroy(p, 5f);
        StartCoroutine(FootstepCooldown());
    }

    IEnumerator FootstepCooldown()
    {
        onCooldown = true;
        yield return new WaitForSecondsRealtime(0.2f);
        onCooldown = false;
    }

    void ReadyToShootMagic()
    {
        PlayerManager.Instance.playerAnimator.SetBool(canShootMagicHash, true);
    }

    public enum AttackState { NotAttacking, Attacking };
    void SetIsAttacking(AttackState state)
    {
        if (state == AttackState.NotAttacking)
        {
            playerAnimator.SetBool(isAttackingHash, false);
        }
        else
        {
            playerAnimator.SetBool(isAttackingHash, true);
        }
    }

    public void Weapon_SwingEvent()
    {
        // PLAY SOUND
        Weapon W = PlayerManager.Instance.equippedWeaponInfo;
        // HINT: PlayerManager.Instance.weaponSlot contains the selected weapon;
        // HINT: This is a good place to play the weapon swing sounds
        AudioSource audioSource = GetComponent<AudioSource>();
        int indexSound = Random.Range(0, 3);
        if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_LeftSwing"))
        {
            audioSource.PlayOneShot(swingSounds[indexSound], 0.7F);
        }
        else if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_RightSwing"))
        {
            audioSource.PlayOneShot(swingSounds[indexSound + 4], 0.7F);
        }
        else if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_TopSwing"))
        {
            audioSource.PlayOneShot(swingSounds[indexSound + 8], 0.7F);
        }
    }

    public void PauseMovement()
    {
        if (!hasPausedMovement)
        {
            hasPausedMovement = true;
            PlayerManager.Instance.motor.SlowMovement();
        }
    }

    public void ResumeMovement()
    {
        if (hasPausedMovement)
        {
            hasPausedMovement = false;
            PlayerManager.Instance.motor.UnslowMovement();
        }
    }

    public void FreezeMotor()
    {
        StartCoroutine(PickupEvent());
    }

    private IEnumerator PickupEvent()
    {
        PlayerManager.Instance.PauseMovement(gameObject);
        yield return new WaitForSeconds(2f);
        PlayerManager.Instance.ResumeMovement(gameObject);
    }

    public void PickUpItem(PickUpType pickType)
    {
        PlayerManager.Instance.PickUpEvent();
        // HINT: This is a good place to play the Get item sound and stinger
        AudioSource audioSource = GetComponent<AudioSource>();
        switch (pickType)
        {
            case PickUpType.General:
                audioSource.PlayOneShot(PickUps[9], 0.7F);
                break;
            case PickUpType.Book:
                audioSource.PlayOneShot(PickUps[0], 0.7F);
                break;
            case PickUpType.Coin:
                audioSource.PlayOneShot(PickUps[Random.Range(1,4)], 0.7F);
                break;
            case PickUpType.Crystals:
                audioSource.PlayOneShot(PickUps[Random.Range(5, 7)], 0.7F);
                break;
            case PickUpType.EvilEssence:
                audioSource.PlayOneShot(PickUps[8], 0.7F);
                break;
            case PickUpType.Key:
                audioSource.PlayOneShot(PickUps[10], 0.7F);
                break;
            case PickUpType.Mushroom:
                audioSource.PlayOneShot(PickUps[11], 0.7F);
                break;
            case PickUpType.Pinecone:
                audioSource.PlayOneShot(PickUps[12], 0.7F);
                break;
            case PickUpType.Axe:
                audioSource.PlayOneShot(PickUps[13], 0.7F);
                break;
            case PickUpType.Dagger:
                audioSource.PlayOneShot(PickUps[14], 0.7F);
                break;
            case PickUpType.Hammer:
                audioSource.PlayOneShot(PickUps[15], 0.7F);
                break;
            case PickUpType.Pickaxe:
                audioSource.PlayOneShot(PickUps[16], 0.7F);
                break;
            case PickUpType.Sword:
                audioSource.PlayOneShot(PickUps[17], 0.7F);
                break;
        }
    }

    public void WeaponSound(int material)
    {
        Weapon EquippedWeapon = PlayerManager.Instance.equippedWeaponInfo;
        AudioSource audioSource = GetComponent<AudioSource>();
        // HINT: This is a good place to play equipped weapon impact sound

        switch (EquippedWeapon.weaponType)
        {
            case WeaponTypes.None:
                break;
            case WeaponTypes.Dagger:
                if (daggerSounds[material] != null)
                    audioSource.PlayOneShot(daggerSounds[material], 0.7F);
                break;
            case WeaponTypes.Sword:
                if (swordSounds[material] != null)
                    audioSource.PlayOneShot(swordSounds[material], 0.7F);
                break;
            case WeaponTypes.Axe:
                if (axeSounds[material] != null)
                    audioSource.PlayOneShot(axeSounds[material], 0.7F);
                break;
            case WeaponTypes.PickAxe:
                if (pickAxeSounds[material] != null)
                    audioSource.PlayOneShot(pickAxeSounds[material], 0.7F);
                break;
            case WeaponTypes.Hammer:
                if(hammerSounds[material] != null)
                    audioSource.PlayOneShot(hammerSounds[material], 0.7F);
                break;
            case WeaponTypes.EvilSpitPlant:
                break;
            case WeaponTypes.EvilCrawler:
                break;
            case WeaponTypes.EvilHead:
                break;
        }
    }
}
