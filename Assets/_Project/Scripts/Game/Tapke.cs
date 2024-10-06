using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnavinarML.General;
using UnityEngine;

public class Tapke : MonoBehaviour
{
    [SerializeField] private GameObject _body;
    [SerializeField] private float _relaxTime = 1f;
    [SerializeField] private float _electronRelaxTime = 5;
    [SerializeField] private float _pigtomChooseDistance = 20f;
    [SerializeField] private float _distanceFromPigtom = 3f;


    Fsm _fsm = new();
    Fsm.State _idle;
    Fsm.State _movingToPigtom;
    Fsm.State _catchingElectron;

    QuatumPig _target;
    TapkeController _controller;


    private void Start()
    {
        _target = Game.Instance.QuatumPig;
        _controller = GetComponent<TapkeController>();

        _idle = Idle;
        _movingToPigtom = MovingToPigtom;
        _catchingElectron = CatchingElectron;

        RelaxABit(ateElectron: false);

        _body.transform.DOLocalMoveY(0.1f, 1).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    void Update()
    {
        _fsm.OnUpdate();
    }


    float _idleTime = 0f;
    float _waitTime = 0f;
    void Idle(Fsm fsm, Fsm.Step step, Fsm.State state)
    {
        if (step == Fsm.Step.Enter)
        {
            _idleTime = 0;
        }
        else if (step == Fsm.Step.Update)
        {
            _idleTime += Time.deltaTime;
            if (_idleTime > _waitTime)
            {
                _fsm.TransitionTo(_movingToPigtom);
            }
        }
        else if (step == Fsm.Step.Exit)
        {
        }
    }

    public void RelaxABit(bool ateElectron)
    {
        _waitTime = ateElectron 
            ? _electronRelaxTime.WithVariation(1) 
            : _relaxTime.WithVariation(0.5f);
        _fsm.TransitionTo(_idle);
    }

    Pigtom _targetPigtom;
    Pigtom _intermediatePigtom;
    Vector3 _targetPosition;
    bool _goingToCorrectPigtom = false;
    Tween _pigtomMoveTween;
    Vector3 debugControlPoint;
    Vector3 debugStartPoint;
    Vector3 debugHitPoint;

    
    void MovingToPigtom(Fsm fsm, Fsm.Step step, Fsm.State state)
    {
        if (step == Fsm.Step.Enter)
        {
            PlanMovementToPigtom();
        }
        else if (step == Fsm.Step.Update)
        {
            if (!_controller.IsMoving)
            {
                if (_goingToCorrectPigtom)
                    _fsm.TransitionTo(_catchingElectron);
                else
                {
                    RelaxABit(ateElectron: false);
                }
            }

        }
        else if (step == Fsm.Step.Exit)
        {
            _pigtomMoveTween?.Kill();
            _targetPigtom = null;
        }
    }

    private void PlanMovementToPigtom()
    {
        _targetPigtom = ChooseTargetPigtom();
        Debug.Log($"Chosen pigtom: {_targetPigtom}");

        Ray ray = new(transform.position, _targetPigtom.transform.position - transform.position);
        if (Physics.SphereCast(ray, 1.5f, out RaycastHit hit, 10000, Globals.PigtomMask))
        {
            var pigtom = hit.collider.GetComponent<Pigtom>();
            var realHit = hit.point + hit.normal * 1.5f;
            if (pigtom == _targetPigtom)
            {
                _targetPosition = realHit - ray.direction * _distanceFromPigtom;
                _goingToCorrectPigtom = true;
                _controller.SetStraightMovementTarget(_targetPosition);
            }
            else
            {
                _goingToCorrectPigtom = false;
                _intermediatePigtom = pigtom;
                debugHitPoint = realHit;

                Vector3 A = transform.position;
                Vector3 B = _targetPigtom.transform.position;
                Vector3 C = _intermediatePigtom.transform.position;

                Vector3 AB = B - A;  // from tapke to final pigtom
                Vector3 AC = C - A;  // from tapke to hitted pigtom
                Vector3 AO = Vector3.Project(AC, AB);  // ???
                Vector3 O = A + AO;  // closest point on AB to hitted pigtom
                Vector3 COdir = (O - C).normalized;  // direction from hitted pigtom to closest point on AB
                Vector3 S = C + COdir * (_intermediatePigtom.NucleusRadius + _distanceFromPigtom);  // target position

                Vector3 AS = S - A;  // from tapke to target point
                Vector3 AM = Vector3.Project(AC, AS);
                Vector3 M = A + AM;  // closest point on AS to sphere
                Vector3 CM = M - C;
                Vector3 D = C + CM.normalized * _intermediatePigtom.NucleusRadius;  // point on sphere closest to M
                Vector3 DM = M - D;
                Vector3 control = D + DM.normalized * (_distanceFromPigtom + 1);

                _controller.SetBezierMovementTarget(S, control);

                debugStartPoint = transform.position;
                debugControlPoint = control;
            }

            _pigtomMoveTween.Play();
        }
        else
        {
            RelaxABit(ateElectron: false);
        }
    }


    private Pigtom ChooseTargetPigtom()
    {
        List<Pigtom> pigtoms = new();

        var allPigtoms = ServiceLocator.Get<PigtomsManager>().CoolPigtoms;
        if (allPigtoms.Count() == 0)
        {
            return null;
        }

        float minDistance = allPigtoms.Min(p => Vector3.Distance(p.transform.position, _target.transform.position));

        foreach (var pigtom in allPigtoms)
            if (Vector3.Distance(pigtom.transform.position, _target.transform.position) < minDistance + _pigtomChooseDistance)
                pigtoms.Add(pigtom);

        foreach (var pigtom in pigtoms)
            Debug.Log(pigtom.name);
        Debug.Log($"Pigtoms found: {pigtoms.Count}");

        // TODO add probabilistic selection(no)
        // return null;
        return pigtoms.RandomElement();
    }


    private void CatchingElectron(Fsm fsm, Fsm.Step step, Fsm.State state)
    {
        if (step == Fsm.Step.Enter)
        {
            RelaxABit(ateElectron: false);
        }
        else if (step == Fsm.Step.Update)
        {
            StartCoroutine(CatchElectron());
        }
        else if (step == Fsm.Step.Exit)
        {
            Debug.Log("CatchingElectron Exit");
        }
    }

    private IEnumerator CatchElectron()
    {
        yield return new WaitForSeconds(_relaxTime);
        // throw new NotImplementedException();
    }

    private void OnDrawGizmos()
    {
        if (_targetPigtom != null)
        {
            Gizmos.color = Color.red;
            // Gizmos.DrawWireSphere(transform.position, 1f);
            if (_goingToCorrectPigtom)
                Gizmos.DrawLine(transform.position, _targetPosition);
            else
            {
                Gizmos.color = Color.magenta;
                Vector3 prev = debugStartPoint;
                for (int i = 0; i < 100; i++)
                {
                    float t = i / 100f;
                    Vector3 next = MathExtensions.Bezier(debugStartPoint, debugControlPoint, _targetPosition, t);
                    Gizmos.DrawLine(prev, next);
                    prev = next;
                }

                Vector3 A = debugStartPoint;
                Vector3 B = _targetPigtom.transform.position;
                Vector3 C = _intermediatePigtom.transform.position;
                Vector3 L = debugHitPoint;

                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(A, 0.5f);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(B, 0.5f);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(C, 0.5f);
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(L, 0.5f);

                Vector3 AB = B - A;
                Vector3 AC = C - A;
                

                Vector3 AO = Vector3.Project(AC, AB);
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(AO, 0.5f);

                Vector3 O = A + AO;


                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(O, 0.5f);

                // Vector3 O = Vector3.Project(AC, AB);
                Vector3 COdir = (O - C).normalized;
                Vector3 S = C + COdir * (_intermediatePigtom.NucleusRadius + _distanceFromPigtom);


                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(S, 0.5f);

                // Gizmos.color = Color.green;
                // Gizmos.DrawLine(C, S);

                // Gizmos.color = Color.cyan;
                // Gizmos.DrawLine(A, C);
                // Gizmos.DrawLine(A, B);


                // Vector3 direction = _targetPigtom.transform.position - debugStartPoint;
                // Vector3 intermDir = _intermediatePigtom.transform.position - debugStartPoint;
                // // project hitted pigtom onto direction from tapke to target pigtom
                // // find point on hitted pigtom which will make closest distance to target pigtom
                // Vector3 projection = Vector3.Project(intermDir, direction);
                // Vector3 dir = (projection - _intermediatePigtom.transform.position).normalized;

                // Gizmos.color = Color.yellow;
                // Gizmos.DrawWireSphere(projection, 0.5f);
                // // Debug.Log(projection);
                // Gizmos.color = Color.green;
                // Gizmos.DrawLine(_intermediatePigtom.transform.position, projection);
                // Gizmos.DrawRay(_intermediatePigtom.transform.position, dir * (_intermediatePigtom.NucleusRadius+2));

                // Gizmos.color = Color.cyan;
                // Gizmos.DrawLine(debugStartPoint, debugHitPoint);


            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_targetPigtom.transform.position, _pigtomChooseDistance);

            // Gizmos.color = Color.black;
            // Gizmos.DrawLine(debugStartPoint, _targetPigtom.transform.position);
        }
    }
}
