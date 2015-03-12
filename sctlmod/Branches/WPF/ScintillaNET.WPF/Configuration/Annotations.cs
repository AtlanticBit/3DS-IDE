using System;
using System.ComponentModel;


namespace ScintillaNET.WPF.Configuration
{
	public sealed class Annotations : ScintillaWPFConfigItem
	{
		private const AnnotationsVisibility DefaultAnnotationVisibility = AnnotationsVisibility.Hidden;
		private AnnotationsVisibility? mAnnotationVisibility;
		/// <summary>
		/// Gets or sets the visibility style for all annotations.
		/// </summary>
		/// <returns>
		/// One of the <see cref="AnnotationsVisibility" /> values.
		/// The default is <see cref="AnnotationsVisibility.Hidden" />.
		/// </returns>
		/// <exception cref="InvalidEnumArgumentException">The value assigned is not one of the <see cref="AnnotationsVisibility" /> values.</exception>
		/// <remarks>
		/// This is named differently because WPF doesn't like it otherwise.
		/// </remarks>
		[Category("Appearance")]
		[Description("Indicates the visibility and appearance of annotations.")]
		[DefaultValue(typeof(AnnotationsVisibility), "Hidden")]
		public AnnotationsVisibility AnnotationVisibility
		{
			get { return (mAnnotationVisibility ?? (AnnotationsVisibility?)DefaultAnnotationVisibility).Value; }
			set { mAnnotationVisibility = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (mAnnotationVisibility != null)
				scintilla.Annotations.Visibility = AnnotationVisibility;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
			scintilla.Annotations.Visibility = AnnotationsVisibility.Hidden;
		}
	}
}
