using System;
using Unity2dPlatformerCookbook.Scripts.Controls;
using Unity2dPlatformerCookbook.Scripts.Utils;
using UnityEngine;

namespace Unity2dPlatformerCookbook.Scripts.Entities.States
{
    public class EntityState : IEntityState
    {
        protected Entity _entity;
        protected EntityStateMachine _stateMachine;

        protected static bool _grounded;
        protected static Direction _facing;
        
        protected static float _moveVelocity;
        protected static float _jumpVelocity;
        protected static float _airJumpCount;
        
        public EntityState(Entity entity, EntityStateMachine stateMachine)
        {
            _entity = entity;
            _stateMachine = stateMachine;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // event delegates

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // internal methods

        protected bool IsGrounded()
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                _entity.BoxCollider2D().bounds.center, 
                _entity.BoxCollider2D().bounds.size, 0, Vector2.down, 
                _entity.JumpConfiguration().GroundDistance, 
                _entity.JumpConfiguration().GroundLayer
            );

            // draw a debug line at the bottom of the object representing what the box raycast would be hitting
            Debug.DrawLine(
                new Vector3(_entity.Rigidbody2D().position.x - _entity.BoxCollider2D().bounds.extents.x, _entity.Rigidbody2D().position.y - _entity.JumpConfiguration().GroundDistance),
                new Vector3(_entity.Rigidbody2D().position.x + _entity.BoxCollider2D().bounds.extents.x, _entity.Rigidbody2D().position.y - _entity.JumpConfiguration().GroundDistance),
                hit.collider is null ? Color.green : Color.red
            );

            return hit.collider is not null;
        }

        protected void ApplyMovementInstantly()
        {
            _entity.Rigidbody2D().velocity = new Vector2(_moveVelocity, _entity.Rigidbody2D().velocity.y);
        }

        protected void ApplyMovementWithAcceleration()
        {
            _entity.Rigidbody2D().velocity = new Vector2(
                Mathf.Clamp(
                    _entity.Rigidbody2D().velocity.x + (_moveVelocity > 0f ? 1f : -1f) * _entity.MoveConfiguration().AccelerationSpeed * Time.deltaTime,
                    -_entity.MoveConfiguration().TopSpeed,
                    _entity.MoveConfiguration().TopSpeed),
                _entity.Rigidbody2D().velocity.y);
        }

        protected void ApplyStopInstantly()
        {
            _entity.Rigidbody2D().velocity = new Vector2(0f, _entity.Rigidbody2D().velocity.y);
        }

        protected void ApplyStopWithDeceleration()
        {
            if (_entity.Rigidbody2D().velocity.x < 0f)
            {
                _entity.Rigidbody2D().velocity = new Vector2(
                    Mathf.Clamp(
                        _entity.Rigidbody2D().velocity.x + _entity.MoveConfiguration().DecelerationSpeed * Time.deltaTime,
                        _entity.Rigidbody2D().velocity.x,
                        _moveVelocity),
                    _entity.Rigidbody2D().velocity.y);
            }

            if (_entity.Rigidbody2D().velocity.x > 0f)
            {
                _entity.Rigidbody2D().velocity = new Vector2(
                    Mathf.Clamp(
                        _entity.Rigidbody2D().velocity.x - _entity.MoveConfiguration().DecelerationSpeed * Time.deltaTime,
                        _moveVelocity,
                        _entity.Rigidbody2D().velocity.x),
                    _entity.Rigidbody2D().velocity.y);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // interface methods

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void Update()
        {
            _grounded = IsGrounded();
        }

        public virtual void FixedUpdate()
        {
        }
    }
}