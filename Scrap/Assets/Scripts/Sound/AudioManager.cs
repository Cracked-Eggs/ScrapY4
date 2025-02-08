using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public Sound[] footstepSounds;
    public Sound[] attackSounds;

    private int footstepIndex = 0; // Tracks the current footstep index
    int attackIndex = 0;
    private bool isReversing = false; // Flag to track playback direction
    private bool isAReversing = false; // Flag to track playback direction

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
        }

        foreach (Sound s in footstepSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
        }
        
        foreach (Sound s in attackSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("The sound " + name + " has not been found.");
            return;
        }

        s.source.Play();
    }

    public void PlayFootsteps()
    {
        if (footstepSounds == null || footstepSounds.Length == 0)
            return;

        // Play the current footstep sound
        footstepSounds[footstepIndex].source.Play();

        // Adjust index based on direction
        if (isReversing)
        {
            footstepIndex--;
        }
        else
        {
            footstepIndex++;
        }

        // If we reach the end, reverse direction
        if (footstepIndex >= footstepSounds.Length)
        {
            footstepIndex = footstepSounds.Length - 2; // Start moving back from the second last sound
            isReversing = true;
        }
        // If we reach the start, switch back to forward direction
        else if (footstepIndex < 0)
        {
            footstepIndex = 1; // Start moving forward from the second sound
            isReversing = false;
        }
    }

    public void PlayAttack()
    {
        if (attackSounds == null || attackSounds.Length == 0)
            return;

        // Play the current footstep sound
        attackSounds[attackIndex].source.Play();

        // Adjust index based on direction
        if (isAReversing)
        {
            attackIndex--;
        }
        else
        {
            attackIndex++;
        }

        // If we reach the end, reverse direction
        if (attackIndex >= attackSounds.Length)
        {
            attackIndex = attackSounds.Length - 2; // Start moving back from the second last sound
            isAReversing = true;
        }
        // If we reach the start, switch back to forward direction
        else if (attackIndex < 0)
        {
            attackIndex = 1; // Start moving forward from the second sound
            isAReversing = false;
        }
    }
}
