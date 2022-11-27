using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace OctopusController
{

    public class MyScorpionController
    {
        //TAIL
        Transform tailTarget;
        Transform tailEndEffector;
        MyTentacleController _tail;
        float animationRange;
        float targetRootDist;

        //LEGS
        Transform[] _legTargets = null;
        Transform[] _legRoots = null;
        Transform[] _legFutureBases = null;
        MyTentacleController[] _legs = new MyTentacleController[6];
        bool _startWalk;
        private List<Vector3[]> _copy;
        private List<float[]> _distances;
        private float _legThreshold = 1.5f;



        #region public
        public void InitLegs(Transform[] LegRoots, Transform[] LegFutureBases, Transform[] LegTargets)
        {
            _legs = new MyTentacleController[LegRoots.Length];
            _legRoots = new Transform[LegRoots.Length];
            _legTargets = new Transform[LegTargets.Length];
            _legFutureBases = new Transform[LegFutureBases.Length];
            _copy = new List<Vector3[]>();
            _distances = new List<float[]>();

            //Legs init
            for (int i = 0; i < LegRoots.Length; i++)
            {
                _legs[i] = new MyTentacleController();
                _legs[i].LoadTentacleJoints(LegRoots[i], TentacleMode.LEG);
                _legRoots[i] = LegRoots[i];
                _legTargets[i] = LegTargets[i];
                _legFutureBases[i] = LegFutureBases[i];
                _copy.Add(new Vector3[_legs[i].Bones.Length]);
                _distances.Add(new float[_legs[i].Bones.Length]);

                for (int x = 0; x < _legs[i].Bones.Length; x++)
                {
                    if (x < _legs[i].Bones.Length - 1)
                    {
                        _distances[i][x] = (_legs[i].Bones[x + 1].position - _legs[i].Bones[x].position).magnitude;
                    }
                    else
                    {
                        _distances[i][x] = 0f;
                    }
                }
            }
        }

        public void InitTail(Transform TailBase)
        {
            _tail = new MyTentacleController();
            _tail.LoadTentacleJoints(TailBase, TentacleMode.TAIL);
        }

        //TODO: Check when to start the animation towards target and implement Gradient Descent method to move the joints.
        public void NotifyTailTarget(Transform target)
        {

        }

        //TODO: Notifies the start of the walking animation
        public void NotifyStartWalk()
        {
            _startWalk = true;
        }

        //TODO: create the apropiate animations and update the IK from the legs and tail

        public void UpdateIK()
        {

            if (_startWalk)
            {
                updateLegs();
                updateLegPos();
            }
        }
        #endregion


        #region private
        private void updateLegPos()
        {
            for (int i = 0; i < _legs.Length; i++)
            {
                if ((_legFutureBases[i].position - _legRoots[i].position).magnitude > _legThreshold)
                {
                    _legRoots[i].position = _legFutureBases[i].position;
                }
            }

        }

        private void updateTail()
        {

        }

        private void updateLegs()
        {
            for (int i = 0; i < _legs.Length; i++)
            {
                fabrik(_legs[i].Bones, _legTargets[i], _distances[i], _copy[i], _legFutureBases[i]);
            }

        }

        public void fabrik(Transform[] joints, Transform target, float[] distances, Vector3[] copy, Transform futurBase)
        {
            for (int i = 0; i < joints.Length; i++)
            {
                copy[i] = joints[i].position;
            }

            float targetRootDist = Vector3.Distance(copy[0], target.position);

            // Update joint positions
            if (targetRootDist > distances.Sum())
            {
                for (int i = 0; i <= joints.Length - 2; i++)
                {
                    joints[i].transform.position = copy[i];
                }
            }
            else
            {
                Vector3[] inversePositions = new Vector3[copy.Length];
                for (int i = (copy.Length - 1); i >= 0; i--)
                {
                    if (i == copy.Length - 1)
                    {
                        copy[i] = target.transform.position;
                        inversePositions[i] = target.transform.position;
                    }
                    else
                    {
                        Vector3 posPrimaSiguiente = inversePositions[i + 1];
                        Vector3 posBaseActual = copy[i];
                        Vector3 direccion = (posBaseActual - posPrimaSiguiente).normalized;
                        float longitud = distances[i];
                        inversePositions[i] = posPrimaSiguiente + (direccion * longitud);
                    }
                }
                for (int i = 0; i < inversePositions.Length; i++)
                {
                    if (i == 0)
                    {
                        copy[i] = joints[0].position;
                    }
                    else
                    {
                        Vector3 posPrimaActual = inversePositions[i];
                        Vector3 posPrimaSegundaAnterior = copy[i - 1];
                        Vector3 direccion = (posPrimaActual - copy[i - 1]).normalized;
                        float longitud = distances[i - 1];
                        copy[i] = posPrimaSegundaAnterior + (direccion * longitud);
                    }
                }
            }

            for (int i = 0; i < joints.Length - 1; i++)
            {
                Vector3 joint01 = (joints[i + 1].position - joints[i].position).normalized;
                Vector3 copy01 = (copy[i + 1] - copy[i]).normalized;

                Vector3 crossBones01 = Vector3.Cross(joint01, copy01).normalized;

                float angle01 = Mathf.Acos(Vector3.Dot(joint01, copy01)) * Mathf.Rad2Deg;

                if (angle01 > 1f)
                {
                    joints[i].rotation = Quaternion.AngleAxis(angle01, crossBones01) * joints[i].rotation;
                }
            }

        }
        #endregion
    }
}
