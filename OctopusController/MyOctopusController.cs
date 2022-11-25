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

        Transform[] _randomTargets;


        float _twistMin, _twistMax;
        float _swingMin, _swingMax;
        float _timer = 0f;
        float TIMER_ENDED = 3f;

        bool _region1b = false;
        bool _region2b = false;
        bool _region3b = false;
        bool _region4b = false;

        [SerializeField]
        float[] _theta, _sin, _cos;

        bool _done = false;

        [SerializeField]
        private int _mtries = 10;

        [SerializeField]
        private int _tries = 0;


        readonly float _epsilon = 0.1f;

        void Start()
        {

        }


        #region public methods

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
            }


            for (int i = 0; i < tentacleRoots.Length; i++)
            {
                _tentacles[i] = new MyTentacleController();
                _tentacles[i].LoadTentacleJoints(tentacleRoots[i], TentacleMode.TENTACLE);
            }
        }


        public void NotifyTarget(Transform target, Transform region)
        {

            _currentRegion = region;
            _target = target;


        }

        public void NotifyShoot()
        {

            Debug.Log("Shoot");

            if (_currentRegion.name == "region1")
            {
                _region1b = true;
            }
            else if (_currentRegion.name == "region2")
            {

                _region2b = true;
            }
            else if (_currentRegion.name == "region3")
            {
                _region3b = true;
            }
            else if (_currentRegion.name == "region4")
            {
                _region4b = true;
            }
        }


        public void UpdateTentacles()
        {
            if (!_done)
            {

                for (int t = 0; t < _tentacles.Length; t++)
                {

                    if (_region1b == true && t == 0)
                    {
                        ApplyCCD(t, _target);
                    }
                    else if (_region2b == true && t == 1)
                    {
                        ApplyCCD(t, _target);
                    }
                    else if (_region3b == true && t == 2)
                    {
                        ApplyCCD(t, _target);
                    }
                    else if (_region4b == true && t == 3)
                    {
                        ApplyCCD(t, _target);
                    }
                    else
                    {
                        ApplyCCD(t, _randomTargets[t]);
                    }
                }

                if (_region1b || _region2b || _region3b || _region3b || _region4b) TimerReset();
            }

            for (int t = 0; t < _tentacles.Length; t++)
            {
                if (_region1b == true && t == 0)
                {
                    ResetTentacle(t, _target);

                }
                else if (_region2b == true && t == 1)
                {
                    ResetTentacle(t, _target);
                }
                else if (_region3b == true && t == 2)
                {
                    ResetTentacle(t, _target);

                }
                else if (_region4b == true && t == 3)
                {
                    ResetTentacle(t, _target);

                }
                else
                {
                    ResetTentacle(t, _randomTargets[t]);
                }
            }
        }

        void TimerReset()
        {
            if (_timer >= TIMER_ENDED)
            {
                _region1b = false;
                _region2b = false;
                _region3b = false;
                _region4b = false;
                _timer = 0f;
            }
            else
            {
                _timer += Time.deltaTime;
            }



        }

        void ResetTentacle(int t, Transform target)
        {
            Vector3 distance;

            distance = target.transform.position - _tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position;
            if (distance.magnitude <= _epsilon)
            {
                _done = true;
            }
            else
            {
                _done = false;
            }
            if (_tentacles[t].Bones[_tentacles[t].Bones.Length - 2].transform.position != target.transform.position)
            {
                _tries = 0;
            }
        }

        void ApplyCCD(int numeroTentaculo, Transform targetPosT)
        {
            if (_tries <= _mtries)
            {
                for (int i = _tentacles[numeroTentaculo].Bones.Length - 2; i >= 0; i--)
                {
                    Vector3 r1 = (_tentacles[numeroTentaculo].Bones[_tentacles[numeroTentaculo].Bones.Length - 2].transform.position - _tentacles[numeroTentaculo].Bones[i].transform.position).normalized;
                    Vector3 r2 = (targetPosT.transform.position - _tentacles[numeroTentaculo].Bones[i].transform.position).normalized;

                    if (r1.magnitude * r2.magnitude > 0.001f)
                    {
                        _cos[i] = Vector3.Dot(r1, r2);
                        Vector3 crossR1R2 = Vector3.Cross(r1, r2);
                        _sin[i] = crossR1R2.magnitude;
                    }

                    Vector3 axis = Vector3.Cross(r1, r2).normalized;
                    _theta[i] = Mathf.Acos(_cos[i]);
                    if (_sin[i] < 0)
                    {
                        _theta[i] = -_theta[i];

                    }
                    _theta[i] = (180 / Mathf.PI) * _theta[i];
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

