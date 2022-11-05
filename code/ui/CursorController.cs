using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkAtAPizzaPlace
{
	// RTS gamemode cursor controller, used here for in-game interaction.
	// Based on https://github.com/Facepunch/sbox-rts/blob/master/code/ui/CursorController.cs
	public class CursorController : Panel
	{
		public CursorController()
		{
			StyleSheet.Load( "/ui/CursorController.scss" );
		}

		public override void Tick()
		{
			base.Tick();
		}


		[Event.BuildInput]
		private void BuildInput( InputBuilder builder )
		{
			if ( Local.Pawn is not Player player )
				return;
			
			// Do a trace from the cursor to the world.
    		TraceResult trace = Trace.Ray( builder.Cursor.Origin, Input.Cursor.Origin + Input.Cursor.Direction.Normal * 10000f ).WorldAndEntities().WithoutTags("player").WithAnyTags(new string[1] {"clickable"}).UseHitboxes(true).Run();
			// If we hit an entity, let's see if it's a valid target.
			if ( trace.Entity is not null )
			{
				// If it's a valid target, let's see if we can interact with it.
				if ( (trace.Entity as ClickableObject).HitBoxHandler(trace.Bone) )
				{
					MouseCursor.Instance.Cursor.SetTexture( "materials/pizzaplace/cursor/select.png" );
					if ( builder.Pressed( InputButton.Attack1 ) )
					{
						// If we can interact with it, let's do so.
						(trace.Entity as ClickableObject).Click(trace.Bone);
					}
				}
			}
			else
			{
				// Reset the cursor.
				MouseCursor.Instance.Cursor.SetTexture( "materials/pizzaplace/cursor/normal.png" );
			}
		}
	}
}