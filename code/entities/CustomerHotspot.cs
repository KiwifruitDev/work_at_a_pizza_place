using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkAtAPizzaPlace
{
	/// <summary>
    /// This entity is used as a reference point for the customer to walk to.
    /// </summary>
    [Library("cashier_customerhotspot")]
	public partial class CustomerHotspot : Entity
	{
        public bool Taken = false;
    }
}