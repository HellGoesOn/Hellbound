using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treeline.Core.Graphics
{
    public static class ParticleManager
    {
        public static Particle[] particles = new Particle[10000];
        public static Particle[] particlesAdditive = new Particle[10000];

        public static int count;
        public static int countAdditive;

        public static void Initialize()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new Particle(Vector3.Zero, Vector3.Zero);
                particles[i].timeLeft = 0;
                particlesAdditive[i] = new Particle(Vector3.Zero, Vector3.Zero);
                particlesAdditive[i].timeLeft = 0;
            }
        }


        public static Particle NewParticle(Vector3 position, Vector3 velocity, int timeLeft = 120, float weight = 0f, bool castShadow = false)
        {
            if (++count >= particles.Length)
            {
                count = particles.Length - 1;

                var particle2 = particles[0];
                particle2.SetDefaults();
                particle2.position = position;
                particle2.velocity = velocity;
                particle2.castShadow = castShadow;
                particle2.weight = weight;
                particle2.timeLeft = timeLeft;
                particle2.diesToGravity = false;
                return particle2;
            }

            var particle = particles[count];
            particle.SetDefaults();
            particle.position = position;
            particle.velocity = velocity;
            particle.castShadow = castShadow;
            particle.weight = weight;
            particle.timeLeft = timeLeft;
            particle.diesToGravity = false;

            return particles[count];
        }

        public static Particle NewParticleAdditive(Vector3 position, Vector3 velocity, int timeLeft = 120, float weight = 0f, bool castShadow = false)
        {
            if (++countAdditive >= particlesAdditive.Length)
            {
                countAdditive = particlesAdditive.Length - 1; 
                var particle2 = particlesAdditive[countAdditive];
                particle2.SetDefaults();
                particle2.position = position;
                particle2.velocity = velocity;
                particle2.castShadow = castShadow;
                particle2.weight = weight;
                particle2.timeLeft = timeLeft;
                particle2.diesToGravity = false;
                return particle2;
            }

            var particle = particlesAdditive[countAdditive];
            particle.SetDefaults();
            particle.position = position;
            particle.velocity = velocity;
            particle.castShadow = castShadow;
            particle.weight = weight;
            particle.timeLeft = timeLeft;
            particle.diesToGravity = false;

            return particlesAdditive[countAdditive];
        }

        public static void Update()
        {
            for (int i = 0; i < count; i++)
            {
                Particle particle = particles[i];

                particle.Update();

                if (particle.timeLeft <= 0 || (particle.diesToGravity && particle.position.Z == 0))
                {
                    (particles[count], particles[i]) = (particles[i], particles[count]);
                    count--;
                }
            }

            for (int i = 0; i < countAdditive; i++)
            {
                Particle particle = particlesAdditive[i];

                particle.Update();

                if (particle.timeLeft <= 0 || (particle.diesToGravity && particle.position.Z == 0))
                {
                    (particlesAdditive[countAdditive], particlesAdditive[i]) = (particlesAdditive[i], particlesAdditive[countAdditive]);
                    countAdditive--;
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < count; i++)
            {
                Particle particle = particles[i];

                particle.Draw(spriteBatch);
            }

            spriteBatch.End();
            Renderer.StartSpriteBatch(spriteBatch, blend: BlendState.Additive);
            for (int i = 0; i < countAdditive; i++)
            {
                Particle particle = particlesAdditive[i];

                particle.Draw(spriteBatch);
            }
            spriteBatch.End();
            Renderer.StartSpriteBatch(spriteBatch);
        }
    }
}
