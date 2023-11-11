using UnityEngine;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.AI.Sensors{
    public interface IHear{ void RespondToSound(Sound sound);  }
    
    [System.Serializable] public class Sound{
        [HideInInspector] public Vector3 position;
        [SerializeField] AudioClip audio;
        [SerializeField] float range = 1;

        /// <summary>
        /// This constructor will hold the information of a given sound
        /// </summary>
        /// <param name="position">The position the sound will be created on</param>
        /// <param name="range">The radial distance of the sound</param>
        /// <param name="audio">The audible sound that will be played</param>
        public Sound(Vector3 position, float range = 1, AudioClip audio = null){
            this.position = position;
            this.range = range;
            this.audio = audio;
        }

        /// <summary>
        /// Once called, this will "generate" the audio and attempt to trigger functions in nearby characters
        /// </summary>
        public void Create(){
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, range);
            foreach(Collider2D col in colliders){
                if(col.TryGetComponent(out IHear listener)){
                    listener.RespondToSound(this);
                }
            }
        }
    }
}