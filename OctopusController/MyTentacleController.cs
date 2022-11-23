using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;




namespace OctopusController
{

    
    internal class MyTentacleController

    //MAINTAIN THIS CLASS AS INTERNAL
    {

        TentacleMode tentacleMode;
        Transform[] _bones;
        Transform _endEffectorSphere;

        public Transform[] Bones { get => _bones; }

        //Exercise 1.
        public Transform[] LoadTentacleJoints(Transform root, TentacleMode mode)
        {
            //TODO: add here whatever is needed to find the bones forming the tentacle for all modes
            //you may want to use a list, and then convert it to an array and save it into _bones
            tentacleMode = mode;

            switch (tentacleMode){
                case TentacleMode.LEG:

                    _bones = new Transform[4];
                    _bones[0] = root.GetChild(0);
                    _bones[1] = _bones[0].GetChild(1);
                    _bones[2] = _bones[1].GetChild(1);
                    _bones[3] = _bones[2].GetChild(1);

                    //TODO: in _endEffectorsphere you keep a reference to the base of the leg
                    _endEffectorSphere = _bones[3];
                    break;
                case TentacleMode.TAIL:
                    _bones = new Transform[6];
                    _bones[0] = root;

                    for (int i = 1; _bones[i - 1].childCount > 0; i++)
                        {
                            _bones[i] = _bones[i - 1].GetChild(1);
                        }

                    _endEffectorSphere = _bones[_bones.Length-1];

                    //TODO: in _endEffectorsphere you keep a reference to the red sphere 
                    break;
                case TentacleMode.TENTACLE:
                    //IMPLEMENTAR EL END EFECTOR
                    _bones = new Transform[53];

                    root = root.GetChild(0).GetChild(0);
                    _bones[0] = root;

                    for (int i = 1; _bones[i - 1].childCount > 0; i++)

                    {

                        _bones[i] = _bones[i - 1].GetChild(0);
                        //Debug.Log(_bones[i]); //FUNCIONA

                    };
                    //QUITAR ULTIMO ELEMENTO DEL BOUNDS[] 
                    _endEffectorSphere = _bones[_bones.Length - 2];

                    //TODO: in _endEffectorphere you  keep a reference to the sphere with a collider attached to the endEffector
                    break;
            }
            return Bones;
        }
    }
}
