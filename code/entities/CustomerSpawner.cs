using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkAtAPizzaPlace
{
	/// <summary>
    /// This entity is used as a reference point for spawning customers.
    /// </summary>
    [Library("cashier_customerspawner")]
	public partial class CustomerSpawner : Entity
	{
        public override void Spawn()
        {
            
            base.Spawn();
        }
        /// <summary>
        /// This method is used to summon a customer at this spawner.
        /// It requires a CustomerHotspot for the customer to walk to.
        /// <see cref="CustomerHotspot"/>
        /// <param name="hotspot">The hotspot for the customer to walk to. If the hotspot is taken, this method will return null.</param>
        /// </summary>
        public Customer SummonCustomer(CustomerHotspot hotspot)
        {
            if (!hotspot.Taken)
            {
                Sound.FromEntity(SoundEvents.EntranceTone, this);
                // Spawn a customer to walk to the hotspot.
                Customer customer = new Customer();
                customer.Position = Position;
                customer.Steer.Target = hotspot.Position;
                customer.Hotspot = hotspot;
                hotspot.Taken = true;
                return customer;
            }
            return null;
        }
    }
}