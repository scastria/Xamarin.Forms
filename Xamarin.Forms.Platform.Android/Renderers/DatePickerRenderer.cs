using System;
using System.ComponentModel;
using Android.App;
using Android.Content.Res;
using Android.Content;
using Android.Widget;
using AView = Android.Views.View;
using Object = Java.Lang.Object;

namespace Xamarin.Forms.Platform.Android
{
	public class DatePickerRenderer : ViewRenderer<DatePicker, EditText>
	{
		DatePickerDialog _dialog;
		bool _disposed;
		TextColorSwitcher _textColorSwitcher;

		public DatePickerRenderer()
		{
			AutoPackage = false;
			if (Forms.IsLollipopOrNewer)
				Device.Info.PropertyChanged += DeviceInfoPropertyChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				if (Forms.IsLollipopOrNewer)
					Device.Info.PropertyChanged -= DeviceInfoPropertyChanged;

				_disposed = true;
				if (_dialog != null)
				{
					_dialog.CancelEvent -= OnCancelButtonClicked;
					_dialog.Hide();
					_dialog.Dispose();
					_dialog = null;
				}
			}
			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				var textField = new EditText(Context) { Focusable = false, Clickable = true, Tag = this };

				textField.SetOnClickListener(TextFieldClickHandler.Instance);
				SetNativeControl(textField);
				_textColorSwitcher = new TextColorSwitcher(textField.TextColors); 
			}

			SetDate(Element.Date);

			UpdateMinimumDate();
			UpdateMaximumDate();
			UpdateTextColor();
			UpdateNullPlaceholder();
			UpdateHasClearButton();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == DatePicker.DateProperty.PropertyName || e.PropertyName == DatePicker.FormatProperty.PropertyName)
				SetDate(Element.Date);
			else if (e.PropertyName == DatePicker.MinimumDateProperty.PropertyName)
				UpdateMinimumDate();
			else if (e.PropertyName == DatePicker.MaximumDateProperty.PropertyName)
				UpdateMaximumDate();
			else if (e.PropertyName == DatePicker.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == DatePicker.NullPlaceholderProperty.PropertyName)
				UpdateNullPlaceholder();
			else if (e.PropertyName == DatePicker.HasClearButtonProperty.PropertyName)
				UpdateHasClearButton();
		}

		internal override void OnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
		{
			base.OnFocusChangeRequested(sender, e);

			if (e.Focus)
				OnTextFieldClicked();
			else if (_dialog != null)
			{
				_dialog.Hide();
				((IElementController)Element).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
				Control.ClearFocus();
				_dialog.CancelEvent -= OnCancelButtonClicked;
				_dialog = null;
			}
		}

		void CreateDatePickerDialog(int year, int month, int day)
		{
			_dialog = new DatePickerDialog(Context, (o, e) =>
			{
				AcceptValueFromDialog(e.Date);
			}, year, month, day);
			if(Element.HasClearButton)
				_dialog.SetButton((int)DialogButtonType.Neutral, "Clear", OnClearButtonClicked);
		}

		void AcceptValueFromDialog(DateTime? date)
		{
			DatePicker view = Element;
			view.Date = date;
			((IElementController)view).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
			Control.ClearFocus();

			_dialog.CancelEvent -= OnCancelButtonClicked;
			_dialog = null;
		}

		void DeviceInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Device.Info.CurrentOrientation))
			{
				DatePickerDialog currentDialog = _dialog;
				if (currentDialog != null && currentDialog.IsShowing)
				{
					currentDialog.Dismiss();
					CreateDatePickerDialog(currentDialog.DatePicker.Year, currentDialog.DatePicker.Month, currentDialog.DatePicker.DayOfMonth);
					_dialog.Show();
				}
			}
		}

		void OnTextFieldClicked()
		{
			DatePicker view = Element;
			((IElementController)view).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);

			DateTime initDate = DateTime.Now.Date;
			if (view.Date != null)
				initDate = view.Date.Value;
			CreateDatePickerDialog(initDate.Year, initDate.Month - 1, initDate.Day);

			UpdateMinimumDate();
			UpdateMaximumDate();

			_dialog.CancelEvent += OnCancelButtonClicked;
			_dialog.Show();
		}

		void OnCancelButtonClicked(object sender, EventArgs e)
		{
			Element.Unfocus();
		}

		void OnClearButtonClicked(object sender, DialogClickEventArgs de)
		{
			AcceptValueFromDialog(null);
		}

		void SetDate(DateTime? date)
		{
			if (date == null)
				Control.Text = null;
			else
				Control.Text = date.Value.ToString(Element.Format);
		}

		void UpdateMaximumDate()
		{
			if (_dialog != null)
			{
				_dialog.DatePicker.MaxDate = (long)Element.MaximumDate.ToUniversalTime().Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
			}
		}

		void UpdateMinimumDate()
		{
			if (_dialog != null)
			{
				_dialog.DatePicker.MinDate = (long)Element.MinimumDate.ToUniversalTime().Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
			}
		}

		void UpdateTextColor()
		{
			_textColorSwitcher?.UpdateTextColor(Control, Element.TextColor);
		}

		void UpdateNullPlaceholder()
		{
			Control.Hint = Element.NullPlaceholder;
		}

		void UpdateHasClearButton()
		{
			//Nothing to do since dialog is created fresh each time
		}

		class TextFieldClickHandler : Object, IOnClickListener
		{
			public static readonly TextFieldClickHandler Instance = new TextFieldClickHandler();

			public void OnClick(AView v)
			{
				((DatePickerRenderer)v.Tag).OnTextFieldClicked();
			}
		}
	}
}