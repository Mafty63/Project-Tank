using System;
using UnityEngine;

[Serializable]
public class Skill
{
    [field: SerializeField] public string Name { get; private set; } 
    [field: SerializeField] public GameObject SkillParticle { get; private set; }
    [field: SerializeField] public float Duration { get; private set; }
    [field: SerializeField] public float SpawnParticleTime { get; private set; }
    [field: SerializeField] public string AnimationName { get; private set; }
    [field: SerializeField] public int DamagePercent { get; private set; }
    [field: SerializeField] public float Knockback { get; private set; }

}