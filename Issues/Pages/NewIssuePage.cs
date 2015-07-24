using System;
using System.Collections.Generic;

using Xamarin.Forms;
using ReactiveUI;

namespace Issues
{
	public class NewIssuePage : ContentPage, IViewFor<NewIssueViewModel>
	{
		const string FROM_CAMERA = "Use Camera";
		const string FROM_PHOTOS = "Choose from Photos";

		public NewIssuePage ()
		{
			ViewModel = new NewIssueViewModel ();

			Title = "New Issue";

			/* Setup the toolbar */
			var addButton = new ToolbarItem { Text = "Add" };
			addButton.Clicked += (sender, e) => {
			};
			ToolbarItems.Add (addButton);

			/* Setup the grid */
			var grid = new Grid {
				VerticalOptions = LayoutOptions.FillAndExpand,
				RowDefinitions = {
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto }
				},
				ColumnDefinitions = {
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = GridLength.Auto },
					new ColumnDefinition { Width = GridLength.Auto }
				}
			};

			/* Row 1 */
			var cameraImage = new Image { Source = "camera-50.png", WidthRequest = 20, HeightRequest = 20 };
			cameraImage.GestureRecognizers.Add (new TapGestureRecognizer (async (v, o) => {
				var action = await DisplayActionSheet (
					"Attach Photo",
					"Cancel",
					null,
					FROM_CAMERA,
					FROM_PHOTOS
				);

				if (action == FROM_CAMERA) {
					await ViewModel.TakePhoto ();
				} else if (action == FROM_PHOTOS) {
					await ViewModel.SelectPhoto ();
				}
			}));
			var subjectLabel = new Label {
				XAlign = TextAlignment.End,
				YAlign = TextAlignment.Center,
				Text = "Subject",
				TextColor = Color.FromHex ("777")
			};
			var subjectEntry = new Entry { WidthRequest = 220 };
			subjectEntry.SetBinding (Entry.TextProperty, "Subject");

			grid.Children.Add (subjectLabel, 0, 1, 0, 1);
			grid.Children.Add (subjectEntry, 1, 2, 0, 1);
			grid.Children.Add (cameraImage, 2, 3, 0, 1);


			/* Row 2 */
			var locationEntry = new Entry { WidthRequest = 220 };
			locationEntry.SetBinding (Entry.TextProperty, "Location.Name");
			locationEntry.Focused += (sender, e) => {
				System.Diagnostics.Debug.WriteLine ("PICKER");
			};

			var locationLabel = new Label {
				XAlign = TextAlignment.End,
				YAlign = TextAlignment.Center,
				Text = "Location",
				TextColor = Color.FromHex ("777")
			};

			grid.Children.Add (locationLabel, 0, 1, 1, 2);
			grid.Children.Add (locationEntry, 1, 2, 1, 2);


			/* Buttons */
			var normal = new Button { WidthRequest = 100, Text = "Normal", TextColor = Color.FromHex ("468ee5") };
			var medium = new Button { WidthRequest = 100, Text = "Medium", TextColor = Color.FromHex ("f59d00") };
			var urgent = new Button { WidthRequest = 100, Text = "Urgent", TextColor = Color.FromHex ("dc3d06") };

			normal.SetBinding (Button.ImageProperty, "NormalButtonSource");
			medium.SetBinding (Button.ImageProperty, "MediumButtonSource");
			urgent.SetBinding (Button.ImageProperty, "UrgentButtonSource");

			/* Image */
			var image = new Image { HeightRequest = 300 };
			image.SetBinding (Image.SourceProperty, "Source");

			/* Description editor */
			var editor = new Editor ();
			editor.SetBinding (Editor.TextProperty, "Description");

			Content = new StackLayout {
				Padding = new Thickness (20, 20, 20, 20),
				VerticalOptions = LayoutOptions.Start,
				Orientation = StackOrientation.Vertical,
				Children = {
					grid,
					new StackLayout {
						Orientation = StackOrientation.Horizontal,
						Children = {
							normal, medium, urgent
						}
					},
					image,
					editor
				}
			};
		}

		public NewIssueViewModel ViewModel {
			get { return (NewIssueViewModel)GetValue (ViewModelProperty); }
			set { SetValue (ViewModelProperty, value); }
		}

		public static readonly BindableProperty ViewModelProperty =
			BindableProperty.Create<NewIssuePage, NewIssueViewModel> (x => x.ViewModel, default (NewIssueViewModel), BindingMode.OneWay);

		object IViewFor.ViewModel {
			get { return ViewModel; }
			set { ViewModel = (NewIssueViewModel)value; }
		}
	}
}
