using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkAtAPizzaPlace
{
	/// <summary>
    /// Customer entity for receiving orders.
    /// Uses AI Lab code for the customer AI.
    /// https://github.com/Facepunch/sbox-ai-lab/blob/master/code/NpcTest.cs
    /// </summary>
    [Library("cashier_customer")]
    [Hammer.Model(Model = "models/citizen/citizen.vmdl")]
	public partial class Customer : ClickableObject
	{
        [ConVar.Replicated]
        public static bool customer_drawpath { get; set; }
        [Property( Title = "Order" ) ]
        public Enumerables.Order Order { get; set; }
        [Property( Title = "New Customer Timeout" ) ]
        public float Timeout { get; set; }
        public bool IsOrderComplete { get; set; }
        /// <summary>
        /// This value does not set Steer.Target, which is used by the AI Lab code to steer the NPC.
        /// It is used to allow new customers to use this space once this customer's order is complete.
        /// </summary>
        public CustomerHotspot Hotspot;
        float Speed;
	    NavPath Path = new NavPath();
	    public NavSteer Steer;
        public Sandbox.Debug.Draw Draw => Sandbox.Debug.Draw.Once;
        Vector3 InputVelocity;
	    Vector3 LookDir;
        public Clothing.Container Clothing = new();
        [Event.Tick.Server]
        public void Tick()
        {
            using var _a = Sandbox.Debug.Profile.Scope( "NpcTest::Tick" );

            InputVelocity = 0;

            if ( Steer != null )
            {
                using var _b = Sandbox.Debug.Profile.Scope( "Steer" );

                Steer.Tick( Position );

                if ( !Steer.Output.Finished )
                {
                    InputVelocity = Steer.Output.Direction.Normal;
                    Velocity = Velocity.AddClamped( InputVelocity * Time.Delta * 500, Speed );
                }

                if ( customer_drawpath )
                {
                    Steer.DebugDrawPath();
                }
            }

            using ( Sandbox.Debug.Profile.Scope( "Move" ) )
            {
                Move( Time.Delta );
            }

            var walkVelocity = Velocity.WithZ( 0 );
            if ( walkVelocity.Length > 0.5f )
            {
                var turnSpeed = walkVelocity.Length.LerpInverse( 0, 100, true );
                var targetRotation = Rotation.LookAt( walkVelocity.Normal, Vector3.Up );
                Rotation = Rotation.Lerp( Rotation, targetRotation, turnSpeed * Time.Delta * 20.0f );
            }

            var animHelper = new CitizenAnimationHelper( this );

            LookDir = Vector3.Lerp( LookDir, InputVelocity.WithZ( 0 ) * 1000, Time.Delta * 100.0f );
            animHelper.WithLookAt( EyePos + LookDir );
            animHelper.WithVelocity( Velocity );
            animHelper.WithWishVelocity( InputVelocity );		
        }
        protected virtual void Move( float timeDelta )
        {
            var bbox = BBox.FromHeightAndRadius( 64, 4 );
            //DebugOverlay.Box( Position, bbox.Mins, bbox.Maxs, Color.Green );

            MoveHelper move = new( Position, Velocity );
            move.MaxStandableAngle = 50;
            move.Trace = move.Trace.Ignore( this ).Size( bbox );

            if ( !Velocity.IsNearlyZero( 0.001f ) )
            {
            //	Sandbox.Debug.Draw.Once
            //						.WithColor( Color.Red )
            //						.IgnoreDepth()
            //						.Arrow( Position, Position + Velocity * 2, Vector3.Up, 2.0f );

                using ( Sandbox.Debug.Profile.Scope( "TryUnstuck" ) )
                    move.TryUnstuck();

                using ( Sandbox.Debug.Profile.Scope( "TryMoveWithStep" ) )
                    move.TryMoveWithStep( timeDelta, 30 );
            }

            using ( Sandbox.Debug.Profile.Scope( "Ground Checks" ) )
            {
                var tr = move.TraceDirection( Vector3.Down * 10.0f );

                if ( move.IsFloor( tr ) )
                {
                    GroundEntity = tr.Entity;

                    if ( !tr.StartedSolid )
                    {
                        move.Position = tr.EndPos;
                    }

                    if ( InputVelocity.Length > 0 )
                    {
                        var movement = move.Velocity.Dot( InputVelocity.Normal );
                        move.Velocity = move.Velocity - movement * InputVelocity.Normal;
                        move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
                        move.Velocity += movement * InputVelocity.Normal;

                    }
                    else
                    {
                        move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
                    }
                }
                else
                {
                    GroundEntity = null;
                    move.Velocity += Vector3.Down * 900 * timeDelta;
                    Sandbox.Debug.Draw.Once.WithColor( Color.Red ).Circle( Position, Vector3.Up, 10.0f );
                }
            }

            Position = move.Position;
            Velocity = move.Velocity;
        }
        public override void Spawn()
        {
            // Set clickable tag.
            Tags.Add("clickable");

            // Import the AI Lab code.
            SetModel( "models/citizen/citizen.vmdl" );
		    EyePos = Position + Vector3.Up * 64;
            CollisionGroup = CollisionGroup.Player;
            SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 72, 8 ) );
            EnableHitboxes = true;
            this.SetMaterialGroup( Rand.Int( 0, 3 ) );
            SetBodyGroup( 1, 0 );
		    Speed = Rand.Float( 100, 300 );

            // Get a random client to load clothing from.
            int clientIndex = Rand.Int( 0, Client.All.Count-1 );
            Client client = Client.All[clientIndex];
            Clothing.LoadFromClient( client );

            base.Spawn();
        }
        /// <summary>
        /// This is called when the customer receives their order.
        /// In gameplay, if the order is wrong, the customer will leave the restaurant.
        /// If the order is correct, the customer will stay for a bit and the order will go through.
        /// <param name="thisOrder">The order that the customer received.</param>
        /// </summary>
        [Input]
        public void OrderSelected(Enumerables.Order thisOrder)
        {
            IsOrderComplete = true;
            if (thisOrder == Order)
            {
                // Order is correct.
                Log.Error("Order is correct.");
            }
            else
            {
                // Order is wrong.
                Log.Error("Order is wrong.");
            }
        }
        /// <summary>
        /// This method is called when the player clicks on the object.
        /// <param name="hitboxIndex">The index of the hitbox clicked.</param>
        /// </summary>
        [ClientRpc]
        public override void Click(int hitboxIndex)
        {
            // Do nothing.
            Log.Error("Click() called on base class.");
        }
        /// <summary>
        /// This method is called whenever the player hovers over the object.
        /// Return true if the player can click on the object.
        /// <param name="hitboxIndex">The index of the hitbox selected.</param>
        /// </summary>
        public override bool HitBoxHandler(int hitboxIndex)
		{
            // Clickable regions are defined in the model file.
            return false;
		}
    }
}