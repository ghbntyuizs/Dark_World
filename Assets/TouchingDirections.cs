/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D castFilter;
    public float groundDistance = 0.05f;
    CapsuleCollider2D touchingCol;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];

    private bool _isGrounded;

    Animator animator;

    public bool isGrounded { get { return _isGrounded}; private set
        {
            _isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, value);
        }; }

    private Awake()
    {
        touchingCol = GetComponent<CapsuleCollider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGround = touchingCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
    }
}
*/