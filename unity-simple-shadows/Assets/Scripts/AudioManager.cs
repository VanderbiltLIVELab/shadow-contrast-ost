using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {


    public AudioClip startBGAudioClip;
    public AudioClip endBGAudioClip;

    public AudioClip correctAudioClip;
    public AudioClip wrongAudioClip;


    public AudioClip readyAudioClip;
    public AudioClip setAudioClip;
    public AudioClip goAudioClip; 
    public AudioClip levelAudioClip;
    public AudioClip completedAudioClip; 

    public AudioClip congratulationsAudioClip;
    public AudioClip missionCompletedAudioClip;

    public AudioClip oneAudioClip;
    public AudioClip twoAudioClip;
    public AudioClip threeAudioClip;
    public AudioClip fourAudioClip;
    public AudioClip fiveAudioClip;


    AudioSource[] audioSources;   

    //Play the music
    bool m_Play;
    //Detect when you use the toggle, ensures music isn’t played multiple times
    bool m_ToggleChange;


    void Start()
    {
        audioSources = GetComponents<AudioSource>();
        audioSources[0].clip = startBGAudioClip;
        //audioSources[2].clip = correctAudioClip;
        //audioSources[3].clip = wrongAudioClip;
        audioSources[1].PlayOneShot(readyAudioClip);
        audioSources[0].Play(); 
    }


    // Toggle if background music is muted.
    // Unmute music when experiment non in session (trial timer not active) 
    public void MuteBackgroundMusic(bool isMuted)
    {
        audioSources[0].mute = isMuted; 
    }

    /*public void PlaySelectSound()
    {
        audioSources[1].PlayOneShot(selectAudioClip); 
    }*/

    public void PlaySetAnchorSound()
    {
        MuteBackgroundMusic(true);
        audioSources[1].PlayOneShot(setAudioClip);
    }

    public void PlayStartSound()
    {
        MuteBackgroundMusic(true);
        audioSources[1].PlayOneShot(goAudioClip);
    }


    public void PlayCorrectSound()
    {
        audioSources[1].PlayOneShot(correctAudioClip);
    }

    public void PlayIncorrectSound()
    {
        audioSources[1].PlayOneShot(wrongAudioClip);
    }


    public void PlayRoundNumber(int level)
    {
        StartCoroutine(PlayRoundSound(level));     
    }

    IEnumerator PlayRoundSound(int level)
    {
        audioSources[1].PlayOneShot(levelAudioClip);
        yield return new WaitForSeconds(levelAudioClip.length);

        // switch statement here
        switch (level)
        {
            case 1:                                             // level 1                                         
                audioSources[1].PlayOneShot(oneAudioClip);
                yield return new WaitForSeconds(oneAudioClip.length);
                break;
            case 2:                                             // level 2
                audioSources[1].PlayOneShot(twoAudioClip);
                yield return new WaitForSeconds(twoAudioClip.length);
                break;
            case 3:                                            // level 3
                audioSources[1].PlayOneShot(threeAudioClip);
                yield return new WaitForSeconds(threeAudioClip.length);
                break;
            case 4:                                            // level 4
                audioSources[1].PlayOneShot(fourAudioClip);
                yield return new WaitForSeconds(fourAudioClip.length);
                break;
            case 5:                                            // level 5
                audioSources[1].PlayOneShot(fiveAudioClip);
                yield return new WaitForSeconds(fiveAudioClip.length);
                break;
            default:                                            // level 6 
                yield return new WaitForSeconds(oneAudioClip.length);
                break;
        }        
        audioSources[1].PlayOneShot(completedAudioClip);

    }

    public void PlayGameOver()
    {
        StartCoroutine(PlayEndSounds());
        audioSources[0].clip = endBGAudioClip;
        MuteBackgroundMusic(false);
        audioSources[0].Play();
    }

    IEnumerator PlayEndSounds()
    {
        audioSources[1].PlayOneShot(congratulationsAudioClip);
        yield return new WaitForSeconds(congratulationsAudioClip.length);
        audioSources[1].PlayOneShot(missionCompletedAudioClip);
        yield return new WaitForSeconds(missionCompletedAudioClip.length);
    }

}
