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
        stateMachine.Skill.SetAttack(stateMachine.Statistic.statisticData.level, stateMachine.Skill1.DamagePercent, stateMachine.Statistic.statisticData.accuracy, stateMachine.Skill1.Knockback);

        stateMachine.trigger.enabled = false;
        StartSkill(skill.Duration);
    }

    public override void Tick(float deltaTime) 
    {
        Move(deltaTime);

        FaceTarget();

        if (duration < .1)
        {
            if (stateMachine.Targeter.CurrentTarget != null)
            {
                stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
            }
            else
            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            }

            stateMachine.Khodam.SetActive(false);
            stateMachine.Skill1.SkillParticle.SetActive(false);
        }
        else if (duration < (skill.Duration - stateMachine.Skill1.SpawnParticleTime))
        {
            stateMachine.Skill1.SkillParticle.SetActive(true);
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

    private void StartSkill(float duration)
    {
        this.duration = duration;
        stateMachine.Khodam.SetActive(true);
    }
}