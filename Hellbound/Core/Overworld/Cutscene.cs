using Casull.Core.DialogueSystem;
using Casull.Core.ECS;
using Casull.Core.ECS.Components;
using Casull.Core.UI;
using Casull.Core.UI.Elements;
using Casull.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.Overworld
{
    public class Cutscene
    {
        private bool finished;
        public bool Finished => finished;

        public Vector2 cameraPosition;

        public Entity following;
        public float followingSpeed = 0.1f;

        public List<Entity> actors = [];

        readonly List<ICutsceneAction> actions = [];

        public void Update()
        {
            if (actions.Count <= 0) {
                finished = true;
                UIManager.overworldUI.SetBlackBars(false);
                actions.Clear();
                return;
            }

            if(following != null) {
                Camera cam = Main.instance.ActiveWorld.GetCamera();

                var value = (following.GetComponent<Transform>().position - cam.centre) * followingSpeed;

                cam.centre += new Vector2(value.X, value.Y);
                cam.Clamp(Vector2.Zero, new Vector2(30) * 32);
            }

            while(actions.Count > 0 && actions[0].Update(actors)) {
                actions.RemoveAt(0);
            }
        }

        public void Add(ICutsceneAction action)
        {
            actions.Add(action); 
        }

        public void BeginTransition(Transition transition)
        {
            Main.instance.transitions.Add(transition);
        }

        public void SetFollowing(Entity e, float speed)
        {
            Add(new SetFollowing(this, e, speed));
        }

        public void InternalSetFollowing(Entity e, float speed)
        {
            following = e;
            followingSpeed = speed;
        }
    }

    public class SpawnEntityFromPrefab : ICutsceneAction
    {
        public SpawnEntityFromPrefab(Context context, string prefab, Vector2 position, out Entity entity) 
        {
            entity = context.CopyFrom(EntitySaver.Load("\\Content\\Prefabs\\", prefab));
            entity.GetComponent<Transform>().position = position;
        }

        public bool Update(List<Entity> actors)
        {
            return true;
        }
    }

    public class Timer(int timeleft) : ICutsceneAction
    {
        public int timeleft = timeleft;

        public bool Update(List<Entity> actors)
        {
            return --timeleft <= 0;
        }
    }

    public class FireAction(Action action) : ICutsceneAction
    {
        Action action = action;

        public bool Update(List<Entity> actors)
        {
            action();
            action = null;
            return true;
        }
    }

    public class FireActionFor(Action action, int timer) : ICutsceneAction
    {
        Action action = action;
        int timer = timer;

        public bool Update(List<Entity> actors)
        {
            if (--timer > 0) {
                action();
                return false;
            }
            action = null;
            return true;
        }
    }

    public class StartDialogue(Dialogue dialogue) : ICutsceneAction
    {
        bool addedDialogue;

        Dialogue dialogue = dialogue;

        public bool Update(List<Entity> actors)
        {
            if(!addedDialogue) {
                addedDialogue = true;
                UIManager.dialogueUI.dialogues.Add(dialogue);
            }

            if(!UIManager.dialogueUI.dialogues.Contains(dialogue)) {
                dialogue = null;
                return true;
            }

            return false;
        }
    }

    public class SetPosition(Entity actor, Vector2 position) : ICutsceneAction
    {
        public Entity actor = actor;
        public Vector2 targetPosition = position;

        public bool Update(List<Entity> actors)
        {
            actor.GetComponent<Transform>().position = targetPosition;
            actor = null;
            return true;
        }
    }

    public class SetComponent(Entity actor, IComponent component) : ICutsceneAction
    {
        public Entity actor = actor;
        public bool Update(List<Entity> actors)
        {
            actor.AddComponent(component);
            actor = null;
            return true;
        }
    }

    public class SetFollowing(Cutscene scene, Entity actor, float speed) : ICutsceneAction
    {
        public Entity actor = actor;

        public bool Update(List<Entity> actors)
        {
            scene.InternalSetFollowing(actor, speed);
            actor = null;
            return true;
        }
    }

    public class WaitForPosition(Entity actor, Vector2 position, float tolerance = 4f) : ICutsceneAction
    {
        public Entity actor = actor;
        public float tolerance = tolerance;
        public Vector2 targetPosition = position;

        public bool Update(List<Entity> actors)
        {
            var transform = actor.GetComponent<Transform>();

            if (Vector2.Distance(transform.position, targetPosition) <= tolerance) {
                transform.position = targetPosition;
                actor = null;
                return true;
            }
            return false;
        }
    }

    public interface ICutsceneAction
    {
        bool Update(List<Entity> actors);
    }
}
