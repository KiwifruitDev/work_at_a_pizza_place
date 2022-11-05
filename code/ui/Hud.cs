using Sandbox.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace WorkAtAPizzaPlace
{
	/// <summary>
	/// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
	/// via RootPanel on this entity, or Local.Hud.
	/// </summary>
	public partial class Hud : Sandbox.HudEntity<RootPanel>
	{
		public Hud()
		{
			if ( IsClient )
			{
				RootPanel.SetTemplate( "/ui/hud.html" );
				RootPanel.AddChild<CursorController>();
				RootPanel.AddChild<MouseCursor>();
			}
		}
	}

}
