using Casull.Core.ECS.Components;
using Casull.Core.Overworld;
using Casull.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.ECS
{
    public class InteractionSystem : IExecute
    {
        readonly Group<Entity> _group;

        public InteractionSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(PlayerMarker), typeof(CollisionBox)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            bool interactableNearby = false;
            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];

                var myBox = entity.GetComponent<CollisionBox>();

                for (int j = 0; j < myBox.CollidedWith.Count; j++) {

                    var id = myBox.CollidedWith[j];

                    var interactable = context.GetById(id);

                    if (interactable == null || !interactable.HasComponent<Interactable>())
                        continue;

                    interactableNearby = true;


                    if (Input.PressedKey(Keys.E)) {
                        var commandToFire = interactable.GetComponent<Interactable>().onInteract;

                        var trigger = World.triggers.FirstOrDefault(x => x.id == commandToFire);

                        if (trigger != null) {
                            trigger.Activate(Main.instance.ActiveWorld);
                            break;
                        }
                    }
                }
            }
            if (interactableNearby)
                UIManager.overworldUI.interactTextOpacityTarget = 1.0f;
            else
                UIManager.overworldUI.interactTextOpacityTarget = 0.0f;
        }
    }
}
