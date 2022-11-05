using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkAtAPizzaPlace
{
	/// <summary>
    /// This is a base class for all clickable objects.
    /// </summary>
	public partial class ClickableObject : AnimEntity
	{
        public override void Spawn()
        {
            // Set clickable tag.
            Tags.Add("clickable");
            base.Spawn();
        }
        /// <summary>
        /// This method is called when the player clicks on the object.
        /// <param name="hitboxIndex">The index of the hitbox clicked.</param>
        /// </summary>
        [ClientRpc]
        public virtual void Click(int hitboxIndex)
        {
            // Do nothing.
        }
        /// <summary>
        /// This method is called whenever the player hovers over the object.
        /// Return true if the player can click on the object.
        /// <param name="hitboxIndex">The index of the hitbox selected.</param>
        /// </summary>
        public virtual bool HitBoxHandler(int hitboxIndex)
		{
            // Clickable regions are defined in the model file.
            return false;
		}
    }
}