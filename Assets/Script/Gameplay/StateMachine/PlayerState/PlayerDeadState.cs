public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.trigger.enabled = false;
        stateMachine.Ragdoll.ToggleRagdoll(true);
        stateMachine.Weapon.gameObject.SetActive(false);

        //noted : perlu di ganti lagi

        LoadScene.instance.loadScene(stateMachine.playerSO.lastScene);
    }

    public override void Tick(float deltaTime) { }

    public override void Exit() { }
}
