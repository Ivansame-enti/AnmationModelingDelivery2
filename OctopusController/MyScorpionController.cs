using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace OctopusController
{

    public struct PositionRotation
    {
        Vector3 position;
        Quaternion rotation;

        public PositionRotation(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

        // PositionRotation to Vector3
        public static implicit operator Vector3(PositionRotation pr)
        {
            return pr.position;
        }
        // PositionRotation to Quaternion
        public static implicit operator Quaternion(PositionRotation pr)
        {
            return pr.rotation;
        }
    }

    public class MyScorpionController
    {
        //TAIL
        Transform tailTarget;
        Transform tailEndEffector;
        MyTentacleController _tail;
        float animationRange;
        float[] Solution = null;
        private float DeltaGradient = 0.1f; // Used to simulate gradient (degrees)
        private float LearningRate = 0.1f; // How much we move depending on the gradient

        //LEGS
        Transform[] legTargets;
        Transform[] legFutureBases;
        MyTentacleController[] _legs = new MyTentacleController[6];

        
        #region public
        public void InitLegs(Transform[] LegRoots,Transform[] LegFutureBases, Transform[] LegTargets)
        {
            _legs = new MyTentacleController[LegRoots.Length];
            //Legs init
            for(int i = 0; i < LegRoots.Length; i++)
            {
                _legs[i] = new MyTentacleController();
                _legs[i].LoadTentacleJoints(LegRoots[i], TentacleMode.LEG);
                //TODO: initialize anything needed for the FABRIK implementation
            }

        }

        public void InitTail(Transform TailBase)
        {
            _tail = new MyTentacleController();
            _tail.LoadTentacleJoints(TailBase, TentacleMode.TAIL);
            /*for (int i=0;i<_tail.Bones.Length;i++)
            {
                Debug.Log(_tail.Bones[i]);
            }*/
            //Debug.Log(_tail.Bones.Length);
            Solution = new float[_tail.Bones.Length];
            //TODO: Initialize anything needed for the Gradient Descent implementation
        }

        //TODO: Check when to start the animation towards target and implement Gradient Descent method to move the joints.
        public void NotifyTailTarget(Transform target)
        {
            tailTarget = target;
            //Debug.Log(Vector3.Distance(_tail.Bones[_tail.Bones.Length-1].position, target.position));
            if (Vector3.Distance(_tail.Bones[_tail.Bones.Length - 1].position, target.position) < 3) UpdateIK();
        }

        //TODO: Notifies the start of the walking animation
        public void NotifyStartWalk()
        {

        }

        //TODO: create the apropiate animations and update the IK from the legs and tail

        public void UpdateIK()
        {
            //TODO

            updateTail();
        }
        #endregion


        #region private
        //TODO: Implement the leg base animations and logic
        private void updateLegPos()
        {
            //check for the distance to the futureBase, then if it's too far away start moving the leg towards the future base position
            //
        }
        //TODO: implement Gradient Descent method to move tail if necessary
        private void updateTail()
        {
            for (int i = 0; i < _tail.Bones.Length; i++)
            {
                float gradient = CalculateGradient(tailTarget.position, Solution, i, DeltaGradient);
                Solution[i] -= LearningRate * gradient;
            }

            for (int i = 0; i < _tail.Bones.Length; i++)
            {
                //_tail.Bones[i]. SetAngle(Solution[i]);
            }

            
        }

        
        private float CalculateGradient(Vector3 target, float[] Solution, int i, float delta)
        {
            float gradient = 0;
            float angle = Solution[i];
            float p = DistanceFromTarget(target, Solution);
            Solution[i] += delta;
            float pDelta = DistanceFromTarget(target, Solution);
            gradient = (pDelta - p) / delta;
            Solution[i] = angle;
            return gradient;
        }

        private float DistanceFromTarget(Vector3 target, float[] Solution)
        {
            Vector3 point = ForwardKinematics(Solution);
            return Vector3.Distance(point, target);
        }

        public PositionRotation ForwardKinematics(float[] Solution)
        {
            Vector3 prevPoint = _tail.Bones[0].transform.position;

            // Takes object initial rotation into account
            Quaternion rotation = _tail.Bones[0].transform.rotation;
            float angle;
            Vector3 axis;
            //TODO
            for (int i = 1; i < _tail.Bones.Length; i++)
            {
                _tail.Bones[i - 1].rotation.ToAngleAxis(out angle, out axis);
                rotation *= Quaternion.AngleAxis(Solution[i - 1], axis);
                Vector3 nextPoint = prevPoint + rotation * _tail.Bones[i].transform.localPosition;

                prevPoint = nextPoint;
            }

            // The end of the effector
            return new PositionRotation(prevPoint, rotation);
        }

        //TODO: implement fabrik method to move legs 
        private void updateLegs()
        {

        }
        #endregion
    }
}
