using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    private readonly int JumpHash = Animator.StringToHash("Jump");

    private Vector3 momentum;

    private const float CrossFadeDuration = 0.1f;

    public PlayerJumpingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {

        momentum = stateMachine.Controller.velocity;
        momentum.y = 0f;

        stateMachine.Animator.CrossFadeInFixedTime(JumpHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        Move(momentum, deltaTime);

        if (stateMachine.Controller.velocity.y <= 0)
        {
            stateMachine.SwitchState(new PlayerFallingState(stateMachine));
            return;
        }

        FaceTarget();
    }

    public override void Exit()
    {
    }

    private void HandleLedgeDetect(Vector3 ledgeForward, Vector3 closestPoint)
    {
    }
}
