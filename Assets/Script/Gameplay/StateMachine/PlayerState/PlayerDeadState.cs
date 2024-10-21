public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.trigger.enabled = false;

        //noted : perlu di ganti lagi

    }

    public override void Tick(float deltaTime) { }

    public override void Exit() { }
}
