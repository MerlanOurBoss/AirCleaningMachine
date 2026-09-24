using System.Collections.Generic;
using UnityEngine;

namespace Math_Module
{
    public static class ParticleFxUtility
    {
        public static void SetOverLifetimeAlpha(ParticleSystem particleSystem, float alpha)
        {
            if (particleSystem == null) return;

            var colorModule = particleSystem.colorOverLifetime;
            var currentGradient = colorModule.color.gradient;

            var newGradient = new Gradient();
            newGradient.SetKeys(
                currentGradient.colorKeys,
                new[] { new GradientAlphaKey(alpha, 0f), new GradientAlphaKey(alpha, 1f) }
            );

            colorModule.color = new ParticleSystem.MinMaxGradient(newGradient);
        }

        public static void SetOverLifetimeAlpha(IEnumerable<ParticleSystem> particleSystems, float alpha)
        {
            if (particleSystems == null) return;
            foreach (var ps in particleSystems)
                SetOverLifetimeAlpha(ps, alpha);
        }
    }
}