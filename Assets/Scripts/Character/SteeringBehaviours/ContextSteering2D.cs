using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSteering2D{
    #region DEBUGGING VARIABLES
    public Vector2[] GET_DIRECTIONS{ get{ return directions; }}
    public float[] GET_INTERESTS{ get{ return interests; }}
    public float[] GET_DANGER{ get{ return dangers; }}
    float[] interests, dangers;
    #endregion

    /// <summary>
    /// Used for optimisation and so that it doesn't hit itself
    /// </summary>
    LayerMask checkedMask;

    /// <summary>
    /// All the directions that will be checked in the character
    /// </summary>
    Vector2[] directions;

    /// <summary>
    /// The distance that will be checked 
    /// </summary>
    float rayDistance = 2f;    

    float addInterest = 0;
    float dangerMultiplier = 1;

    /// <summary>
    /// Constructor to generate the steering class to allow obstacle avoidance 
    /// </summary>
    /// <param name="checkedMask">Used for optimisation and so that it doesn't hit itself</param>
    /// <param name="numOfRays">Number of directions to check</param>
    /// <param name="rayDistance">The distance the rays will go in order to check for collisions</param>
    /// <param name="addInterest">Used to help use more direction available to the character</param>
    /// <param name="dangerMultiplier">Used to help balance out the added interest</param>
    public ContextSteering2D(LayerMask checkedMask, int numOfRays = 8, float rayDistance = 2f, float addInterest = 1, float dangerMultiplier = 1){
        this.checkedMask = checkedMask;
        this.rayDistance = rayDistance;
        this.addInterest = addInterest;
        this.dangerMultiplier = dangerMultiplier;

        // Create data holders
        this.directions = new Vector2[numOfRays];     
        this.interests = new float[numOfRays];  
        this.dangers = new float[numOfRays];  

        // Populate directions mathematically
        for(int i = 0; i < directions.Length; i++){
            float angle = (i * 2 * Mathf.PI / numOfRays); // i = currentSegment | 2 = diameter | numOfRays = numberOfSegments || "currentSegment * diameter * circle circumferance / numberOfSegments"
            this.directions[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)); // Plot directions using Cos (x) and Sin (y) functions. 
        }
    }

    public Vector2 Solve(Vector2 origin, Vector2 targetDirection){
        Vector2 calculatedDirection = Vector2.zero;

        for(int i = 0; i < directions.Length; i++){
            float interest = Vector2.Dot(targetDirection, directions[i]) + addInterest; // Calculate the dotproduct angle between the targetDirection and a direction surrounding the character

            RaycastHit2D hit = Physics2D.Raycast(origin, directions[i], rayDistance, checkedMask); // Fire ray in a direction around the character
            
            float danger = (hit.collider)? dangerMultiplier * (1 - (hit.distance / rayDistance)) : 0; // Increase distance the closer to the object we are.
            float interestSubDanger = Mathf.Clamp01(interest - danger); // Subtract the danger from the interest value to get a direction

            calculatedDirection += directions[i] * interestSubDanger; // Multiply the direction by the new length of the direction
            // FOR DEBUGGING
            interests[i] = interest;
            dangers[i] = danger;
        }

        return calculatedDirection.normalized;
    }   
}
