using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace keijo
{
    public class Skeleton : EnemyAI
    {

        public override void RunPatrolState()
        {
            base.RunPatrolState();
        }


        public override void RunCombatState()
        {
            base.RunCombatState();

            bool playerInSight = HasLineOfSight(targetPlayer.position, 100);
            Debug.Log("in sight " + playerInSight);

            // if target is not lost
            if (playerInSight)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    agent.velocity = Vector3.zero;
                }
                agent.SetDestination(targetPlayer.position);
            }
            else
            {
                // if target not found after reaching last known location, go to alert mode
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    Debug.Log("Target lost!");

                    SwitchToAlertState();
                }
            }
        }

        public override void RunAlertState()
        {
            base.RunAlertState();
            // move around
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // return to patrol when back in nest
                /*if (returningToNest)
                {
                    Debug.Log("Returned to nest, back to patrolling");
                    alertTimer = maxAlertTimer;
                    returningToNest = false;
                    SwitchToPatrolState();
                    return;
                }*/

                // otherwise pick a random direction to look
                Vector3 point;
                if (RandomPoint(lastSightingPos, alertPatrolRange, out point))
                {
                    Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                    agent.SetDestination(point);
                }
            }

            // set movement animation
            if (agent.velocity.normalized.magnitude > 0)
            {
                animator.SetFloat("MoveSpeed", agent.velocity.normalized.magnitude);
            }


        }

        public override void RunDeadState()
        {
            base.RunDeadState();
        }
    }
}

