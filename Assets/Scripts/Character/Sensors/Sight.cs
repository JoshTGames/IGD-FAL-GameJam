using System.Collections.Generic;
using UnityEngine;
using System;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

namespace JoshGames.AI.Sensors{
    // public interface ISee{ void RespondToSight(Sight sight);  }

    [Serializable] public class Sight{
        [SerializeField, Tooltip("The layers this class can see")] LayerMask mask;
        [SerializeField, Tooltip("Any layers passed here will stop objects being seen if behind an object which holds one of these layers")] LayerMask obstacleMask;
        [SerializeField, Tooltip("The distance this class can see up to")] float viewDistance = 2.0f;
        [SerializeField, Range(-1, 1), Tooltip("The angle around the 'viewDistance' (-1 = behind, 1 = front)")] float viewAngle = 0.75f;

        [SerializeField] DebugSettings debugSettings;
        public DebugSettings DEBUG_SETTINGS{
            get{ return debugSettings; }
        }
        public float VIEW_DISTANCE{
            get{ return viewDistance; }
        }
        public float VIEW_ANGLE{
            get{ return viewAngle; }
        }

        /// <summary>
        /// This constructor will hold information about how this class can see
        /// </summary>
        /// <param name="mask">The layers this class can see</param>
        /// <param name="viewDistance">The distance this class can see up to</param>
        /// <param name="viewAngle">The angle around the 'viewDistance'</param>
        public Sight(LayerMask mask, float viewDistance = 2.0f, float viewAngle = 0.75f){
            this.mask = mask;
            this.viewDistance = viewDistance;
            this.viewAngle = viewAngle;
        }

        /// <summary>
        /// This will use this class's variables to search for the objects defined by the layerMask and return a list of objects which have been detected
        /// </summary>
        /// <param name="parent">Used so that this object doesn't get picked up in the returning list</param>
        /// <param name="direction">Should be a normalised value depicting the direction the search function should be viewing</param>
        /// <returns>A list of objects which match the sight settings</returns>
        public List<Transform> Search(Vector2 position, Vector2 direction){
            Collider2D[] scannedObjects = Physics2D.OverlapCircleAll(position, viewDistance, mask);

            List<Transform> matchedObjects = new List<Transform>();
            foreach(Collider2D collider in scannedObjects){
                // Checks to see if object is inside the view angle
                if(Vector2.Dot(direction, ((Vector2)collider.transform.position - position).normalized) < viewAngle){ continue; }
                // If there is an obstacle in the way
                if(Physics2D.Raycast(position, direction, Vector2.Distance(collider.transform.position, position), obstacleMask)){ continue; }
                matchedObjects.Add(collider.transform);
            }
            return matchedObjects;
        }
    }

    [Serializable] public class DebugSettings{
        public bool doDebug = false;
        public Color debugColour = Color.red;
    }
}