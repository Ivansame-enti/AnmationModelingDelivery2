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
        Transform[] legTargets = null;
        Transform[] legRoots = null;
        Transform[] legFutureBases = null;
        Vector3[][] copy = null;
        Transform[] legBones = null;
        MyTentacleController[] _legs = new MyTentacleController[6];
        private float[] distances = null;
        bool startWalk;


        #region public
        public void InitLegs(Transform[] LegRoots, Transform[] LegFutureBases, Transform[] LegTargets)
        {
            
            _legs = new MyTentacleController[LegRoots.Length];
            legBones = new Transform[LegRoots.Length];
            legRoots = new Transform[LegRoots.Length];
            legTargets = new Transform[LegTargets.Length];
            legFutureBases = new Transform[LegFutureBases.Length];

            for (int i = 0; i < _legs.Length; i++)
            {
                for (int x = 0; x < _legs[i].Bones.Length; x++)
                {
                    copy = new Vector3[_legs.Length][];
                    distances = new float[_legs[i].Bones.Length];
                }
            }
                    //copy = new Transform[LegRoots.Length][];
                    //joints = new Transform[LegRoots.Length];


                    //Legs init
                    for (int i = 0; i < LegRoots.Length; i++)
            {
                _legs[i] = new MyTentacleController();
                _legs[i].LoadTentacleJoints(LegRoots[i], TentacleMode.LEG);
                //legBones[i] = _legs[i].Bones;
                //joints[i] = LegRoots[i];
                legRoots[i] = LegRoots[i];
                legTargets[i] = LegTargets[i];
                legFutureBases[i] = LegFutureBases[i];
                Debug.Log(_legs[0].Bones[0]);

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
            //TODO: Initialize anything needed for the Gradient Descent implementation
        }

        //TODO: Check when to start the animation towards target and implement Gradient Descent method to move the joints.
        public void NotifyTailTarget(Transform target)
        {
            //tailTarget = target;
        }

        //TODO: Notifies the start of the walking animation
        public void NotifyStartWalk()
        {
            startWalk = true;
        }

        //TODO: create the apropiate animations and update the IK from the legs and tail

        public void UpdateIK()
        {

            //if (startWalk)
            //{
            //updateLegPos();
            updateLegs();
            //}
        }
        #endregion


        #region private
        //TODO: Implement the leg base animations and logic
        private void updateLegPos()
        {
            //check for the distance to the futureBase, then if it's too far away start moving the leg towards the future base position


        }
        //TODO: implement Gradient Descent method to move tail if necessary
        private void updateTail()
        {
            /*float gradient = 0;
            float angle = _tail.Bones[i];
            float p = DistanceFromTarget(target, _tail.Bones);
            _tail.Bones[i] += Time.deltaTime;
            float pDelta = DistanceFromTarget(target, _tail.Bones);
            gradient = (pDelta - p) / Time.deltaTime;
            _tail.Bones[i] = angle;
            return gradient;*/
        }
        //TODO: implement fabrik method to move legs 
        private void updateLegs()
        {
            fabrik();
        }

        public void fabrik()
        {
            //DICCIONARIO

            //JOINTS = LEGS.BONES
            //TARGET = legTargets
            // Copy the joints positions to work with

            for (int i = 0; i < _legs.Length; i++)
            {
                for (int x = 0; x < _legs[i].Bones.Length; x++)
                {
                    copy[i][x] = _legs[i].Bones[x].position;
                }

            }

            //TODO



            //done = true;

            for (int i = 0; i < _legs.Length; i++)
            {
                targetRootDist = Vector3.Distance(copy[i][0], legTargets[i].position);
            }
            // Update joint positions
            if (targetRootDist > distances.Sum())
            {
                Debug.Log("The target is unreachable");
                // The target is unreachable

                for (int i = 0; i < _legs.Length; i++)
                {
                    for (int x = 0; x < _legs[i].Bones.Length; x++)
                    {
                        _legs[i].Bones[x].transform.position = copy[i][x];
                        //TODO 
                    }

                }
            }
            else
            {

                // The target is reachable
                //while (TODO)

                // STAGE 1: FORWARD REACHING
                //TODO

                Vector3[][] inversePositions = new Vector3[_legs.Length][];
                for (int i = (copy.Length - 1); i >= 0; i--)
                {
                    for (int x = 0; x < _legs[i].Bones.Length; x++)
                    {
                        if (x == copy[x].Length - 1)
                        {
                            copy[i][x] = legTargets[i].position;
                            inversePositions[i][x] = legTargets[i].position;
                            //copyInverse[i] = target.position;
                        }
                        else
                        {
                            //Vector3 posPrimaSiguiente = copy[i + 1];
                            Vector3 posPrimaSiguiente = inversePositions[i][x + 1];
                            Vector3 posBaseActual = copy[i][x];
                            Vector3 direccion = (posBaseActual - posPrimaSiguiente).normalized;
                            float longitud = distances[x];
                            inversePositions[i][x] = posPrimaSiguiente + (direccion * longitud);
                        }
                    }
                }
                // STAGE 2: BACKWARD REACHING
                //TODO
                for (int i = 0; i < inversePositions.Length; i++)
                {
                    for (int x = 0; x < _legs[i].Bones.Length; x++)
                    {
                        if (x == 0)
                        {
                            copy[i][x] = _legs[i].Bones[0].position;
                        }
                        else
                        {
                            Vector3 posPrimaActual = inversePositions[i][x];
                            Vector3 posPrimaSegundaAnterior = copy[i][x - 1];
                            Vector3 direccion = (posPrimaActual - copy[i][x - 1]).normalized;
                            float longitud = distances[i - 1];
                            copy[i][x] = posPrimaSegundaAnterior + (direccion * longitud);
                        }
                    }
                }
            }

            // Update original joint rotations
            for (int i = 0; i < _legs[i].Bones.Length - 1; i++)
            {
                for (int x = 0; x < _legs[i].Bones.Length; x++)
                {

                    Vector3 joint01 = (_legs[i].Bones[x + 1].position - _legs[i].Bones[x].position).normalized;
                    Vector3 copy01 = (copy[i][x + 1] - copy[i][x]).normalized;


                    //Crossproduct
                    Vector3 crossBones01 = Vector3.Cross(joint01, copy01).normalized;
                    //Dot product
                    float angle01 = Mathf.Acos(Vector3.Dot(joint01, copy01)) * Mathf.Rad2Deg;

                    if (angle01 > 1f)
                    {
                        //joints[i].transform.Rotate(crossBones01, angle01, Space.World);
                        _legs[i].Bones[x].rotation = Quaternion.AngleAxis(angle01, crossBones01) * _legs[i].Bones[x].rotation;

                    }
                }
            }


        }
        #endregion
    }
}
