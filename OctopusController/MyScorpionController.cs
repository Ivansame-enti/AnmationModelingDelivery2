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
        MyTentacleController _tail;
        float[] _tailAngles = null;
        Vector3[] _tailAxis = null;
        Vector3[] _tailOffset = null;
        private float _deltaGradient = 0.1f; // Used to simulate gradient (degrees)
        private float _learningRate = 3.0f; // How much we move depending on the gradient
        private float _distanceThreshold = 5.0f;

        //LEGS
        Transform[] legTargets;
        Transform[] legFutureBases;
        MyTentacleController[] _legs = new MyTentacleController[6];


        #region public
        public void InitLegs(Transform[] LegRoots, Transform[] LegFutureBases, Transform[] LegTargets)
        {
            _legs = new MyTentacleController[LegRoots.Length];
            //Legs init
            for (int i = 0; i < LegRoots.Length; i++)
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
            _tailAngles = new float[_tail.Bones.Length];
            _tailAxis = new Vector3[_tail.Bones.Length];
            _tailOffset = new Vector3[_tail.Bones.Length];

            _tailAxis[0] = new Vector3(0, 0 ,1);
            _tailAxis[1] = new Vector3(1, 0, 0);
            _tailAxis[2] = new Vector3(1, 0, 0);
            _tailAxis[3] = new Vector3(1, 0, 0);
            _tailAxis[4] = new Vector3(1, 0, 0);
            _tailAxis[5] = new Vector3(1, 0, 0);

            for (int i = 0; i < _tailOffset.Length; i++)
            {
                if (i != 0) _tailOffset[i] = Quaternion.Inverse(_tail.Bones[i - 1].rotation) * (_tail.Bones[i].position - _tail.Bones[i-1].position);
                else _tailOffset[i] = _tail.Bones[i].position;
            }
        }

        //Check when to start the animation towards target and implement Gradient Descent method to move the joints.
        public void NotifyTailTarget(Transform target)
        {
            if (Vector3.Distance(_tail.Bones[_tail.Bones.Length - 1].position, target.position) < _distanceThreshold)
            {
                tailTarget = target;
            }
        }

        //Notifies the start of the walking animation
        public void NotifyStartWalk()
        {

        }

        //Create the apropiate animations and update the IK from the legs and tail
        public void UpdateIK()
        {
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

        //Implement Gradient Descent method to move tail if necessary
        private void updateTail()
        {
            if (tailTarget != null)
            {
                for (int i = 0; i < _tail.Bones.Length; i++)
                {
                    float gradient = CalculateGradient(tailTarget.position, _tailAngles, i, _deltaGradient);
                    _tailAngles[i] -= _learningRate * gradient;
                }

                for (int i = 0; i < _tail.Bones.Length; i++)
                {
                    if (_tailAxis[i].x == 1) _tail.Bones[i].localEulerAngles = new Vector3(_tailAngles[i], 0, 0);
                    else if (_tailAxis[i].y == 1) _tail.Bones[i].localEulerAngles = new Vector3(0, _tailAngles[i], 0);
                    else if (_tailAxis[i].z == 1) _tail.Bones[i].localEulerAngles = new Vector3(0, 0, _tailAngles[i]);
                }
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

            Quaternion rotation = _tail.Bones[0].transform.parent.rotation;

            for (int i = 1; i < _tail.Bones.Length; i++)
            {
                rotation *= Quaternion.AngleAxis(Solution[i - 1], _tailAxis[i - 1]);
                Vector3 nextPoint = prevPoint + rotation * _tailOffset[i];
                Debug.DrawLine(prevPoint, nextPoint);

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
