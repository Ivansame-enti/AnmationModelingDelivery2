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
        Transform[] legBones = null;
        MyTentacleController[] _legs = new MyTentacleController[6];
        bool startWalk;
        private List<Vector3[]> copy;
        private List<float[]> distances;


        #region public
        public void InitLegs(Transform[] LegRoots, Transform[] LegFutureBases, Transform[] LegTargets)
        {
            Debug.Log("Tamaño: " + LegRoots.Length);
            _legs = new MyTentacleController[LegRoots.Length];
            legBones = new Transform[LegRoots.Length];
            legRoots = new Transform[LegRoots.Length];
            legTargets = new Transform[LegTargets.Length];
            legFutureBases = new Transform[LegFutureBases.Length];
            copy = new List<Vector3[]>();
            distances = new List<float[]>();

            //copy = new Transform[LegRoots.Length][];
            //joints = new Transform[LegRoots.Length];
            
            //Legs init
            for (int i = 0; i < LegRoots.Length; i++)
            {
                
                //DICCIONARIO
                _legs[i] = new MyTentacleController();
                _legs[i].LoadTentacleJoints(LegRoots[i], TentacleMode.LEG);
                //legBones[i] = _legs[i].Bones;
                //joints[i] = LegRoots[i];
                legRoots[i] = LegRoots[i];
                legTargets[i] = LegTargets[i];
                legFutureBases[i] = LegFutureBases[i];
                copy.Add(new Vector3[_legs[i].Bones.Length]);
                distances.Add(new float[_legs[i].Bones.Length]);
                
                for(int x = 0; x < _legs[i].Bones.Length; x++)
                {
                    Debug.Log("Dentro del for:" + x);
                    if (x < _legs[i].Bones.Length - 1)
                    {
                        distances[i][x] = (_legs[i].Bones[x + 1].position - _legs[i].Bones[x].position).magnitude;
                    }
                    else
                    {
                        distances[i][x] = 0f;
                    }
                }
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
            for(int i = 0; i < _legs.Length; i++)
            {
                
                fabrik(_legs[i].Bones,legTargets[i],distances[i],copy[i]);
            }
            
        }
        //COPIAR Y PEGAR DE FABRIK PERO LE PASAMOS LOS PARAMETROS QUE NECESITA FABRIK
        public void fabrik(Transform[] joints,Transform target,float[] distances, Vector3[] copy)
        {
            // Copy the joints positions to work with

            for (int i = 0; i < joints.Length; i++)
            {
                copy[i] = joints[i].position;
            }

            //TODO



            //done = true;

            float targetRootDist = Vector3.Distance(copy[0], target.position);

            // Update joint positions
            if (targetRootDist > distances.Sum())
            {
                Debug.Log("The target is unreachable");
                // The target is unreachable
                for (int i = 0; i <= joints.Length - 2; i++)
                {
                    joints[i].transform.position = copy[i];
                    Debug.Log(joints[2]);
                }
                //TODO 


            }

            else
            {

                // The target is reachable
                //while (TODO)

                // STAGE 1: FORWARD REACHING
                //TODO

                //CAMBIARE VARIABLE MAS ADELANTE

                Vector3[] inversePositions = new Vector3[copy.Length];
                for (int i = (copy.Length - 1); i >= 0; i--)
                {
                    if (i == copy.Length - 1)
                    {
                        copy[i] = target.transform.position;
                        inversePositions[i] = target.transform.position;
                        //copyInverse[i] = target.position;
                    }
                    else
                    {
                        //Vector3 posPrimaSiguiente = copy[i + 1];
                        Vector3 posPrimaSiguiente = inversePositions[i + 1];
                        Vector3 posBaseActual = copy[i];
                        Vector3 direccion = (posBaseActual - posPrimaSiguiente).normalized;
                        float longitud = distances[i];
                        inversePositions[i] = posPrimaSiguiente + (direccion * longitud);
                    }
                }
                // STAGE 2: BACKWARD REACHING
                //TODO
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
                //CAMBIAR MAS ADELANTE

            }

            // Update original joint rotations
            for (int i = 0; i < joints.Length - 1; i++)
            {
                Vector3 joint01 = (joints[i + 1].position - joints[i].position).normalized;
                Vector3 copy01 = (copy[i + 1] - copy[i]).normalized;


                //Crossproduct
                Vector3 crossBones01 = Vector3.Cross(joint01, copy01).normalized;
                //Dot product
                float angle01 = Mathf.Acos(Vector3.Dot(joint01, copy01)) * Mathf.Rad2Deg;

                if (angle01 > 1f)
                {
                    //joints[i].transform.Rotate(crossBones01, angle01, Space.World);
                    joints[i].rotation = Quaternion.AngleAxis(angle01, crossBones01) * joints[i].rotation;

                }
            }

        }
        #endregion
    }
}
