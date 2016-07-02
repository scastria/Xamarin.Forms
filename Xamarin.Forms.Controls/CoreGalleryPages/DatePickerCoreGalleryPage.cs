using System;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls
{
	internal class DatePickerCoreGalleryPage : CoreGalleryPage<DatePicker>
	{
		private const string NULL_PLACEHOLDER = "Select date...";

		protected override bool SupportsTapGestureRecognizer => false;

		protected override void Build(StackLayout stackLayout)
		{
			base.Build(stackLayout);

			var dateContainer = new ViewContainer<DatePicker>(Test.DatePicker.Date,
				new DatePicker { Date = new DateTime(1987, 9, 13), NullPlaceholder = NULL_PLACEHOLDER });

			var dateSelectedContainer = new EventViewContainer<DatePicker>(Test.DatePicker.DateSelected, new DatePicker { NullPlaceholder = NULL_PLACEHOLDER });
			dateSelectedContainer.View.DateSelected += (sender, args) => dateSelectedContainer.EventFired();

			var formatDateContainer = new ViewContainer<DatePicker>(Test.DatePicker.Format, new DatePicker { Format = "ddd", NullPlaceholder = NULL_PLACEHOLDER });
			var minimumDateContainer = new ViewContainer<DatePicker>(Test.DatePicker.MinimumDate,
				new DatePicker { MinimumDate = new DateTime(1987, 9, 13), NullPlaceholder = NULL_PLACEHOLDER });
			var maximumDateContainer = new ViewContainer<DatePicker>(Test.DatePicker.MaximumDate,
				new DatePicker { MaximumDate = new DateTime(2087, 9, 13), NullPlaceholder = NULL_PLACEHOLDER });
			var textColorContainer = new ViewContainer<DatePicker>(Test.DatePicker.TextColor,
				new DatePicker { Date = new DateTime(1978, 12, 24), TextColor = Color.Lime, NullPlaceholder = NULL_PLACEHOLDER, HasClearButton = true });

			Add(dateContainer);
			Add(dateSelectedContainer);
			Add(formatDateContainer);
			Add(minimumDateContainer);
			Add(maximumDateContainer);
			Add(textColorContainer);
		}
	}
}