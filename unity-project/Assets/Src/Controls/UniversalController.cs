using Assets.Src.Controls;
using UnityEngine;

public class UniversalController : MonoBehaviour
{
    private IControlScheme _currentlyActiveControlScheme;

    private Rigidbody _rigidBody;

    public float MoveSpeed = 5f;
    public float MouseSensitivity = 100f;

    void Start()
    {
        _currentlyActiveControlScheme = ControlSchemeBuilder.BuildDefaultControlSheme(MoveSpeed, MouseSensitivity);

        _currentlyActiveControlScheme.StartupActions()();

        _rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var movementCommand = _currentlyActiveControlScheme.MovePlayer(gameObject, _rigidBody);
        var rotateCommand = _currentlyActiveControlScheme.RotatePlayer(gameObject, Camera.main.transform);

        movementCommand.ExecuteMovement();
        rotateCommand.ExecuteRotate();
    }
}

