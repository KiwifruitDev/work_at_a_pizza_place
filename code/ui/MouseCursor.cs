using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace WorkAtAPizzaPlace
{
	// RTS gamemode cursor, used here for in-game interaction.
	// Based on https://github.com/Facepunch/sbox-rts/blob/master/code/ui/MouseCursor.cs
	public class MouseCursor : Panel
	{
		public static MouseCursor Instance { get; private set; }

		public Image Cursor { get; private set; }

		public MouseCursor() : base()
		{
			StyleSheet.Load( "/ui/MouseCursor.scss" );

			Cursor = Add.Image( "", "cursor" );
			Cursor.SetTexture( "materials/pizzaplace/cursor/normal.png" );

			Instance = this;
		}

		public override void Tick()
		{
			var mousePosition = Mouse.Position / Screen.Size;

			// The cursor image is in the center of the real cursor as the real cursor cannot be hidden.
			// Therefore, cursors need to be designed as hotspots.
			Cursor.Style.Left = Length.Fraction( mousePosition.x - Cursor.Texture.Width/2 );
			Cursor.Style.Top = Length.Fraction( mousePosition.y - Cursor.Texture.Height/2 );
			Cursor.Style.Dirty();

			base.Tick();
		}
	}
}