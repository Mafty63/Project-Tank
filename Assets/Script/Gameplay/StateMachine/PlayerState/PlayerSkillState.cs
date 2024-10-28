using UnityEngine;

public class PlayerSkillState : PlayerBaseState
{
    private Skill skill;
    private float duration;

    public PlayerSkillState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        skill = stateMachine.Skill1;
    }

    public override void Enter()
    {
    }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);

        FaceTarget();

        if (duration < .1)
        {
            if (stateMachine.Targeter.CurrentTarget != null)
            {
            }
            else
            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            }

        }
        else if (duration < (skill.Duration - stateMachine.Skill1.SpawnParticleTime))
        {
        }

        duration -= Time.deltaTime;
    }

    public class WeaponDamage
    {
        public void SetAttack(int damage, float knockback)
        {
            // Logika untuk menetapkan serangan
        }

        public void SetSkill(int effect, float duration)
        {
            // Logika untuk menetapkan keterampilan
            // Misalnya, menerapkan efek keterampilan ke target
            ApplySkillEffect(effect);

            // Menetapkan durasi keterampilan
            SetSkillDuration(duration);
        }

        private void ApplySkillEffect(int effect)
        {
            // Implementasi untuk menerapkan efek keterampilan ke target
        }

        private void SetSkillDuration(float duration)
        {
            // Implementasi untuk menetapkan durasi keterampilan
        }
    }

    public override void Exit()
    {

    }


}