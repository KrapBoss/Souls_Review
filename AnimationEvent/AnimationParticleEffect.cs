using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationParticleEffect : MonoBehaviour
{
    [Serializable]
    public class Effect
    {
        public string name;
        public ParticleSystem particle;
    }

    public List<Effect> effets;

    public void StartEffect(string name)
    {
        Effect effect = effets.Where((x) => x.name.Equals(name))?.FirstOrDefault();

        if(effect != null)
        {
            effect.particle.Play();
        }
    }
}
