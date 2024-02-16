using com.perceptlab.armultiplayer;
using MixedReality.Toolkit.SpatialManipulation;
using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static Normal.Realtime.Realtime;

[RequireComponent(typeof(RealtimeView),typeof(RealtimeTransform))]
public class SharedInteractableHandler : MonoBehaviour
{
    private ObjectManipulator _objectManipulator;
    private RealtimeView _realtimeView;
    private RealtimeTransform _realtimeTransform;
    private Rigidbody _rigidbody;
    private bool _isGrabbed = false;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _objectManipulator = GetComponent<ObjectManipulator>();
        _realtimeView = GetComponent<RealtimeView>();
        _realtimeTransform = GetComponent<RealtimeTransform>();
        _realtimeView.ownerIDSelfDidChange += (RealtimeView realtimeView, int newId) =>
                        RLogger.Log("RealtimeView " + _realtimeView.name + "'s ownership changed to(or from) " + newId + " and is now " + _realtimeView.ownerIDSelf);
        _realtimeView.preventOwnershipTakeover = false;
        if (_objectManipulator != null)
        {
            _objectManipulator.selectEntered.AddListener((SelectEnterEventArgs args) => SetGrabbed(true));
            _objectManipulator.selectEntered.AddListener((SelectEnterEventArgs args) => _realtimeView.RequestOwnership());
            _objectManipulator.selectEntered.AddListener((SelectEnterEventArgs args) => _realtimeTransform.RequestOwnership());//first request view then transform. transform request gets rejected if view is not owned locally
            _objectManipulator.selectEntered.AddListener((SelectEnterEventArgs args) => RLogger.Log(_realtimeTransform.name + " select entered called"));

            _objectManipulator.selectExited.AddListener((SelectExitEventArgs args) => SetGrabbed(false));
            _objectManipulator.selectExited.AddListener((SelectExitEventArgs args) => RLogger.Log(_realtimeTransform.name + " select exited called"));
        }
    }

    void SetGrabbed(bool isGrabbed)
    {
        _isGrabbed = isGrabbed;
    }

    void Update()
    {
        if (_rigidbody != null && !_isGrabbed) 
        {
            if (!_rigidbody.isKinematic && _rigidbody.useGravity == false)
            {
                RLogger.Log("Shared object with Rigidbody and no gravity. Setting Gravity by hand");
                _rigidbody.useGravity = true;
            }
        }
    }
}
