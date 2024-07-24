using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace keijo
{
    public class PlayerAudio : MonoBehaviour
    {
        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip defaultFootstep;
        public AudioClip woodFootstep;
        public AudioClip gravelFootstep;
        public AudioClip landFootstep;
        public AudioClip takeDamageClip;

        public void PlayFootstep()
        {
            RaycastHit hit;
            string objectName = "";
            Ray downRay = new Ray(transform.position, Vector3.down);
            if(Physics.Raycast(downRay, out hit, 1f))
            {
                objectName = hit.collider.gameObject.name;
            }
            if(objectName == "WoodFloor")
            {
                audioSource.PlayOneShot(woodFootstep);
            }
            else if(objectName == "Floor")
            {
                audioSource.PlayOneShot(gravelFootstep);
            }
            else
            {
                audioSource.PlayOneShot(defaultFootstep);
            }
        }

        public void PlayLanded()
        {
            audioSource.PlayOneShot(landFootstep);
        }

        public void PlayTakeDamage()
        {
            audioSource.PlayOneShot(takeDamageClip);
        }
    }
}
