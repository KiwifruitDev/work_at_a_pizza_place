using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkAtAPizzaPlace
{
	/// <summary>
    /// This entity is an order station used to take customers' orders.
    /// </summary>
    [Library("cashier_orderpicker")]
    [Hammer.Model(Model = "models/pizzaplace/orderpicker.vmdl", BodyGroup = "{ base = 1 button1 = 0 button2 = 0 button3 = 0 button4 = 0 }")]
	public partial class OrderPicker : ClickableObject
	{
        public Customer Customer { get; set; }
        [Property( Title = "Linked Customer Spawner" )]
        public string Spawner { get; set; }
        Entity ActualSpawner { get; set; }
        [Property( Title = "Linked Customer Hotspot" )]
        public string Hotspot { get; set; }
        Entity ActualHotspot { get; set; }
        public override void Spawn()
        {
            SetModel( "models/pizzaplace/orderpicker.vmdl" );
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic, false);
            EnableAllCollisions = true;
            EnableHitboxes = true;
            MoveType = MoveType.None;
            Log.Info("Spawner: "+Spawner+", Hotspot: "+Hotspot);
            ActualSpawner = Entity.FindByName( Spawner );
            ActualHotspot = Entity.FindByName( Hotspot );
            Log.Error("ActualSpawner: "+ActualSpawner+", ActualHotspot: "+ActualHotspot);
            // Spawn a customer.
            if ( ActualSpawner != null && ActualHotspot != null )
            {
                Log.Error("Spawning customer");
                //SetCustomer(ActualSpawner.SummonCustomer(ActualHotspot));
            }
            base.Spawn();
        }
        /// <summary>
        /// This method is used whenever the player clicks on the order station.
        /// It sends a command to the server to select the customer's order.
        /// <see cref="Enumerables.Order"/>
        /// <param name="order">The order to be selected.</param>
        /// </summary>
        [ServerCmd, Input]
        public void ClientTakeOrder(Enumerables.Order order)
        {
            if (Customer != null)
            {
                Customer.OrderSelected(order);
                Customer = null;
            }
        }
        /// <summary>
        /// This method sets the customer entity linked to this order picker.
        /// <see cref="Customer"/>
        /// <param name="customer">The customer entity linked to this order picker.</param>
        /// </summary>
        [Input]
        public void SetCustomer(Customer customer)
        {
            this.Customer = customer;
        }
        /// <summary>
        /// This method is called when the player clicks on the object.
        /// <param name="hitboxIndex">The index of the hitbox clicked.</param>
        /// </summary>
        [ClientRpc]
        public override void Click(int hitboxIndex)
        {

        }
        /// <summary>
        /// This method is called whenever the player hovers over the object.
        /// Return true if the player can click on the object.
        /// <param name="hitboxIndex">The index of the hitbox selected.</param>
        /// </summary>
        public override bool HitBoxHandler(int hitboxIndex)
		{
			// Let's see if we're hitting a button.
            // +1 because the first hitbox is the root.
			switch( hitboxIndex )
			{
				case 1:
					// Pepperoni Pizza order.
                    Log.Error( "Pepperoni Pizza" );
                    return true;
                case 2:
                    // Sausage Pizza order.
                    Log.Error( "Sausage Pizza" );
                    return true;
                case 3:
                    // Cheese Pizza order.
                    Log.Error( "Cheese Pizza" );
                    return true;
                case 4:
                    // Fizzly (drink) order.
                    Log.Error( "Fizzly" );
                    return true;
			}
            return false;
		}
    }
}