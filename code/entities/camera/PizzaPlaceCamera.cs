using Sandbox;

namespace WorkAtAPizzaPlace
{
    /// <summary>
    /// This class re-implements ThirdPersonCamera to only rotate when right click is held down.
	/// Also, the camera allows the player to switch to first person mode seamlessly.
    /// </summary>
    public class PizzaPlaceCamera : ThirdPersonCamera
    {
        private Angles orbitAngles;
		private float orbitDistance = 150;
		public float cameraDistance = 130.0f;
		[ConVar.Replicated]
		public static float pizzaplace_camerasensitivity { get; set; } = 1f;
		public Angles fakeInput;
		Vector3 lastPos;

		public void FirstPersonActivated()
		{
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			Pos = pawn.EyePos;
			Rot = pawn.EyeRot;

			lastPos = Pos;
		}

		public void FirstPersonUpdate()
		{
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			var eyePos = pawn.EyePos;
			if ( eyePos.Distance( lastPos ) < 300 ) // TODO: Tweak this, or add a way to invalidate lastpos when teleporting
			{
				Pos = Vector3.Lerp( eyePos.WithZ( lastPos.z ), eyePos, 20.0f * Time.Delta );
			}
			else
			{
				Pos = eyePos;
			}

			Rot = Rotation.From(fakeInput); //pawn.EyeRot;

			FieldOfView = 80;

			Viewer = pawn;
			lastPos = Pos;
		}
		public void ThirdPersonUpdate()
		{
			var pawn = Local.Pawn as AnimEntity;
			var client = Local.Client;

			if ( pawn == null )
				return;

			Pos = pawn.Position;
			Vector3 targetPos;

			var center = pawn.Position + Vector3.Up * 64;

			if ( thirdperson_orbit )
			{
				Pos += Vector3.Up * (pawn.CollisionBounds.Center.z * pawn.Scale);
				Rot = Rotation.From( orbitAngles );

				targetPos = Pos + Rot.Backward * orbitDistance;
			}
			else
			{
				Pos = center;
				Rot = Rotation.From(fakeInput); //Rotation.FromAxis( Vector3.Up, 4 ) * Rotation.From(fakeInput);

				float distance = cameraDistance * pawn.Scale;
				targetPos = Pos; //+ Rotation.From(fakeInput).Right * ((pawn.CollisionBounds.Maxs.x + 15) * pawn.Scale);
				targetPos += Rotation.From(fakeInput).Forward * -distance;
			}

			if ( thirdperson_collision )
			{
				var tr = Trace.Ray( Pos, targetPos )
					.Ignore( pawn )
					.Radius( 8 )
					.Run();

				Pos = tr.EndPos;
			}
			else
			{
				Pos = targetPos;
			}

			FieldOfView = 70;

			Viewer = null;
		}
        public override void Update()
		{
			// Are we in first person?
			if(cameraDistance <= 25)
			{
				FirstPersonUpdate();
			}
			else
			{
				ThirdPersonUpdate();
			}
		}
        public override void BuildInput( InputBuilder input )
		{
			base.BuildInput( input );
			// When the player holds down right click, orbit the camera.
			if (input.Down(InputButton.Attack2 ))
			{
				fakeInput = new Angles(MathX.Clamp(fakeInput.pitch + Mouse.Delta.y * pizzaplace_camerasensitivity, -90, 90), fakeInput.yaw - Mouse.Delta.x * pizzaplace_camerasensitivity, 0);
			}
			// This is absolutely required or else the player will not be able to move in the direction they are looking.
			if (input.InputDirection != Vector3.Zero)
			{
				input.ViewAngles = fakeInput;
			}
			// Scroll wheel controls the distance from the camera to the player.
			cameraDistance -= input.MouseWheel * 50;
			if(cameraDistance <= 25)
			{
				cameraDistance = 25;
			}
			else if(cameraDistance >= 1000)
			{
				cameraDistance = 1000;
			}
		}
    }
}