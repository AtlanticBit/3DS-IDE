#region Using Directives

using System;
using System.Collections;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Collections.Generic;

#endregion Using Directives


namespace ScintillaNet.Design
{
	// Provides additional design-time support for the Scintilla control
	internal class ScintillaDesigner : ControlDesigner
	{
		#region Fields

		private Scintilla _scintilla;

		#endregion Fields


		#region Methods

		private Attribute[] GetCustomAttributes(string propertyName)
		{
			List<Attribute> attrs = new List<Attribute>();
			foreach (Attribute a in _scintilla.GetType().GetProperty(propertyName).GetCustomAttributes(false))
				attrs.Add(a);

			return attrs.ToArray();
		}


		public override void Initialize(IComponent component)
		{
			_scintilla = (Scintilla)component;
			Wrapping.PropertyChanged += new PropertyChangedEventHandler(Wrapping_PropertyChanged);

			base.Initialize(component);
		}


		public override void InitializeNewComponent(IDictionary defaultValues)
		{
			base.InitializeNewComponent(defaultValues);

			// By default the VS control designer sets the Text property to the control
			// name instead of the default value... which is what we'll do here.
			_scintilla.Text = null;
		}


		protected override void PostFilterProperties(IDictionary properties)
		{
			// To support the Reset option in the property grid a property must
			// have a setter (amongst other things). Most of ours do not because
			// the public API does not require it. To offer a Reset option we
			// instead tell the designer to use the following properties in this
			// class as if they were on the control class, thereby allowing us
			// to provide a (hidden) setter and get Reset functionality.

			// All your property are belong to us ;)

			properties["Scrolling"] = TypeDescriptor.CreateProperty(
				GetType(),
				"Scrolling",
				typeof(Scrolling),
				GetCustomAttributes("Scrolling"));

			properties["Wrapping"] = TypeDescriptor.CreateProperty(
				GetType(),
				"Wrapping",
				typeof(Wrapping),
				GetCustomAttributes("Wrapping"));

			base.PostFilterProperties(properties);
		}


		private void ResetScrolling()
		{
			Util.Reset(Scrolling);
		}


		private void ResetWrapping()
		{
			Util.Reset(Wrapping);
		}


		private bool ShouldSerializeScrolling()
		{
			return Util.ShouldSerialize(Scrolling);
		}


		private bool ShouldSerializeWrapping()
		{
			return Util.ShouldSerialize(Wrapping);
		}

		private void Wrapping_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// Reset the indent and indicator when there is no wrapping
			if (e.PropertyName == "Mode" && Wrapping.Mode == WrappingMode.None)
			{
				Wrapping.Indent = 0;
				Wrapping.Indicator = WrappingIndicator.None;
			}
		}

		#endregion Methods


		#region Properties

		public Scrolling Scrolling
		{
			get { return _scintilla.Scrolling; }
			set { }
		}

		public Wrapping Wrapping
		{
			get { return _scintilla.Wrapping; }
			set { }
		}

		#endregion Properties
	}
}
