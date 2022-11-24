using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace OctopusController
{
    public enum TentacleMode { LEG, TAIL, TENTACLE };

    public class MyOctopusController
    {

        MyTentacleController[] _tentacles = new MyTentacleController[4];

        Transform _currentRegion;
        Transform _target;

        Transform[] _randomTargets;// = new Transform[4];


        float _twistMin, _twistMax;
        float _swingMin, _swingMax;
        bool region1b = false;
        bool region2b = false;
        bool region3b = false;
        bool region4b = false;
        Transform ramdomTargetPositionFinal;

        [SerializeField]
        GameObject[] joints;

        // The target for the IK system
        [SerializeField]
        GameObject targ;

        // Array of angles to rotate by (for each joint), as well as sin and cos values
        [SerializeField]
        float[] _theta, _sin, _cos;

        // To check if the target is reached at any point
        bool _done = false;

        // To store the position of the target
        private Vector3 tpos;

        // Max number of tries before the system gives up (Maybe 10 is too high?)
        [SerializeField]
        private int _mtries = 10;
        // The number of tries the system is at now
        [SerializeField]
        private int _tries = 0;

        // the range within which the target will be assumed to be reached
        readonly float _epsilon = 0.1f;

        // Initializing the variables
        void Start()
        {

        }


        #region public methods
        //DO NOT CHANGE THE PUBLIC METHODS!!

        public float TwistMin { set => _twistMin = value; }
        public float TwistMax { set => _twistMax = value; }
        public float SwingMin { set => _swingMin = value; }
        public float SwingMax { set => _swingMax = value; }


        public void TestLogging(string objectName)
        {


            Debug.Log("hello, I am initializing my Octopus Controller in object " + objectName);


        }

        public void Init(Transform[] tentacleRoots, Transform[] randomTargets)
        {


            _tentacles = new MyTentacleController[tentacleRoots.Length];
            _cos = new float[53];
            _theta = new float[53];
            _sin = new float[53];

            _randomTargets = new Transform[randomTargets.Length];

            for (int i = 0; i < randomTargets.Length; i++)
            {
                _randomTargets[i] = randomTargets[i];
                // tpos[i] = new Vector3(_randomTargets[i].transform.position.x, _randomTargets[i].transform.position.y, _randomTargets[0].transform.position.z);

                //  Debug.Log(_randomTargets[i]); PRINTEA BIEN
            }

            // foreach (Transform t in tentacleRoots)
            for (int i = 0; i < tentacleRoots.Length; i++)
            {

                _tentacles[i] = new MyTentacleController();

                _tentacles[i].LoadTentacleJoints(tentacleRoots[i], TentacleMode.TENTACLE);


                //TODO: initialize any variables needed in ccd
            }
            //Debug.Log(_tentacles[1].Bones[50]);
            //Debug.Log(_tentacles[1].Bones[51]);
            //Debug.Log(_tentacles[1].Bones[52]);
            //Debug.Log(_tentacles[1].Bones[53]);


            //TODO: use the regions however you need to make sure each tentacle stays in its region

        }


        public void NotifyTarget(Transform target, Transform region)
        {

            _currentRegion = region;
            _target = target;


        }

        public void NotifyShoot()
        {
            //TODO. what happens here?
            Debug.Log("Shoot");
            Debug.Log(_currentRegion.name);
            if (_currentRegion.name == "region1")
            {
                region1b = true;
            }
            else if (_currentRegion.name == "region2")
            {

                region2b = true;
            }
            else if (_currentRegion.name == "region3")
            {
                region3b = true;
            }
            else if (_currentRegion.name == "region4")
            {
                region4b = true;
            }

        }


        public void UpdateTentacles()
        {
            if (!_done)
            {

                for (int t = 0; t < _tentacles.Length; t++)
                {

                    if (region1b == true && t == 0)
                    {
                        Debug.Log(_currentRegion.position);
                        actualiza(t, _target);

                    }
                    else if (region2b == true && t == 1)
                    {
                        Debug.Log("Se mueve2");
                        actualiza(t, _target);
                    }
                    else if (region3b == true && t == 2)
                    {
                        Debug.Log("Se mueve3");
                        actualiza(t, _target);
                    }
                    else if (region4b == true && t == 3)
                    {
                        Debug.Log("Se mueve4");
                        actualiza(t, _target);
                    }
                    else
                    {
                        Debug.Log("Se mueve random");
                        actualiza(t, _randomTargets[t]);
                    }
                }
            }

            for (int t = 0; t < _tentacles.Length; t++)
            {
                {
                    Vector3 diferenciaCOSAS;
                    if (region1b == true && t == 0)
                    {
                        diferenciaCOSAS = _target.transform.position - _tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position;
                        if (diferenciaCOSAS.magnitude <= _epsilon)
                        {
                            _done = true;
                        }
                        else
                        {
                            _done = false;
                        }
                        if (_tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position != _target.transform.position)
                        {
                            _tries = 0;
                        }

                    }
                    else if (region2b == true && t == 1)
                    {
                        diferenciaCOSAS = _target.transform.position - _tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position;
                        if (diferenciaCOSAS.magnitude <= _epsilon)
                        {
                            _done = true;
                        }
                        else
                        {
                            _done = false;
                        }
                        if (_tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position != _target.transform.position)
                        {
                            _tries = 0;
                        }

                    }
                    else if (region3b == true && t == 2)
                    {
                        diferenciaCOSAS = _target.transform.position - _tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position;
                        if (diferenciaCOSAS.magnitude <= _epsilon)
                        {
                            _done = true;
                        }
                        else
                        {
                            _done = false;
                        }
                        if (_tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position != _target.transform.position)
                        {
                            _tries = 0;
                        }

                    }
                   else if (region4b == true && t == 3)
                    {
                        diferenciaCOSAS = _target.transform.position - _tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position;
                        if (diferenciaCOSAS.magnitude <= _epsilon)
                        {
                            _done = true;
                        }
                        else
                        {
                            _done = false;
                        }
                        if (_tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position != _target.transform.position)
                        {
                            _tries = 0;
                        }

                    }
                    else
                    {
                     diferenciaCOSAS = _randomTargets[t].transform.position - _tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position;
                    if (diferenciaCOSAS.magnitude <= _epsilon)
                    {
                        _done = true;
                    }
                    else
                    {
                        _done = false;
                    }
                    if (_tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position != _randomTargets[t].transform.position)
                    {
                        _tries = 0;
                    }
                    }
                  
                }
            }

        }
        void actualiza(int numeroTentaculo, Transform targetPosT)
        {


            if (_tries <= _mtries)
            {

                for (int i = _tentacles[numeroTentaculo].Bones.Length - 2; i >= 0; i--)
                {

                    Vector3 r1 = (_tentacles[numeroTentaculo].Bones[_tentacles[numeroTentaculo].Bones.Length - 2].transform.position - _tentacles[numeroTentaculo].Bones[i].transform.position).normalized;

                    Vector3 r2 = (targetPosT.transform.position - _tentacles[numeroTentaculo].Bones[i].transform.position).normalized;
                    if (r1.magnitude * r2.magnitude <= 0.001f)
                    {
                    }
                    else
                    {
                        _cos[i] = Vector3.Dot(r1, r2);
                        Vector3 CrossR1R2 = Vector3.Cross(r1, r2);
                        _sin[i] = CrossR1R2.magnitude;

                    }

                    Vector3 axis = Vector3.Cross(r1, r2).normalized;
                    _theta[i] = Mathf.Acos(_cos[i]);
                    if (_sin[i] < 0)
                    {
                        _theta[i] = -_theta[i];

                    }
                    _theta[i] = (180 / Mathf.PI) * _theta[i]; //THETA EN GRADOS 
                    if (_theta[i] > 0.1)
                    {
                        _tentacles[numeroTentaculo].Bones[i].transform.Rotate(axis, _theta[i], Space.World);
                    }

                }
                _tries++;
            }






        }






        #endregion




        


    }
}

