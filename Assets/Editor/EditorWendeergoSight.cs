using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JoshGames.AI.Sensors;

/*
--- This code has has been written by Joshua Thompson (https://joshgames.co.uk) ---
        --- Copyright ©️ 2023-2024 Joshua Thompson. All Rights Reserved. ---
*/

[CustomEditor(typeof(WendeergoController))]
public class EditorWendeergoSight : Editor{
        private void OnSceneGUI(){
                WendeergoController wendeergo = (WendeergoController)target;
                
                if(wendeergo.TARGETS_TO_TRACK == null || !wendeergo.TARGETS_TO_TRACK.DEBUG_SETTINGS.doDebug){ return; }
                Sight settings = wendeergo.TARGETS_TO_TRACK;
                if(!settings.DEBUG_SETTINGS.doDebug){ return; }
                Handles.color = settings.DEBUG_SETTINGS.debugColour;
                Handles.DrawWireArc(wendeergo.transform.position, Vector3.forward, Vector3.right, 360, settings.VIEW_DISTANCE);     
                Vector3 viewAngleA = GetDirectionFromAngle(wendeergo.transform, -(Mathf.Acos(settings.VIEW_ANGLE) * Mathf.Rad2Deg));
                Vector3 viewAngleB = GetDirectionFromAngle(wendeergo.transform, (Mathf.Acos(settings.VIEW_ANGLE) * Mathf.Rad2Deg));
                Handles.DrawLine(wendeergo.transform.position, wendeergo.transform.position + viewAngleA * settings.VIEW_DISTANCE);
                Handles.DrawLine(wendeergo.transform.position, wendeergo.transform.position + viewAngleB * settings.VIEW_DISTANCE);
        }

        /// <summary>
        /// This function is useful for editor scripts to create a representation on the view cone of the object.
        /// </summary>
        /// <param name="obj">The object we are trying to get the direction from</param>
        /// <param name="angleInDegrees">The angle the vision will reach</param>
        /// <returns>A directional vector representing the vision of the sensor object</returns>
        Vector2 GetDirectionFromAngle(Transform obj, float angleInDegrees) => new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
