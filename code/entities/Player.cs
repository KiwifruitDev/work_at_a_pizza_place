using Sandbox;

namespace WorkAtAPizzaPlace
{
	partial class PizzaPlacePlayer : Player
	{
		private DamageInfo lastDamage;
		[Net, Predicted] public PizzaPlaceCamera MainCamera { get; set; }
		[Net, Predicted] public FirstPersonCamera FpCamera { get; set; }
		[Net, Predicted] public SpectateRagdollCamera SpectateCamera { get; set; }
		[Net, Predicted] public PizzaPlaceWalkController WalkController { get; set; }
		public Clothing.Container Clothing = new();
		public PizzaPlacePlayer()
		{
			// Don't do anything yet.
		}
		public PizzaPlacePlayer( Client cl ) : this()
		{
			// Load clothing from client data
			Clothing.LoadFromClient( cl );
		}
		public override void Spawn()
		{
			MainCamera = new PizzaPlaceCamera();
			FpCamera = new FirstPersonCamera();
			SpectateCamera = new SpectateRagdollCamera();
			Tags.Add( "player" );
			base.Spawn();
		}
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			//
			// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
			//
			Animator = new StandardPlayerAnimator();

			//
			// Use ThirdPersonCamera (you can make your own Camera for 100% control)
			//
			Camera = MainCamera;

			//
			// Use WalkController for movement (you can make your own PlayerController for 100% control)
			//
			WalkController = new PizzaPlaceWalkController();
			Controller = WalkController;

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Clothing.DressEntity( this );

			base.Respawn();
		}

		public override void TakeDamage( DamageInfo info )
		{
			if ( GetHitboxGroup( info.HitboxIndex ) == 1 )
			{
				info.Damage *= 10.0f;
			}

			lastDamage = info;

			base.TakeDamage( info );
		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			//
			// If you have active children (like a weapon etc) you should call this to 
			// simulate those too.
			//
			SimulateActiveChild( cl, ActiveChild );
		}

		public override void OnKilled()
		{
			base.OnKilled();

			EnableDrawing = false;
			BecomeRagdollOnClient( Velocity, lastDamage.Flags, lastDamage.Position, lastDamage.Force, GetHitboxBone( lastDamage.HitboxIndex ) );
			Camera = SpectateCamera;
			Controller = null;
			EnableAllCollisions = false;
			EnableDrawing = false;
		}
	}
}
