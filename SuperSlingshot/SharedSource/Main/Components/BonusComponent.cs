﻿using System.Runtime.Serialization;
using SuperSlingshot.Managers;
using SuperSlingshot.Scenes;
using WaveEngine.Common.Physics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Physics2D;

namespace SuperSlingshot.Components
{
    [DataContract]
    public class BonusComponent : Component
    {
        private GameScene scene;

        [RequiredComponent(false)]
        private Collider2D collider = null;

        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.collider.BeginCollision += this.OnBeginCollision;
            this.collider.EndCollision += this.OnEndCollision;

            var scene = this.Owner.Scene as GameScene;

            if (scene != null)
            {
                this.scene = scene;
            }
        }

        private void OnBeginCollision(ICollisionInfo2D contact)
        {
            // Against gem
            if ((contact.ColliderB.CollisionCategories 
                & ColliderCategory2D.Cat5) == ColliderCategory2D.Cat5)
            {
                return;
            }
            else
            {
                contact.IsEnabled = false;

                this.scene.EntityManager.Remove(this.Owner);

                this.scene.Score.Gems += 1;
            }
        }

        private void OnEndCollision(ICollisionInfo2D contact)
        {
        }

        protected override void DeleteDependencies()
        {
            base.DeleteDependencies();

            if (this.collider != null)
            {
                this.collider.BeginCollision -= this.OnBeginCollision;
                this.collider.EndCollision -= this.OnEndCollision;
            }
        }
    }
}
