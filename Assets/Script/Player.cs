using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    private Input_Player _input = null;

    private Vector2 _moveVector = Vector2.zero;
    [SerializeField] private float _playerSpeed = 6.0f;
    [SerializeField] private TextMeshProUGUI _numberHead;
    [SerializeField] private bool win = false;

    public int currentKey = 0;

    private void Awake()
    {
        _input = new Input_Player();
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.PlayerMovement.Move.performed += OnMovementPerformed;
        _input.PlayerMovement.Move.canceled += OnMovementCanceled;

    }

    private void OnDisable()
    {
        _input.Disable();
        _input.PlayerMovement.Move.performed -= OnMovementPerformed;
        _input.PlayerMovement.Move.canceled -= OnMovementCanceled;
    }

    private void Update()
    {
        if(win)
        {
            _numberHead.text = "";
        }
        else
        {
            _numberHead.text = currentKey.ToString();
        }

        Vector3 target = new Vector3(transform.position.x + _moveVector.x, transform.position.y + _moveVector.y, 0);
        transform.position = Vector3.Lerp(transform.position, target, _playerSpeed * Time.deltaTime);
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        _moveVector = value.ReadValue<Vector2>();
    }
    private void OnMovementCanceled(InputAction.CallbackContext value)
    {
        _moveVector = value.ReadValue<Vector2>();
    }

    public void Die()
    {
        transform.position = Vector3.zero;
        GameManager.Instance.Lose();
    }
}
